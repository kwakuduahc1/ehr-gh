using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Asp.Versioning;
using Serilog;
using ShimsServer.Context;
using ShimsServer.Repositories;
using System.Net;
using System.Text;
using Asp.Versioning.ApiExplorer;

namespace ShimsServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog with memory optimization
            var minLevel = builder.Environment.IsDevelopment()
                ? Serilog.Events.LogEventLevel.Information
                : Serilog.Events.LogEventLevel.Warning; // Reduce logging in production

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(minLevel)
                .WriteTo.Console()
                .WriteTo.File(
                    "logs/shims-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7, // Reduced from 30 for low-memory environments
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddDbContextPool<ApplicationDbContext>(o =>
            {
                o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), opt =>
                {
                    opt.UseRelationalNulls(true)
                       .EnableRetryOnFailure(3)
                       .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }).UseLowerCaseNamingConvention()
                .EnableDetailedErrors(builder.Environment.IsDevelopment())
                .EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
            }, poolSize: 16); // Reduced from default 128 for low-memory environments

            builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("DefaultConnection")!, x =>
            {
                x.EnableDynamicJson()
                 .EnableParameterLogging(builder.Environment.IsDevelopment());
            });

            builder.Services.AddScoped<IConnection, Connection>();
            builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
            builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            builder.Services.AddScoped<ISchemesRepository, SchemesRepository>();
            builder.Services.AddScoped<IDrugsRepository, DrugsRepository>();
            builder.Services.AddScoped<ISchemeDrugsRepository, SchemeDrugsRepository>();
            builder.Services.AddScoped<ISchemeServiceRepository, SchemeServiceRepository>();
            builder.Services.AddScoped<ISchemeInvestigationRepository, SchemeInvestigationRepository>();
            builder.Services.AddScoped<ISchemeServicePricingRepository, SchemeServicePricingRepository>();

            builder.Services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = builder.Configuration.GetConnectionString("Valkey");
                o.InstanceName = "shims_";
                // Configure for low-memory environment
                o.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    ConnectRetry = 3,
                    ConnectTimeout = 5000,
                    SyncTimeout = 5000,
                    KeepAlive = 180,
                    DefaultDatabase = 0
                };
            });
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(x =>
            {
                x.SignIn.RequireConfirmedAccount = false;
                x.Password.RequiredLength = 6;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireDigit = false;
                x.Password.RequireUppercase = false;
                x.Password.RequireLowercase = false;
                x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddSingleton<IAppFeatures, AppFeatures>();

            // Get JWT settings directly from configuration
            var jwtSettings = builder.Configuration.GetSection("AppFeatures").Get<AppModel>() ?? throw new InvalidOperationException("AppFeatures configuration section is missing");
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ValidateIssuer = true,
                    RequireExpirationTime = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience
                };
            });

            //builder.Services.AddAuthorizationBuilder()
            //    .AddDefaultPolicy("Default", x => x.RequireAuthenticatedUser());

            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("Developer", policy => policy.RequireRole("Developer"))
                .AddPolicy("SysAdmin", policy => policy.RequireRole("SysAdmin", "Developer"))
                .AddPolicy("Doctor", policy => policy.RequireRole("Doctor", "SysAdmin"))
                .AddPolicy("Nurse", policy => policy.RequireRole("Nurse", "OPDNurse", "Wards", "Doctor", "SysAdmin"))
                .AddPolicy("OPDNurse", policy => policy.RequireRole("OPDNurse", "SysAdmin"))
                .AddPolicy("Wards", policy => policy.RequireRole("Wards", "SysAdmin"))
                .AddPolicy("Pharmacist", policy => policy.RequireRole("Pharmacist", "Pharmacy", "SysAdmin"))
                .AddPolicy("Pharmacy", policy => policy.RequireRole("Pharmacy", "SysAdmin"))
                .AddPolicy("Billing", policy => policy.RequireRole("Billing", "SysAdmin"))
                .AddPolicy("Administration", policy => policy.RequireRole("Administration", "Records", "SysAdmin"))
                .AddPolicy("Records", policy => policy.RequireRole("Records", "SysAdmin"))
                .AddPolicy("Patient", policy => policy.RequireRole("Patient"))
                .AddPolicy("Staff", policy => policy.RequireRole("Doctor", "Nurse", "OPDNurse", "Wards", "Pharmacist", "Pharmacy", "Billing", "Administration", "Records", "Developer", "SysAdmin"));

            builder.Services.AddHealthChecks()
                .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
                .AddRedis(builder.Configuration.GetConnectionString("Valkey")!);

            builder.Services.AddDataProtection();
            builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

            string locs = builder.Configuration.GetSection("AppFeatures").GetValue<string>("Audience") ?? throw new Exception("No access control");
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("bStudioApps",
                    x => x.WithOrigins(locs)
                          .WithHeaders("Content-Type", "Accept", "Origin", "Authorization", "X-XSRF-TOKEN", "XSRF-TOKEN", "enctype", "Access-Control-Allow-Origin", "Access-Control-Allow-Credentials", "bookingsid")
                          .WithMethods("GET", "POST", "OPTIONS", "PUT", "DELETE")
                          .AllowCredentials());
            });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
            });

            builder.Services.AddControllers();
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            builder.Services.AddSignalR(x => x.KeepAliveInterval = TimeSpan.FromSeconds(10));
            builder.Services.AddResponseCaching();
            builder.Services.AddRateLimiter();



            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped(typeof(CancellationToken), sp =>
            {
                var ctx = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
                return ctx?.RequestAborted ?? CancellationToken.None;
            });
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddOpenApi("shims-server");
                builder.Services.AddSwaggerGen(options =>
                {
                    var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider()
                        .GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(description.GroupName, new OpenApiInfo
                        {
                            Version = description.ApiVersion.ToString(),
                            Title = "BStudio EHR API",
                            Description = "API for managing EHR",
                            Contact = new OpenApiContact
                            {
                                Name = "BStudio",
                                Email = "bstudio@bstudio.com"
                            }
                        });
                    }

                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT"
                    });
                });
            }

            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseResponseCaching();
            app.UseRouting();
            app.UseCors("bStudioApps");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = "swagger";
                    c.DefaultModelsExpandDepth(0);
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                });
            }
            app.Run();

        }
    }
}

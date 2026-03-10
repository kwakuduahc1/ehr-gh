
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShimsServer.Context;
using ShimsServer.EndPoints;
using System.Net;
using System.Text;

namespace ShimsServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("DefaultConnection")!, x =>
            {
                x.EnableDynamicJson()
                 .EnableParameterLogging(builder.Environment.IsDevelopment());
            });

            builder.Services.AddDbContext<ApplicationDbContext>(o =>
            {
                o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), opt =>
                {
                    opt.UseRelationalNulls(true)
                       .EnableRetryOnFailure(3)
                       .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }).UseLowerCaseNamingConvention()
                .EnableDetailedErrors(builder.Environment.IsDevelopment())
                .EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
            });

            builder.Services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = builder.Configuration.GetConnectionString("Valkey");
                o.InstanceName = "shims_";
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

            builder.Services.AddAuthorizationBuilder()
                .AddDefaultPolicy("Default", x => x.RequireAuthenticatedUser());

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

            builder.Services.AddControllersWithViews();
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
                builder.Services.AddOpenApi();
            }

            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("bStudioApps");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.UseResponseCaching();
            app.MapSchemesEndPoints();
            app.MapDrugsEndPoints();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.Run();

        }
    }
}

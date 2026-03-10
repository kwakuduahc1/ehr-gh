using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SHIMS.Components;
using SHIMS.Context;
using System.Net;
using Microsoft.AspNetCore.Components.Authorization;
using SHIMS.Components.Account;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

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
    o.InstanceName = "ehr_";
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(x =>
{
    x.SignIn.RequireConfirmedAccount = false;
    x.Password.RequiredLength = 8;
    x.Password.RequireNonAlphanumeric = true;
    x.Password.RequireDigit = true;
    x.Password.RequireUppercase = true;
    x.Password.RequireLowercase = true;
    x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSingleton<IAppFeatures, AppFeatures>();


builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Administrator", policy => policy.RequireRole("Administrator"))
    .AddPolicy("Superuser", policy => policy.RequireRole("Superuser"))
    .AddPolicy("Nurses", policy => policy.RequireRole("Nurse"))
    .AddPolicy("Physicians", policy => policy.RequireRole("Physician"))
    .AddPolicy("Lab", policy => policy.RequireRole("Lab"))
    .AddPolicy("Records", policy => policy.RequireRole("Records"))
    .AddPolicy("Wards", policy => policy.RequireRole("Ward"))
    .AddPolicy("Services", policy => policy.RequireRole("Services"))
    .AddPolicy("Requesters", policy => policy.RequireRole("Requester"))
    .AddPolicy("Accounts", policy => policy.RequireRole("Accounts"))
    .AddPolicy("Pharmacy", policy => policy.RequireRole("Pharmacy"))
    .AddPolicy("Dispensers", policy => policy.RequireRole("Dispenser"))
    .AddPolicy("CanAdmit", policy => policy.RequireClaim("CanAdmit").RequireRole("Physician"))
    .AddPolicy("CanDischarge", policy => policy.RequireClaim("CanDischarge").RequireRole("Physician"))
    .AddDefaultPolicy("Default", x => x.RequireAuthenticatedUser());

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration.GetConnectionString("Valkey")!);

builder.Services.AddDataProtection();
builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(typeof(CancellationToken), sp =>
{
    var ctx = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    return ctx?.RequestAborted ?? CancellationToken.None;
});

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

//builder.Services.AddAuthentication(options =>
//    {
//        options.DefaultScheme = IdentityConstants.ApplicationScheme;
//        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//    })
//    .AddIdentityCookies();

//builder.Services.AddIdentityCore<ApplicationUser>(options =>
//    {
//        options.SignIn.RequireConfirmedAccount = true;
//        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
//    })
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddSignInManager()
//    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SHIMS.Client._Imports).Assembly);

app.MapAdditionalIdentityEndpoints();

app.Run();

using AspNetCoreRateLimit;
using Blazored.LocalStorage;
using GoogleAnalytics.Blazor;
using Material.Blazor;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Events;
using System.IO.Compression;
using Website.Lib;

const string _customTemplate = "{Timestamp: HH:mm:ss.fff}\t[{Level:u3}]\t{Message}{NewLine}{Exception}";
const string _loggingWebhook = "https://blacklandcapital.webhook.office.com/webhookb2/6ccfaed1-7c02-440c-83f0-9265cf35b379@ef73a184-f1db-4f24-b406-e4f8f9633dfa/IncomingWebhook/18bed2df0852449aa5d92541255caade/34ba3a07-c6f6-4e3f-896d-148fb6c1765f";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

builder.Services.AddMBServices(loggingServiceConfiguration: Utilities.GetDefaultLoggingServiceConfiguration(), toastServiceConfiguration: Utilities.GetDefaultToastServiceConfiguration(), snackbarServiceConfiguration: Utilities.GetDefaultSnackbarServiceConfiguration());

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddHttpClient();

builder.Services.AddTransient<INotificationService, NotificationService>();

builder.Services.AddScoped<ContentSecurityPolicyService>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // Has Pentest fixes
    options.CheckConsentNeeded = context => true;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.Configure<StaticFileOptions>(options =>
{
    // Pentest fix
    options.OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Add("Cache-Control", "public, max-age=86400");
        ctx.Context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    };
});

builder.Services.AddOptions();
// needed to store rate limit counters and ip rules
builder.Services.AddMemoryCache();

//load general configuration from appsettings.json
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));

//load client rules from appsettings.json
builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));

builder.Services.AddInMemoryRateLimiting();

// configuration (resolvers, counter key builders)
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddGBService(
    trackingId: "G-V061TDSPDR",
    globalEventParams: new Dictionary<string, object>()
    {
        { Utilities.EventCategory, Utilities.DialogActions },
        { Utilities.NonInteraction, true },
    });

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    .MinimumLevel.Information()
#endif
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("GoogleAnalytics.Blazor", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Conditional(evt => builder.Environment.IsDevelopment(), wt => wt.Async(a => a.Console(outputTemplate: _customTemplate, restrictedToMinimumLevel: LogEventLevel.Debug)))
    .WriteTo.Conditional(evt => !builder.Environment.IsDevelopment(), wt => wt.Async(a => a.Console(outputTemplate: _customTemplate, restrictedToMinimumLevel: LogEventLevel.Information)))
    .WriteTo.Conditional(evt => !app.Environment.IsDevelopment(), wt => wt.Async(a => a.MicrosoftTeams(outputTemplate: _customTemplate, webHookUri: _loggingWebhook, titleTemplate: "Dioptra Website", restrictedToMinimumLevel: LogEventLevel.Warning)))
    .WriteTo.Conditional(evt => app.Environment.IsDevelopment(), wt => wt.Async(a => a.File(outputTemplate: _customTemplate, path: Environment.GetEnvironmentVariable("LOCALAPPDATA") + "\\Dioptra Website\\blazor-server-app.log", restrictedToMinimumLevel: LogEventLevel.Debug, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)))
    .CreateLogger();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseResponseCompression();

app.UseCookiePolicy();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();

app.UseStaticFiles();

// Pentest fix
app.UseContentSecurityPolicy();

// Pentest fix
app.UseNoCacheMiddleware();

app.UseRouting();

app.UseClientRateLimiting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("/_Host");

app.MapGet("/sitemap.xml", async context => {
    await Sitemap.Generate(context);
});

app.Run();

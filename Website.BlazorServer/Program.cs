using AspNetCoreRateLimit;
using Blazored.LocalStorage;
using CompressedStaticFiles;
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

#region Potentially omit to avoid CRIME and BREACH attacks
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// Performance test (performed in debug mode locally):
// NoCompression - material.blazor.min.css takes circa 10 to 20 ms to download, 270 Kb - page load 95 to 210 ms - 3.2 MB transfered
// Fastest - material.blazor.min.css takes circa 12 to 28 ms to download, 34.7 Kb - page load 250 to 270 ms - 2.2 MB transfered
// SmallestSize & Optimal - material.blazor.min.css takes circa 500 to 800 ms to download, 16.2 Kb - page load 900 to 1100 ms (unacceptably slow) - 2.1 MB transfered
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});
#endregion

builder.Services.AddResponseCaching();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
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

// Pentest fix
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

builder.Services.AddCompressedStaticFiles();

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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Potentially omit to avoid CRIME and BREACH attacks - https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-6.0#compression-with-https
app.UseResponseCompression();

app.UseCookiePolicy(); 

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCompressedStaticFiles();

// Pentest fix
// Pentest fix
app.UseContentSecurityPolicy();

// Pentest fix
app.UseNoCacheMiddleware();

app.UseRouting();

app.UseClientRateLimiting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapGet("/sitemap.xml", async context => {
    await Sitemap.Generate(context);
});

app.Run();

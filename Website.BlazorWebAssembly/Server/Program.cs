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

builder.Services.AddScoped<NonceService>();

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

builder.Services.AddGBService("G-V061TDSPDR");

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Async(a => a.Console(outputTemplate: _customTemplate, restrictedToMinimumLevel: LogEventLevel.Information))
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
app.Use(async (context, next) =>
{
    var nonceValue = context.RequestServices.GetService<NonceService>()?.NonceValue ?? throw new Exception("Nonce service unavailable");

    var source = (app.Environment.IsDevelopment() ? "'self' " : "") + $"'nonce-{nonceValue}'";

    var baseUri = context.Request.Host.ToString();
    var baseDomain = context.Request.Host.Host;

    var csp =
        "base-uri 'self'; " +
        "block-all-mixed-content; " +
        "child-src 'self' ; " +
        $"connect-src 'self' wss://{baseDomain}:* www.google-analytics.com region1.google-analytics.com; " +
        "default-src 'self'; " +
        "font-src use.typekit.net fonts.gstatic.com; " +
        "frame-ancestors 'none'; " +
        "frame-src 'self'; " +
        "form-action 'none'; " +
        "img-src 'self' www.google-analytics.com *.openstreetmap.org data: w3.org/svg/2000; " +
        "manifest-src 'self'; " +
        "media-src 'self'; " +
        "prefetch-src 'self'; " +
        "object-src  data: 'unsafe-eval'; " +
        $"report-to https://{baseUri}/api/CspReporting/UriReport; " +
        $"report-uri https://{baseUri}/api/CspReporting/UriReport; " +
        $"script-src {source} 'self' 'sha256-3b0LA1ZE3o1c1aNFfpkF0fkCBHXmfVFpWjGIve/v2XQ=' 'strict-dynamic' 'report-sample' 'unsafe-eval';" +
        "style-src 'self' 'unsafe-inline' 'report-sample' p.typekit.net use.typekit.net fonts.gstatic.com; " +
        "upgrade-insecure-requests; " +
        "worker-src 'self';";

    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
    context.Response.Headers.Add("X-ClientId", "dioptra");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
    context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");

#if !DEBUG
    context.Response.Headers.Add("Content-Security-Policy", csp);
#endif

    await next();
});

// Pentest fix
app.UseMiddleware<NoCacheMiddleware>();

app.UseRouting();

app.UseClientRateLimiting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("/_Host");

app.MapGet("/sitemap.xml", async context => {
    await Sitemap.Generate(context);
});

app.Run();

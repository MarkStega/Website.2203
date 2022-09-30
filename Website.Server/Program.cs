using Blazored.LocalStorage;
using CompressedStaticFiles;
using GoogleAnalytics.Blazor;
using HttpSecurity.AspNet;
using Material.Blazor;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Events;
using System.IO.Compression;
using System.Threading.RateLimiting;
using Website.Lib;
using Website.Server;

const string _customTemplate = "{Timestamp: HH:mm:ss.fff}\t[{Level:u3}]\t{Message}{NewLine}{Exception}";
string _loggingWebhook = Environment.GetEnvironmentVariable("LOGGING_WEBHOOK") ?? "https://nonexistent.nothing";

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

builder.Services.AddHttpsSecurityHeaders(options =>
{
    options

    // Content Security Policies
    .AddBaseUriCSP(o => o.AddSelf())
    .AddBlockAllMixedContentCSP()
    .AddChildSrcCSP(o => o.AddSelf())
    .AddConnectSrcCSP(o => o.AddSelf().AddUri((baseUri, baseDomain) => $"wss://{baseDomain}:*").AddUri("https://www.googletagmanager.com").AddUri("https://www.google-analytics.com").AddUri("https://region1.google-analytics.com").AddUri("https://p.typekit.net").AddUri("https://use.typekit.net").AddUri("https://fonts.googleapis.com").AddUri("https://fonts.gstatic.com"))
    .AddDefaultSrcCSP(o => o.AddSelf())
    .AddFontSrcCSP(o => o.AddUri("https://use.typekit.net").AddUri("https://fonts.googleapis.com").AddUri("https://fonts.gstatic.com"))
    .AddFrameAncestorsCSP(o => o.AddNone())
    .AddFrameSrcCSP(o => o.AddSelf())
    .AddFormActionCSP(o => o.AddNone())
    .AddImgSrcCSP(o => o.AddSelf().AddUri("https://www.google-analytics.com").AddUri("https://*.openstreetmap.org").AddSchemeSource(SchemeSource.Data, "w3.org/svg/2000"))
    .AddManifestSrcCSP(o => o.AddSelf())
    .AddMediaSrcCSP(o => o.AddSelf())
    .AddPrefetchSrcCSP(o => o.AddSelf())
    .AddObjectSrcCSP(o => o.AddNone())
    .AddReportUriCSP(o => o.AddUri((baseUri, baseDomain) => $"https://{baseUri}/api/CspReporting/UriReport"))
    // The sha-256 hash relates to an inline script added by blazor's javascript
    .AddScriptSrcCSP(o => 
            o.AddHashValue(HashAlgorithm.SHA256, "v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA=")
            .AddUriIf((baseUri, baseDomain) => $"https://{baseUri}/_framework/aspnetcore-browser-refresh.js", () => builder.Environment.IsDevelopment())
            .AddSelfIf(() => builder.Environment.IsDevelopment() || PlatformDetermination.IsBlazorWebAssembly)
            .AddStrictDynamicIf(() => !builder.Environment.IsDevelopment() && PlatformDetermination.IsBlazorWebAssembly)
            .AddUnsafeInlineIf(() => PlatformDetermination.IsBlazorWebAssembly)
            .AddReportSample()
            .AddUnsafeEvalIf(() => PlatformDetermination.IsBlazorWebAssembly)
            .AddUri("https://www.googletagmanager.com/gtag/js")
            .AddGeneratedHashValues(StaticFileExtension.JS))
    .AddStyleSrcCSP(o => o.AddSelf().AddUnsafeInline().AddReportSample().AddUri("https://p.typekit.net").AddUri("https://use.typekit.net").AddUri("https://fonts.googleapis.com").AddUri("https://fonts.gstatic.com"))
    .AddUpgradeInsecureRequestsCSP()
    .AddWorkerSrcCSP(o => o.AddSelf())

    // Other headers
    .AddAccessControlAllowOriginAll()
    // ref: <a href="http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers">http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers</a>
    .AddCacheControl("no-cache, public, max-age=86400")
    .AddExpires("0")
    .AddReferrerPolicy(ReferrerPolicyDirective.NoReferrer)
    .AddPermissionsPolicy("accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()")
    .AddStrictTransportSecurity(31536000, true)
    .AddXClientId("Dioptra")
    .AddXContentTypeOptionsNoSniff()
    .AddXFrameOptionsDirective(XFrameOptionsDirective.Deny)
    .AddXXssProtectionDirective(XXssProtectionDirective.OneModeBlock)
    .AddXPermittedCrossDomainPoliciesDirective(XPermittedCrossDomainPoliciesDirective.None);
});


builder.Services.AddResponseCaching();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorPages();

#if BLAZOR_SERVER

builder.Services.AddServerSideBlazor();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

#endif

// Needed for prerendering on WebAssembly as well as general use
builder.Services.AddTransient<INotification, ServerNotificationService>();

builder.Services.AddMBServices();

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // Has Pentest fixes
    options.CheckConsentNeeded = context => true;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddOptions();
// needed to store rate limit counters and ip rules
builder.Services.AddMemoryCache();

builder.Services.AddHttpContextAccessor();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddGBService(options =>
{
    options.TrackingId = "G-V061TDSPDR";
    options.GlobalEventParams = new Dictionary<string, object>()
    {
        { Utilities.EventCategory, Utilities.DialogActions },
        { Utilities.NonInteraction, true },
    };
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

app.UseHttpSecurityHeaders();

app.UseCompressedStaticFiles();

app.UseRouting();

// Limit api calls to 10 in a second to prevent external denial of service.
app.UseRateLimiter(new()
{
    GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            return RateLimitPartition.GetNoLimiter("NoLimit");
        }

        return RateLimitPartition.GetFixedWindowLimiter("GeneralLimit",
            _ => new FixedWindowRateLimiterOptions()
            {
                Window = TimeSpan.FromSeconds(1),
                PermitLimit = 1,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10,
            });
    }),
    RejectionStatusCode = 429,
});

app.MapControllers();

#if BLAZOR_SERVER
app.MapBlazorHub();
#else
app.UseBlazorFrameworkFiles();
#endif

app.MapFallbackToPage("/Host");

app.MapGet("/sitemap.xml", async context =>
{
    await Sitemap.Generate(context);
});

app.Run();

using System.IO.Compression;
using System.Threading.RateLimiting;

using Blazored.LocalStorage;

using CompressedStaticFiles.AspNet;

using GoogleAnalytics.Blazor;

using HttpSecurity.AspNet;

using Material.Blazor;

using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.ResponseCompression;

using Serilog;
using Serilog.Events;

using Website.Client;
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

builder.Services.AddHttpsSecurityHeaders(options => OptionsBuilder.BuildGeneralHeaderOptions(builder, options), onStartupOptions => OptionsBuilder.BuildOnStartupHeaderOptions(builder, onStartupOptions));

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
if (app.Environment.IsDevelopment())
{
#if BLAZOR_SERVER
        app.UseDeveloperExceptionPage();
#else
    app.UseWebAssemblyDebugging();
#endif
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
}

// Potentially omit to avoid CRIME and BREACH attacks - https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-6.0#compression-with-https
app.UseResponseCompression();

app.UseCookiePolicy();

app.UseSerilogRequestLogging();

//app.UseHttpsRedirection();

app.UseHttpSecurityHeaders();

app.UseCompressedStaticFiles();

app.UseRouting();

// Limit api calls to 10 in a second to prevent external denial of service.
app.UseRateLimiter(new()
{
    GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        // I thnik this is a mistake - The API should be rate limited - ms
        //
        //if (!context.Request.Path.StartsWithSegments("/api"))
        //{
        //    return RateLimitPartition.GetNoLimiter("NoLimit");
        //}

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

await app.RunAsync();

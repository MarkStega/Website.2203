using Blazored.LocalStorage;
using GoogleAnalytics.Blazor;
using Material.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Website.Lib;
using Website.BlazorWebAssembly.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Events;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMBServices(loggingServiceConfiguration: Utilities.GetDefaultLoggingServiceConfiguration(), toastServiceConfiguration: Utilities.GetDefaultToastServiceConfiguration(), snackbarServiceConfiguration: Utilities.GetDefaultSnackbarServiceConfiguration());

builder.Services.AddTransient<INotificationService, NotificationService>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // Has Pentest fixes
    options.CheckConsentNeeded = context => true;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddBlazoredLocalStorage();

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    .MinimumLevel.Information()
#endif
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("GoogleAnalytics.Blazor", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Async(a => a.BrowserConsole(outputTemplate: "{Timestamp:HH:mm:ss.fff}\t[{Level:u3}]\t{Message}{NewLine}{Exception}"))
    .CreateLogger();

builder.Logging.AddProvider(new SerilogLoggerProvider());

builder.Services.AddGBService(
    trackingId: "G-V061TDSPDR",
    globalEventParams: new Dictionary<string, object>()
    {
        { Utilities.EventCategory, Utilities.DialogActions },
        { Utilities.NonInteraction, true },
    });

await builder.Build().RunAsync();

using Blazored.LocalStorage;
using Material.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Website.Lib;
using Website.BlazorWebAssembly.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMBServices(loggingServiceConfiguration: Utilities.GetDefaultLoggingServiceConfiguration(), toastServiceConfiguration: Utilities.GetDefaultToastServiceConfiguration(), snackbarServiceConfiguration: Utilities.GetDefaultSnackbarServiceConfiguration());

builder.Services.AddTransient<ITeamsNotificationService, TeamsNotificationService>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // Has Pentest fixes
    options.CheckConsentNeeded = context => true;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();

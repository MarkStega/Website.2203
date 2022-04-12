using Material.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Website.Lib;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMBServices(loggingServiceConfiguration: Utilities.GetDefaultLoggingServiceConfiguration(), toastServiceConfiguration: Utilities.GetDefaultToastServiceConfiguration(), snackbarServiceConfiguration: Utilities.GetDefaultSnackbarServiceConfiguration());

builder.Services.AddHttpClient();
builder.Services.AddTransient<ITeamsNotificationService, TeamsNotificationService>();

await builder.Build().RunAsync();

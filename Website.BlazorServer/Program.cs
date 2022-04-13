using Material.Blazor;
using Serilog;
using Serilog.Events;
using Website.Lib;

const string _customTemplate = "{Timestamp: HH:mm:ss.fff}\t[{Level:u3}]\t{Message}{NewLine}{Exception}";
const string _loggingWebhook = "https://blacklandcapital.webhook.office.com/webhookb2/6ccfaed1-7c02-440c-83f0-9265cf35b379@ef73a184-f1db-4f24-b406-e4f8f9633dfa/IncomingWebhook/18bed2df0852449aa5d92541255caade/34ba3a07-c6f6-4e3f-896d-148fb6c1765f";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddMBServices(loggingServiceConfiguration: Utilities.GetDefaultLoggingServiceConfiguration(), toastServiceConfiguration: Utilities.GetDefaultToastServiceConfiguration(), snackbarServiceConfiguration: Utilities.GetDefaultSnackbarServiceConfiguration());

builder.Services.AddHttpClient();
builder.Services.AddTransient<ITeamsNotificationService, TeamsNotificationService>();

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Async(a => a.Console(outputTemplate: _customTemplate, restrictedToMinimumLevel: LogEventLevel.Information))
    .WriteTo.Conditional(evt => app.Environment.IsDevelopment(), wt => wt.Async(a => a.MicrosoftTeams(outputTemplate: _customTemplate, webHookUri: _loggingWebhook, titleTemplate: "Dioptra Website", restrictedToMinimumLevel: LogEventLevel.Warning)))
    .WriteTo.Conditional(evt => app.Environment.IsDevelopment(), wt => wt.Async(a => a.File(outputTemplate: _customTemplate, path: Environment.GetEnvironmentVariable("LOCALAPPDATA") + "\\Dioptra Website\\blazor-server-app.log", restrictedToMinimumLevel: LogEventLevel.Debug, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)))
    .CreateLogger();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

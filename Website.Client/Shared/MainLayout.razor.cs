using GoogleAnalytics.Blazor;
using Material.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Website.Client;

/// <summary>
/// The standard Blazor MainLayout component.
/// </summary>
public partial class MainLayout : LayoutComponentBase
{
    [Inject] private INotification Notifier { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private IGBAnalyticsManager AnalyticsManager { get; set; } = default!;


    private MBDialog ContactDialog { get; set; } = new();
    private bool HomeButtonExited { get; set; } = true;
    private ContactMessage ContactMessage { get; set; } = new();



    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("Website.General.instantiateErrorDialog");
        }
    }

    private async Task OpenContactDialogAsync()
    {
        ContactMessage = new();

        await AnalyticsManager.TrackEvent("Open Contact Dialog");

        await ContactDialog.ShowAsync();
    }


    private async Task CloseContactDialogAsync()
    {
        await AnalyticsManager.TrackEvent("Close Contact Dialog");

        await ContactDialog.HideAsync();
    }


    private async Task ContactDialogSubmittedAsync()
    {
        await CloseContactDialogAsync();
        await Notifier.Send(ContactMessage);
    }


    private async Task HomeClick()
    {
        NavigationManager.NavigateTo("/");
        await JSRuntime.InvokeVoidAsync("Website.General.scrollToTop").ConfigureAwait(false);
        HomeButtonExited = true;
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }


    public void ShowHomeButton(bool show)
    {
        HomeButtonExited = !show;
        _ = InvokeAsync(StateHasChanged);
    }
}

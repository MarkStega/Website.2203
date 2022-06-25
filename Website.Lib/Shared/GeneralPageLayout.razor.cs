using GoogleAnalytics.Blazor;
using Material.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Website.Lib.Shared;

public partial class GeneralPageLayout : ComponentBase
{
    [Inject] private INotificationService TeamsNotificationService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private IGBAnalyticsManager AnalyticsManager { get; set; }



    [Parameter] public string ColorClass { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }


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
        await TeamsNotificationService.SendNotification(ContactMessage);
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

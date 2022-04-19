namespace Website.Lib.Shared;

using Material.Blazor;
using Microsoft.AspNetCore.Components;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] private ITeamsNotificationService TeamsNotificationService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    private MBDialog ContactDialog { get; set; } = new();
    private MBFloatingActionButton HomeButton { get; set; }
    private bool HomeButtonExited { get; set; } = true;
    private ContactMessage ContactMessage { get; set; } = new();


    private async Task OpenContactDialogAsync()
    {
        ContactMessage = new();

        await ContactDialog.ShowAsync();
    }

    private async Task CloseContactDialogAsync()
    {
        await ContactDialog.HideAsync();
    }

    private async Task ContactDialogSubmittedAsync()
    {
        await ContactDialog.HideAsync();
        await TeamsNotificationService.SendNotification(ContactMessage);
    }

    private void HomeClick()
    {
        NavigationManager.NavigateTo("/");
        HomeButtonExited = true;
        _ = InvokeAsync(StateHasChanged);
    }

    private void ShowHomeButton(bool show)
    {
        _ = Task.Run(async () =>
        {
            if (show)
            {
                await Task.Delay(500);
            }

            HomeButtonExited = !show;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        });
    }


}


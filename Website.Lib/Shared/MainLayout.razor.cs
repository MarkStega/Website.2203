namespace Website.Lib.Shared;

using Material.Blazor;
using Microsoft.AspNetCore.Components;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] private ITeamsNotificationService TeamsNotificationService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    private MBMenu Menu { get; set; } = new();
    private MBDialog ContactDialog { get; set; } = new();
    private MBFloatingActionButton HomeButton { get; set; }
    private bool HomeButtonExited { get; set; } = true;
    private ContactData Contact { get; set; } = new();


    private async Task OpenContactDialogAsync()
    {
        Contact = new();

        await ContactDialog.ShowAsync();
    }

    private async Task CloseContactDialogAsync()
    {
        await ContactDialog.HideAsync();
    }

    private async Task ContactDialogSubmittedAsync()
    {
        await ContactDialog.HideAsync();
        await TeamsNotificationService.SendNotification(Contact);
    }

    private void HomeClick()
    {
        NavigationManager.NavigateTo("/");
        HomeButtonExited = true;
        _ = InvokeAsync(StateHasChanged);
    }

    private void ShowHomeButton()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(500);
            HomeButtonExited = false;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        });
    }


}


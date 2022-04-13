namespace Website.Lib.Shared;

using Material.Blazor;
using Microsoft.AspNetCore.Components;

public partial class MainLayout
{
    [Inject] private ITeamsNotificationService TeamsNotificationService { get; set; }

    private MBMenu Menu { get; set; } = new();
    private MBDialog ContactDialog { get; set; } = new();
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

}


using Material.Blazor;
using Microsoft.AspNetCore.Components;

namespace Website.Lib.Pages;
public partial class RealEstateClients : ComponentBase
{
    [Inject] private ITeamsNotificationService TeamsNotificationService { get; set; }
    [CascadingParameter] private Action<bool> ShowHomeButton { get; set; }

    private MBDialog Dialog { get; set; } = new();
    private RealEstateInvestorEnquiry RealEstateInvestorEnquiry { get; set; } = new();
    private string HiringDialogTitle { get; set; } = "";
    private bool ShowProspectiveRole { get; set; } = false;


    protected override void OnInitialized()
    {
        base.OnInitialized();

        ShowHomeButton(true);
    }


    private async Task OpenDialogAsync()
    {
        RealEstateInvestorEnquiry = new();

        await Dialog.ShowAsync();
    }

    private async Task CloseDialogAsync()
    {
        await Dialog.HideAsync();
    }

    private async Task DialogSubmittedAsync()
    {
        await Dialog.HideAsync();
        await TeamsNotificationService.SendNotification(RealEstateInvestorEnquiry);
    }

}

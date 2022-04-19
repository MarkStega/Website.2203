using Material.Blazor;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace Website.Lib.Pages;
public partial class VentureCapitalInvestors : ComponentBase
{
    [Inject] private ITeamsNotificationService TeamsNotificationService { get; set; }
    [CascadingParameter] private Action<bool> ShowHomeButton { get; set; }

    private MBDialog HiringDialog { get; set; } = new();
    private HiringEnquiry HiringEnquiry { get; set; } = new();
    private List<MBSelectElement<HiringEnquiry.RoleType>> SelectElements { get; set; }
    private string HiringDialogTitle { get; set; } = "";
    private bool ShowProspectiveRole { get; set; } = false;


    protected override void OnInitialized()
    {
        base.OnInitialized();

        ShowHomeButton(true);

        SelectElements = Enum.GetValues<HiringEnquiry.RoleType>().Select(x => new MBSelectElement<HiringEnquiry.RoleType>() { SelectedValue = x, Label = x.ToString() }).ToList();
    }


    private async Task OpenHiringDialogAsync(HiringEnquiry.RoleType? prospectiveRole = null)
    {
        HiringEnquiry = new();

        if (prospectiveRole == HiringEnquiry.RoleType.Analyst)
        {
            HiringEnquiry.ProspectiveRole = HiringEnquiry.RoleType.Analyst;
            HiringDialogTitle = "Analyst Role Enquiry";
            ShowProspectiveRole = false;
        }
        else if (prospectiveRole == HiringEnquiry.RoleType.Technologist)
        {
            HiringEnquiry.ProspectiveRole = HiringEnquiry.RoleType.Technologist;
            HiringDialogTitle = "Technologist Role Enquiry";
            ShowProspectiveRole = false;
        }
        else if (prospectiveRole == null)
        {
            HiringEnquiry.ProspectiveRole = HiringEnquiry.RoleType.Analyst;
            HiringDialogTitle = "Recuritment Enquiry";
            ShowProspectiveRole = true;
        }

        await HiringDialog.ShowAsync();
    }

    private async Task CloseHiringDialogAsync()
    {
        await HiringDialog.HideAsync();
    }

    private async Task HiringDialogSubmittedAsync()
    {
        await HiringDialog.HideAsync();
        await TeamsNotificationService.SendNotification(HiringEnquiry);
    }

}

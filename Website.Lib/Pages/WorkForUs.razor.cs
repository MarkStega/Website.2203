using Material.Blazor;
using Microsoft.AspNetCore.Components;
using Website.Lib.Shared;

namespace Website.Lib.Pages;
[Sitemap(SitemapAttribute.ChangeFreqType.Weekly, 0.8)]
public partial class WorkForUs : ComponentBase
{
    [Inject] private ITeamsNotificationService TeamsNotificationService { get; set; }


    private GeneralPageLayout GeneralPageLayout { get; set; }
    private MBDialog RecruitmentEnquiryDialog { get; set; } = new();
    private RecruitmentEnquiry RecruitmentEnquiry { get; set; } = new();
    private List<MBSelectElement<RecruitmentEnquiry.RoleType>> SelectElements { get; set; }
    private string HiringDialogTitle { get; set; } = "";
    private bool ShowProspectiveRole { get; set; } = false;


    protected override void OnInitialized()
    {
        base.OnInitialized();

        SelectElements = Enum.GetValues<RecruitmentEnquiry.RoleType>().Select(x => new MBSelectElement<RecruitmentEnquiry.RoleType>() { SelectedValue = x, Label = x.ToString() }).ToList();
    }


    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            GeneralPageLayout.ShowHomeButton(true);
        }
    }


    private async Task OpenDialogAsync(RecruitmentEnquiry.RoleType? prospectiveRole = null)
    {
        RecruitmentEnquiry = new();

        if (prospectiveRole == RecruitmentEnquiry.RoleType.Analyst)
        {
            RecruitmentEnquiry.ProspectiveRole = RecruitmentEnquiry.RoleType.Analyst;
            HiringDialogTitle = "Analyst Role Enquiry";
            ShowProspectiveRole = false;
        }
        else if (prospectiveRole == RecruitmentEnquiry.RoleType.Technologist)
        {
            RecruitmentEnquiry.ProspectiveRole = RecruitmentEnquiry.RoleType.Technologist;
            HiringDialogTitle = "Technologist Role Enquiry";
            ShowProspectiveRole = false;
        }
        else if (prospectiveRole == null)
        {
            RecruitmentEnquiry.ProspectiveRole = RecruitmentEnquiry.RoleType.Analyst;
            HiringDialogTitle = "Recuritment Enquiry";
            ShowProspectiveRole = true;
        }

        await RecruitmentEnquiryDialog.ShowAsync();
    }

    private async Task ClosegDialogAsync()
    {
        await RecruitmentEnquiryDialog.HideAsync();
    }

    private async Task DialogSubmittedAsync()
    {
        await RecruitmentEnquiryDialog.HideAsync();
        await TeamsNotificationService.SendNotification(RecruitmentEnquiry);
    }

}

using GoogleAnalytics.Blazor;
using Material.Blazor;
using Microsoft.AspNetCore.Components;

namespace Website.Lib;

/// <summary>
/// The "Work for us" website page.
/// </summary>
[Sitemap(SitemapAttribute.ChangeFreqType.Weekly, 0.8)]
public partial class WorkForUs : ComponentBase
{
    [CascadingParameter] private MainLayout MainLayout { get; set; } = default!;


    [Inject] private INotification Notifier { get; set; } = default!;
    [Inject] private IGBAnalyticsManager AnalyticsManager { get; set; } = default!;


    private MBDialog RecruitmentEnquiryDialog { get; set; } = new();
    private RecruitmentEnquiry RecruitmentEnquiry { get; set; } = new();
    private List<MBSelectElement<RecruitmentEnquiry.RoleType>> SelectElements { get; set; } = default!;
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
            MainLayout.ShowHomeButton(true);
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

        await AnalyticsManager.TrackEvent("Open Recruitment Dialog");

        await RecruitmentEnquiryDialog.ShowAsync();
    }

    private async Task ClosegDialogAsync()
    {
        await AnalyticsManager.TrackEvent("Close Recruitment Dialog");
        
        await RecruitmentEnquiryDialog.HideAsync();
    }

    private async Task DialogSubmittedAsync()
    {
        await ClosegDialogAsync();
        await Notifier.Send(RecruitmentEnquiry);
    }

}

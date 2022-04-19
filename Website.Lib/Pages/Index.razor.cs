using Microsoft.AspNetCore.Components;
using Website.Lib.Shared;

namespace Website.Lib.Pages;
public partial class Index : ComponentBase
{
    private class CarouselDataType
    {
        public string Uri { get; set; } = "";
        public string Caption { get; set; } = "";
    }


    [Inject] private NavigationManager NavigationManager { get; set; }

    [CascadingParameter] private Action<bool> ShowHomeButton { get; set; }


    private static readonly CarouselDataType[] CarouselData = new CarouselDataType[]
    {
        new() { Uri = "_content/Website.Lib/images/01-main-screen.webp", Caption = "Dioptra's main screen layout" },
        new() { Uri = "_content/Website.Lib/images/02-main-screen-search.webp", Caption = "Searching for a scheme that exibits high risk" },
        new() { Uri = "_content/Website.Lib/images/03-march-costs-chart.webp", Caption = "Budget, actual and forecast development costs" },
        new() { Uri = "_content/Website.Lib/images/04-march-accruals.webp", Caption = "Loan interest accruals" },
        new() { Uri = "_content/Website.Lib/images/05-march-cost-summary.webp", Caption = "Summary of project costs" },
        new() { Uri = "_content/Website.Lib/images/06-march-capital-flows.webp", Caption = "Chart of actual and forecast capital structure" },
        new() { Uri = "_content/Website.Lib/images/07-march-land-reg.webp", Caption = "UK Land Registry query of sold unit prices" },
        new() { Uri = "_content/Website.Lib/images/08-march-version-graph.webp", Caption = "Scheme data versioning" },
        new() { Uri = "_content/Website.Lib/images/09-march-edit-budget-schedule.webp", Caption = "Editting a suite of cost budget schedules" },
    };


    protected override void OnInitialized()
    {
        base.OnInitialized();

        ShowHomeButton(false);
    }


    private void RealEstateClientsClick()
    {
        NavigationManager.NavigateTo("/real-estate-clients");
    }


    private void VentureCapitalInvestorsClick()
    {
        NavigationManager.NavigateTo("/venture-capital-investors");
    }


    private void WorkForUsClick()
    {
        NavigationManager.NavigateTo("/work-for-us");
    }
}


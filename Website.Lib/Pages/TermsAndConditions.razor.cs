using Microsoft.AspNetCore.Components;
using Website.Lib.Shared;

namespace Website.Lib.Pages;
[Sitemap(SitemapAttribute.ChangeFreqType.Monthly, 0.1)]
public partial class TermsAndConditions : ComponentBase
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private GeneralPageLayout GeneralPageLayout { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            GeneralPageLayout.ShowHomeButton(true);
        }
    }
}

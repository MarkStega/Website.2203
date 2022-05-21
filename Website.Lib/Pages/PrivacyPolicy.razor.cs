using Microsoft.AspNetCore.Components;
using Website.Lib.Shared;

namespace Website.Lib.Pages;

[Sitemap(SitemapAttribute.ChangeFreqType.Monthly, 0.1)]
public partial class PrivacyPolicy : ComponentBase
{
    private GeneralPageLayout GeneralPageLayout { get; set; }


    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            GeneralPageLayout.ShowHomeButton(true);
        }
    }
}

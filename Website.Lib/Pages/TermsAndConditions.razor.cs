using Microsoft.AspNetCore.Components;

namespace Website.Lib;

/// <summary>
/// Terms and conditions page.
/// </summary>
[Sitemap(SitemapAttribute.ChangeFreqType.Monthly, 0.1)]
public partial class TermsAndConditions : ComponentBase
{
    [CascadingParameter] private MainLayout MainLayout { get; set; } = default!;


    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            MainLayout.ShowHomeButton(true);
        }
    }
}

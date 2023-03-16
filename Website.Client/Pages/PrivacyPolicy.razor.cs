using Microsoft.AspNetCore.Components;

namespace Website.Client;

/// <summary>
/// The privacy policy page.
/// </summary>
[Sitemap(SitemapAttribute.ChangeFreqType.Monthly, 0.1)]
public partial class PrivacyPolicy : ComponentBase
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

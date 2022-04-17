using Material.Blazor;
using Microsoft.AspNetCore.Components;

namespace Website.Lib.Pages;
public partial class WorkForUs : ComponentBase
{
    [CascadingParameter] private Action<bool> ShowHomeButton { get; set; }


    protected override void OnInitialized()
    {
        base.OnInitialized();

        ShowHomeButton(true);
    }
}

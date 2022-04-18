using Microsoft.AspNetCore.Components;
using Website.Lib.Shared;

namespace Website.Lib.Pages;
public partial class Index : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; }

    [CascadingParameter] private Action<bool> ShowHomeButton { get; set; }


    protected override void OnInitialized()
    {
        base.OnInitialized();

        ShowHomeButton(false);

        /*CarouselData = new()
        {
            new() { Caption = "Panel 1" },
            new() { Caption = "Panel 2" },
            new() { Caption = "Panel 3" },
            new() { Caption = "Panel 4" },
            new() { Caption = "Panel 5" },
        };*/
    }


    private void WorkForUsClick()
    {
        NavigationManager.NavigateTo("/work-for-us");
    }
}


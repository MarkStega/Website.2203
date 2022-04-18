using Microsoft.AspNetCore.Components;
using Website.Lib.Shared;

namespace Website.Lib.Pages;
public partial class Index : ComponentBase
{
    private class CarouselDataType
    {
        public string HoldingText { get; set; } = "";
        public string Caption { get; set; } = "";
    }


    [Inject] private NavigationManager NavigationManager { get; set; }

    [CascadingParameter] private Action<bool> ShowHomeButton { get; set; }


    private static readonly CarouselDataType[] CarouselData = new CarouselDataType[]
    {
        new() { HoldingText = "Panel 1", Caption = "Caption 1" },
        new() { HoldingText = "Panel 2", Caption = "Caption 2" },
        new() { HoldingText = "Panel 3", Caption = "Caption 3" },
        new() { HoldingText = "Panel 4", Caption = "Caption 4" },
        new() { HoldingText = "Panel 5", Caption = "Caption 5" },
    };


    protected override void OnInitialized()
    {
        base.OnInitialized();

        ShowHomeButton(false);
    }


    private void WorkForUsClick()
    {
        NavigationManager.NavigateTo("/work-for-us");
    }
}


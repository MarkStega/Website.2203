using Microsoft.AspNetCore.Components;

namespace Website.Lib.Pages;
public partial class Index
{
    [Inject] private NavigationManager NavigationManager { get; set; }

    [CascadingParameter] private Action ShowHomeButton { get; set; }


    private void WorkForUsClick()
    {
        NavigationManager.NavigateTo("/work-for-us");

        ShowHomeButton();
    }
}


using Microsoft.AspNetCore.Components;

namespace Website.Lib.Pages;
public partial class Index : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; }

    [CascadingParameter] private Action<bool> ShowHomeButton { get; set; }



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


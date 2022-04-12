using Material.Blazor;

namespace Website.Lib.Shared;

public partial class MainLayout
{
    private MBMenu Menu { get; set; } = new();
    private MBDialog ContactDialog { get; set; } = new();
    private ContactData Contact { get; set; } = new();

    private void SideBarToggle()
    {

    }

    private void OpenMenuAsync()
    {

    }

    private async Task OpenContactDialogAsync()
    {
        Contact = new();

        await ContactDialog.ShowAsync();
    }

    private async Task CloseContactDialogAsync()
    {
        await ContactDialog.HideAsync();
    }

    private async Task ContactDialogSubmittedAsync()
    {
        await ContactDialog.HideAsync();
        Console.WriteLine(Contact.ToString());
    }

}


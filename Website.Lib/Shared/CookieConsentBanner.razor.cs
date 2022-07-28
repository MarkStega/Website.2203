using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
namespace Website.Lib.Shared;
public partial class CookieConsentBanner : ComponentBase
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] private ILocalStorageService LocalStorage { get; set; }


    [Parameter] public string ColorClass { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.



    private const string CookieConsentKey = "CookieConsentKey";
    private const string CookiesConsentedValue = "yes";
    
    
    private bool ShowBanner { get; set; } = false;


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var consentedValue = await LocalStorage.GetItemAsync<string>(CookieConsentKey).ConfigureAwait(false);

            ShowBanner = consentedValue != CookiesConsentedValue;

            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }
    }



    private async Task AcceptCookie()
    {
        await LocalStorage.SetItemAsync(CookieConsentKey, CookiesConsentedValue);
        ShowBanner = false;
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }
}

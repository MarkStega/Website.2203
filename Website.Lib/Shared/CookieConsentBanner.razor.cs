using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
namespace Website.Lib.Shared;
public partial class CookieConsentBanner : ComponentBase
{
    [Inject] private ILocalStorageService LocalStorage { get; set; }


    [Parameter] public string ColorClass { get; set; }



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

using Material.Blazor;
using System.Net.Http.Json;
using Website.Client;

namespace Website.WebAssembly;


/// <summary>
/// Implements <see cref="INotification"/> for the Blazor WebAssembly project, posting messages to a controller on the server.
/// </summary>
public class WebAssemblyNotificationService : INotification
{
    private readonly HttpClient _httpClient;
    private readonly IMBToastService _mBToastService;


    public WebAssemblyNotificationService(HttpClient httpClient, IMBToastService mBToastService)
    {
        _httpClient = httpClient;
        _mBToastService = mBToastService;
    }


    public async Task Send(ContactMessage message)
    {
        NotifyError(await _httpClient.PostAsJsonAsync("api/Notification/PostContactMessage", message).ConfigureAwait(false));
    }


    public async Task Send(RecruitmentEnquiry message)
    {
        NotifyError(await _httpClient.PostAsJsonAsync("api/Notification/PostRecruitmentEnquiry", message).ConfigureAwait(false));
    }


    public async Task Send(RealEstateInvestorEnquiry message)
    {
        NotifyError(await _httpClient.PostAsJsonAsync("api/Notification/PostRealEstateInvestorEnquiry", message).ConfigureAwait(false));
    }


    public async Task Send(VentureCapitalEnquiry message)
    {
        NotifyError(await _httpClient.PostAsJsonAsync("api/Notification/PostVentureCapitalEnquiry", message).ConfigureAwait(false));
    }


    private void NotifyError(HttpResponseMessage response)
    {
#warning need to find out why the IMBToastService throws an exception unable to find the OnAdd method.
        //if (response.IsSuccessStatusCode)
        //{
        //    _mBToastService.ShowToast(MBToastLevel.Error, "Message failed to send, try again in a few seconds.");
        //}
    }
}

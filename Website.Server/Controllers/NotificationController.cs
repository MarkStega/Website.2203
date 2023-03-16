using Microsoft.AspNetCore.Mvc;
using Website.Client;

namespace Website.Server;


/// <summary>
/// Receives notifications from the Blazor WebAssembly client app. Applies messages received to <see cref="ServerNotificationService"/>
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    public readonly INotification _notificationService;


    public NotificationController(INotification notificationService)
    {
        _notificationService = notificationService;
    }


    /// <summary>
    /// POST "contact me" messages.
    /// </summary>
    /// <param name="contactMessage"></param>
    /// <returns></returns>
    [HttpPost("PostContactMessage")]
    public async Task PostContactMessage([FromBody] ContactMessage contactMessage)
    {
        await _notificationService.Send(contactMessage).ConfigureAwait(false);
    }


    /// <summary>
    /// POST client/real estate investor enquries.
    /// </summary>
    /// <param name="contactMessage"></param>
    /// <returns></returns>
    [HttpPost("PostRealEstateInvestorEnquiry")]
    public async Task PostRealEstateInvestorEnquiry([FromBody] RealEstateInvestorEnquiry realEstateInvestorEnquiry)
    {
        await _notificationService.Send(realEstateInvestorEnquiry).ConfigureAwait(false);
    }


    /// <summary>
    /// POST recruitment enquries.
    /// </summary>
    /// <param name="contactMessage"></param>
    /// <returns></returns>
    [HttpPost("PostRecruitmentEnquiry")]
    public async Task PostRecruitmentEnquiry([FromBody] RecruitmentEnquiry recruitmentEnquiry)
    {
        await _notificationService.Send(recruitmentEnquiry).ConfigureAwait(false);
    }


    /// <summary>
    /// POST VC investor enquries.
    /// </summary>
    /// <param name="contactMessage"></param>
    /// <returns></returns>
    [HttpPost("PostVentureCapitalEnquiry")]
    public async Task PostVentureCapitalEnquiry([FromBody] VentureCapitalEnquiry ventureCapitalEnquiry)
    {
        await _notificationService.Send(ventureCapitalEnquiry).ConfigureAwait(false);
    }
}
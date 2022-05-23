using Microsoft.AspNetCore.Mvc;
using Website.Lib;

namespace Website.BlazorWebAssembly.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    public readonly INotificationService _notificationService;


    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }


    [HttpPost("PostContactMessage")]
    public async Task PostContactMessage([FromBody] ContactMessage contactMessage)
    {
        await _notificationService.SendNotification(contactMessage).ConfigureAwait(false);
    }


    [HttpPost("PostRealEstateInvestorEnquiry")]
    public async Task PostRealEstateInvestorEnquiry([FromBody] RealEstateInvestorEnquiry realEstateInvestorEnquiry)
    {
        await _notificationService.SendNotification(realEstateInvestorEnquiry).ConfigureAwait(false);
    }


    [HttpPost("PostRecruitmentEnquiry")]
    public async Task PostRecruitmentEnquiry([FromBody] RecruitmentEnquiry recruitmentEnquiry)
    {
        await _notificationService.SendNotification(recruitmentEnquiry).ConfigureAwait(false);
    }


    [HttpPost("PostVentureCapitalEnquiry")]
    public async Task PostVentureCapitalEnquiry([FromBody] VentureCapitalEnquiry ventureCapitalEnquiry)
    {
        await _notificationService.SendNotification(ventureCapitalEnquiry).ConfigureAwait(false);
    }
}
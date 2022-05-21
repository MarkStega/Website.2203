using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Website.Lib;

namespace Website.BlazorWebAssembly.Client;

public class TeamsNotificationService : ITeamsNotificationService
{
   
    public Task SendNotification(ContactMessage message)
    {
        return Task.CompletedTask;
    }

    public Task SendNotification(RecruitmentEnquiry message)
    {
        return Task.CompletedTask;
    }

    public Task SendNotification(RealEstateInvestorEnquiry message)
    {
        return Task.CompletedTask;
    }

    public Task SendNotification(VentureCapitalEnquiry message)
    {
        return Task.CompletedTask;
    }
}

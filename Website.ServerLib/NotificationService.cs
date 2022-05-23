namespace Website.Lib;

using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class NotificationService : INotificationService
{
    private const string _messagingWebhook = "https://blacklandcapital.webhook.office.com/webhookb2/6ccfaed1-7c02-440c-83f0-9265cf35b379@ef73a184-f1db-4f24-b406-e4f8f9633dfa/IncomingWebhook/b89fe29282274986b059a32b41fea397/34ba3a07-c6f6-4e3f-896d-148fb6c1765f";
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }


    private async Task SendGenericNotification(IMessage message)
    {
        try
        {
            var client = new HttpClient();

            var json = message.GetMessageCardJson(_serializerOptions);
            
            var response = await client.PostAsync(_messagingWebhook, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new NotSupportedException($"Received failed result {response.StatusCode} when posting events to Microsoft Teams.");
            }

            _logger.LogInformation($"Sent message to Teams using {_messagingWebhook}; received this response: {response.StatusCode}", message, response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact message to Teams", message);
        }
    }

    public Task SendNotification(ContactMessage message)
    {
        return SendGenericNotification(message);
    }

    public Task SendNotification(RecruitmentEnquiry message)
    {
        return SendGenericNotification(message);
    }

    public Task SendNotification(RealEstateInvestorEnquiry message)
    {
        return SendGenericNotification(message);
    }

    public Task SendNotification(VentureCapitalEnquiry message)
    {
        return SendGenericNotification(message);
    }
}

namespace Website.Lib;

using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class TeamsNotificationService : ITeamsNotificationService
{
    private class MessageCard
    {
        public string Title { get; } = "Dioptra Website Contact Message";

        public string Text { get; } = $"Received on {DateTime.Now:ddd dd-MM-yyyy HH:mm:ss}";
        
        public List<Section> Sections { get; set; } = new();
    }

    private class Section
    {
        public List<Fact> Facts { get; set; } = new();
    }

    private class Fact
    {
        public string Name { get; set; } = "";
        
        public string Value{ get; set; } = "";
    }

    private const string _messagingWebhook = "https://blacklandcapital.webhook.office.com/webhookb2/6ccfaed1-7c02-440c-83f0-9265cf35b379@ef73a184-f1db-4f24-b406-e4f8f9633dfa/IncomingWebhook/b89fe29282274986b059a32b41fea397/34ba3a07-c6f6-4e3f-896d-148fb6c1765f";
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<TeamsNotificationService> _logger;

    public TeamsNotificationService(IHttpClientFactory clientFactory, ILogger<TeamsNotificationService> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }


    public async Task SendNotification(ContactData contactData)
    {
        try
        {
            var client = new HttpClient();

            MessageCard messageCard = new()
            {
                Sections = new()
                {
                    {
                        new()
                        {
                            Facts = new()
                            {
                                new() { Name = "Name", Value = contactData.Name },
                                new() { Name = "Email", Value = contactData.Email },
                                new() { Name = "Phone", Value = contactData.Phone },
                                new() { Name = "Message", Value = contactData.Message },
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(messageCard, _serializerOptions);
            
            var response = await client.PostAsync(_messagingWebhook, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new NotSupportedException($"Received failed result {response.StatusCode} when posting events to Microsoft Teams.");
            }

            _logger.LogInformation($"Sent contact message to Teams using {_messagingWebhook}; received this response: {response.StatusCode}", contactData, response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact message to Teams", contactData);
        }
    }
}

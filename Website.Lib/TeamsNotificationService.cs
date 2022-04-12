namespace Website.Lib;

using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class TeamsNotificationService : ITeamsNotificationService
{
    private const string _teamsNotifierClient = "TEAMS_NOTIFIER_CLIENT";
    private const string _webhookUri = "https://blacklandcapital.webhook.office.com/webhookb2/6ccfaed1-7c02-440c-83f0-9265cf35b379@ef73a184-f1db-4f24-b406-e4f8f9633dfa/IncomingWebhook/b89fe29282274986b059a32b41fea397/34ba3a07-c6f6-4e3f-896d-148fb6c1765f";

    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<TeamsNotificationService> _logger;

    public TeamsNotificationService(IHttpClientFactory clientFactory, ILogger<TeamsNotificationService> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }


    private HttpClient CreateClient()
    {
        var client = _clientFactory.CreateClient(_teamsNotifierClient);

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return client;
    }

    public async Task SendNotification(ContactData contactData)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(_webhookUri, contactData);

            _logger.LogInformation($"Sent contact message to Teams using {_webhookUri}; received this response: {response.StatusCode}", contactData, response.StatusCode);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Failed to send contact message to Teams", contactData);
        }
    }
}

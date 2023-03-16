using System.Text;
using System.Text.Json;
using Website.Client;

namespace Website.Server;


/// <summary>
/// Implements <see cref="INotification"/> for the Blazor Server/WASM hosting project.
/// </summary>
public class ServerNotificationService : INotification
{
    private static readonly string _messagingWebhook = Environment.GetEnvironmentVariable("MESSAGING_WEBHOOK") ?? "https://nonexistent.nothing";

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly ILogger<ServerNotificationService> _logger;

    public ServerNotificationService(ILogger<ServerNotificationService> logger)
    {
        _logger = logger;
    }


    private async Task GenericSend(IMessage message)
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


    /// <inheritdoc/>
    public Task Send(ContactMessage message)
    {
        return GenericSend(message);
    }


    /// <inheritdoc/>
    public Task Send(RecruitmentEnquiry message)
    {
        return GenericSend(message);
    }


    /// <inheritdoc/>
    public Task Send(RealEstateInvestorEnquiry message)
    {
        return GenericSend(message);
    }


    /// <inheritdoc/>
    public Task Send(VentureCapitalEnquiry message)
    {
        return GenericSend(message);
    }
}

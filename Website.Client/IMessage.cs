using System.Text.Json;

namespace Website.Client;
public interface IMessage
{
    string GetMessageCardJson(JsonSerializerOptions jsonSerializerOptions);
}
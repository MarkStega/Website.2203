using System.Text.Json;

namespace Website.Lib;
public interface IMessage
{
    string GetMessageCardJson(JsonSerializerOptions jsonSerializerOptions);
}
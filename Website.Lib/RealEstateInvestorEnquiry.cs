using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Website.Lib;
public class RealEstateInvestorEnquiry : IMessage
{
    private class MessageCard
    {
        public string Title { get; } = "Dioptra Website Real Estate Investor Enquiry";

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

        public string Value { get; set; } = "";
    }


    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailValidation(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [PhoneValidation(ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string CompanyName { get; set; } = string.Empty;
    [Required]
    public string Website { get; set; } = string.Empty;
    [Required]
    public string HowCanWeHelp { get; set; } = string.Empty;


    public string GetMessageCardJson(JsonSerializerOptions jsonSerializerOptions)
    {
        MessageCard messageCard = new()
        {
            Sections = new()
            {
                {
                    new()
                    {
                        Facts = new()
                        {
                            new() { Name = "Name", Value = Name },
                            new() { Name = "Email", Value = Email },
                            new() { Name = "Phone", Value = Phone },
                            new() { Name = "CompanyName", Value = CompanyName },
                            new() { Name = "Website", Value = Website },
                            new() { Name = "HowCanWeHelp", Value = HowCanWeHelp },
                        }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(messageCard, jsonSerializerOptions);

        return json;
    }
}


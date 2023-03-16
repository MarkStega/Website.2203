using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Website.Client;
public class RecruitmentEnquiry : IMessage
{
    public enum RoleType { Analyst, Technologist, Other}

    private class MessageCard
    {
        public string Title { get; } = "Dioptra Website Recruitment Query";

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
    public RoleType ProspectiveRole { get; set; }

    [Required]
    public string AboutMe { get; set; } = string.Empty;


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
                            new() { Name = "PropspectiveRole", Value = ProspectiveRole.ToString() },
                            new() { Name = "AboutMe", Value = AboutMe },
                        }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(messageCard, jsonSerializerOptions);

        return json;
    }
}


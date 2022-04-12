namespace Website.Lib;

using System.Text.Json;

using System.ComponentModel.DataAnnotations;

public class ContactData
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailValidation(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [PhoneValidation(ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;
}


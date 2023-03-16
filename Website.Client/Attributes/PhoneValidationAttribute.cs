using PhoneNumbers;
using System.ComponentModel.DataAnnotations;

namespace Website.Client;

/// <summary>
/// Validates correct phone number formatting, given a UK region code.
/// </summary>
public class PhoneValidationAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var phoneNumberUtil = PhoneNumberUtil.GetInstance();

        if (!phoneNumberUtil.IsPossibleNumber((value ?? "").ToString(), "GB"))
        {
            return new ValidationResult(ErrorMessage, new[] { validationContext.MemberName ?? "" });
        }

        return null;
    }
}

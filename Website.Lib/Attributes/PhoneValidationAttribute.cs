namespace Website.Lib;

using PhoneNumbers;
using System.ComponentModel.DataAnnotations;

public class PhoneValidationAttribute : ValidationAttribute
{
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

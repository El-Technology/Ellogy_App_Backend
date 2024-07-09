using System.ComponentModel.DataAnnotations;
using UserManager.Common.Helpers;

namespace UserManager.Common.Attributes;
public class PasswordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string password)
        {
            PasswordValidationHelper.ValidatePassword(password);

            return ValidationResult.Success;
        }
        return new ValidationResult("Password is required");
    }
}
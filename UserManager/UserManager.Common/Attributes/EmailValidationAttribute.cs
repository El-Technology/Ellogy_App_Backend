using System.ComponentModel.DataAnnotations;
using UserManager.Common.Exceptions;
using UserManager.Common.Helpers;

namespace UserManager.Common.Attributes;
public class EmailValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string email)
        {

            if (!EmailHelper.IsValidEmail(email))
                throw new InvalidEmailException(email);

            return ValidationResult.Success;
        }
        return new ValidationResult("Email is required");
    }
}
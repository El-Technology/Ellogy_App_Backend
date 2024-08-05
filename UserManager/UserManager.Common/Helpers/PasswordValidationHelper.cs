using UserManager.Common.Exceptions;

namespace UserManager.Common.Helpers;
public static class PasswordValidationHelper
{
    private const int MinPasswordLength = 12;

    public static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password));

        if (password.Length < MinPasswordLength)
            throw new PasswordValidationException($"Password must be at least {MinPasswordLength} characters long");

        if (!password.Any(char.IsDigit))
            throw new PasswordValidationException("Password must contain at least one digit");

        if (!password.Any(char.IsUpper))
            throw new PasswordValidationException("Password must contain at least one uppercase letter");

        if (!password.Any(char.IsLower))
            throw new PasswordValidationException("Password must contain at least one lowercase letter");

        if (!password.Any(char.IsSymbol))
            throw new PasswordValidationException("Password must contain at least one special character");
    }
}

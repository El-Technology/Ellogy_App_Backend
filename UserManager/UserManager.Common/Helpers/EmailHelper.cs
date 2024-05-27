using System.Text.RegularExpressions;

namespace UserManager.Common.Helpers;
public static class EmailHelper
{
    private static readonly Regex EmailRegex = new Regex(
    @"^(([^<>()\[\]\\.,;:\s@\""]+(\.[^<>()\[\]\\.,;:\s@\""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z0-9]+[-a-zA-Z0-9]*[a-zA-Z0-9]+\.)+[a-zA-Z]{2,}))$",
    RegexOptions.Compiled);

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return EmailRegex.IsMatch(email);
    }
}

using System.Security.Claims;

namespace AICommunicationService.Common.Helpers;
public static class TokenParseHelper
{
    public static string GetValueFromJwt(string claimName, ClaimsPrincipal claimsPrincipal)
    {
        var value = string.Empty;

        try
        {
            value = claimsPrincipal.FindFirst(claimName)?.Value;
            ArgumentNullException.ThrowIfNull(value);
        }
        catch
        {
            throw new Exception($"Taking {claimName} error, try again later");
        }

        return value;
    }
}

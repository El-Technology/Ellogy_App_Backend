using PaymentManager.Common.Options;
using System.Security.Claims;

namespace PaymentManager.Common.Helpers;
public static class TokenParseHelper
{
    public static Guid GetUserId(ClaimsPrincipal claimsPrincipal)
    {
        var status = Guid.TryParse(claimsPrincipal.FindFirst(JwtOptions.UserIdClaimName)?.Value, out var userId);
        if (!status)
            throw new Exception("Taking user id error, try again later");

        return userId;
    }
}

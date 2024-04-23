using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PaymentManager.Common.Options;

public static class JwtOptions
{
    public const string UserIdClaimName = "userId";
    public const string Issuer = "https://ellogy.usermanager";

    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
    {
        return key is null
            ? throw new NullReferenceException()
            : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }
}
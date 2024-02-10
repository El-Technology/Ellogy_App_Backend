using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PaymentManager.Common.Options;

public static class JwtOptions
{
    public const string UserIdClaimName = "userId";
    public const string Issuer = "https://ellogy.usermanager";

    private static readonly string? Key = EnvironmentVariables.JwtSecretKey;

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return Key is null
            ? throw new NullReferenceException()
            : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserManager.Common.Options;

public static class JwtOptions
{
    public const string UserIdClaimName = "userId";
    public const string Issuer = "https://ellogy.usermanager";

    public static readonly TimeSpan TokenLifeTime = TimeSpan.FromMinutes(100);
    public static readonly TimeSpan RefreshTokenLifeTime = TimeSpan.FromDays(7);

    private static readonly string? Key = EnvironmentVariables.JwtSecretKey;

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        Key is null
            ? throw new NullReferenceException()
            : new(Encoding.UTF8.GetBytes(Key));

}

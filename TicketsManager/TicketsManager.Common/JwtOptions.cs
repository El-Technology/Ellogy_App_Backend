using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TicketsManager.Common;

public static class JwtOptions
{
    public const string UserIdClaimName = "userId";
    public const string Issuer = "https://ellogy.usermanager";
    public static readonly TimeSpan TokenLifeTime = TimeSpan.FromMinutes(10);

    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key) =>
        key is null
            ? throw new NullReferenceException()
            : new(Encoding.UTF8.GetBytes(key));

}
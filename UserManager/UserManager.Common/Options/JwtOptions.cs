using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UserManager.Common.Options;

public static class JwtOptions
{
    public const string AccountPlan = "AccountPlan";
    public const string UserIdClaimName = "userId";
    public const string Issuer = "https://ellogy.usermanager";

    public static readonly TimeSpan TokenLifeTime = TimeSpan.FromMinutes(100);
    public static readonly TimeSpan RefreshTokenLifeTime = TimeSpan.FromDays(7);

    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
    {
        return key is null
            ? throw new NullReferenceException()
            : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }
}
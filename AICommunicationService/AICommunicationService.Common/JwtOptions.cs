using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AICommunicationService.Common;

public static class JwtOptions
{
    public const string ACCOUNT_PLAN = "AccountPlan";
    public const string USER_ID_CLAIM_NAME = "userId";
    public const string ISSUER = "https://ellogy.usermanager";

    private static readonly string? Key = EnvironmentVariables.JwtSecretKey;

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return Key is null
            ? throw new NullReferenceException()
            : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}
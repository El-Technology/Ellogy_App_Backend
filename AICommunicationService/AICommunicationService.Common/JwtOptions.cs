using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AICommunicationService.Common;

public static class JwtOptions
{
    public const string ACCOUNT_PLAN = "AccountPlan";
    public const string USER_ID_CLAIM_NAME = "userId";
    public const string ISSUER = "https://ellogy.usermanager";

    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
    {
        return key is null
            ? throw new NullReferenceException()
            : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }
}
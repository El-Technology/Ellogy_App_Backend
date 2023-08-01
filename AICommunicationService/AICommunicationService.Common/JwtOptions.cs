using Microsoft.IdentityModel.Tokens;
using System.Text;

using TicketsManager.Common;

namespace AICommunicationService.Common;

public static class JwtOptions
{
    public const string Issuer = "https://ellogy.usermanager";
    public static readonly TimeSpan TokenLifeTime = TimeSpan.FromMinutes(10);

    private static readonly string? Key = EnvironmentVariables.JwtSecretKey;

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        Key is null
            ? throw new NullReferenceException()
            : new(Encoding.UTF8.GetBytes(Key));

}
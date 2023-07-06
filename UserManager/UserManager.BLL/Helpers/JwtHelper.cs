using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserManager.Common;
using UserManager.Common.Options;
using UserManager.DAL.Models;

namespace UserManager.BLL.Helpers;

public static class JwtHelper
{
    public static string GenerateJwt(User user)
    {
        var claims = GetClaims(user);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new(claims),
            Expires = DateTime.Now.Add(JwtOptions.TokenLifeTime),
            IssuedAt = DateTime.Now,
            Issuer = JwtOptions.Issuer,
            SigningCredentials = new(JwtOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        };

        return new JwtSecurityTokenHandler().CreateEncodedJwt(tokenDescriptor);
    }

    private static IEnumerable<Claim> GetClaims(User user)
    {
        return new List<Claim>
        {
            new ("userId", user.Id.ToString()),
            new (ClaimTypes.Email, user.Email),
        };
    }
}

﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserManager.BLL.Exceptions;
using UserManager.Common.Options;
using UserManager.DAL.Models;

namespace UserManager.BLL.Helpers;

public static class JwtHelper
{
    private static IEnumerable<Claim> GetClaims(User user)
    {
        return new List<Claim>
        {
            new (JwtOptions.UserIdClaimName, user.Id.ToString()),
            new (ClaimTypes.Email, user.Email),
        };
    }

    public static void ValidateJwt(string jwtToken, TokenValidationParameters tokenValidationParameters)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out _);
        }
        catch (Exception)
        {
            throw new InvalidJwtException();
        }
    }

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

    public static string? GetJwtClaimValue(string jwtToken, string claimName)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        return jwt.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
    }

}

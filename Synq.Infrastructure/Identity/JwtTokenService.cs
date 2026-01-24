using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Synq.Application.Common.Interfaces;
using Synq.Application.DTOs.Auth;
using Synq.Domain.Entities;

namespace Synq.Infrastructure.Identity;

public class JwtTokenService(IConfiguration configuration, IPasswordHasher hasher) : IJwtTokenService
{
    public TokenDto GenerateNewToken(User user)
    {
        var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

        var b = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(b);
        var refreshToken = hasher.HashPassword(Convert.ToBase64String(b));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"], 
            audience: configuration["Jwt:Audience"], 
            expires: DateTime.UtcNow.AddHours(2), 
            claims: claims, 
            signingCredentials: credentials);

        return new TokenDto(AccessToken: new JwtSecurityTokenHandler().WriteToken(token), RefreshToken: refreshToken );
    }
}
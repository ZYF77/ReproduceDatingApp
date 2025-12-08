using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens; //高并发时考虑替换

namespace API.Services;

public class TokenService(IOptions<JwtSettings> options,UserManager<AppUser> userManager) : ITokenService
{
    public async Task<string> CreateToken(AppUser user)
    {
        var tokenKey = Encoding.UTF8.GetBytes(options.Value.Secret);
        if (tokenKey.Length <64) throw new Exception("Your token key needs to be >= 64 characters for HMAC-SHA512");

        var key = new SymmetricSecurityKey(tokenKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); //如何加密

        //颁发对象信息
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,user.Email!),
            new(ClaimTypes.NameIdentifier,user.Id.ToString())
        };
        if (user.Member != null)
        {
            claims.Add(new Claim(ClaimTypes.DateOfBirth, user.Member.DateOfBirth.ToString("yyyy-MM-dd")));
        }
        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        //配置颁发的token签名
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(options.Value.TokenExpiryInMinutes),
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor); //创建

        return tokenHandler.WriteToken(token); //颁发
    }
}

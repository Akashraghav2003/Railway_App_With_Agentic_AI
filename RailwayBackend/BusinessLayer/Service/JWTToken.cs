using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Entity;

namespace BusinessLayer.Service;

public class JWTToken : IJwtToken
{

    private readonly IConfiguration _config;

    public JWTToken(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("UserId", user.UserId.ToString()),   
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
}


    public string ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["JWT:Key"]);


        try
        {
            var claimsPrincipal = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["JWT:Issuer"],
                ValidAudience = _config["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, claimsPrincipal, out validatedToken);
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            return email;

        }
        catch (Exception ex)
        {
            throw new Exception("Invalid or expired token.", ex);
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Model;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services.Utility;

public class Authentication
{
    private readonly IConfiguration _configuration;

    public Authentication(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(User user)
    {  List<Claim> claims = new List<Claim>();
        
        claims.Add(new Claim("id", user.Id.ToString()));
        claims.Add(new Claim("name", user.Username)); ;
        claims.Add(new Claim("email",user.Email));
        
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
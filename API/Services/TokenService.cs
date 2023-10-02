using API.Entities;
using API.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private IDbConnection db;

    public TokenService(IConfiguration config)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        this.db = new SqlConnection(config.GetConnectionString("DefaultConnection"));
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<string> CreateToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()), // lo jalo como: User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName), // user.FindFirst(ClaimTypes.Name)?.Value
        };

        var roles = await db.QueryAsync<string>("sp_getUserRolesName",
                                    new { userId = user.Id },
                                    commandType: CommandType.StoredProcedure);

        // roles viene como IEnumerable, asi que me añade la lista
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        /* 
         p' admin        
             "role": [
                "Admin",
                "Moderator"
              ],

         p' user
            "role": "Member",
        */


        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
}

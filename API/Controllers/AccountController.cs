using API.DTOs;
using API.Entities;
using API.Interfaces;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private IDbConnection db;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public AccountController(IConfiguration configuration, 
                             ITokenService tokenService,
                             IUserRepository userRepository)
    {
        this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // POST: api/Account/register
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        db.Insert(user);

        var userDto = new UserDto
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };

        return Ok(userDto);
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // POST: api/Account/login
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        //var user = (
        //            await db.QueryAsync<AppUser>("sp_getUserByUserName",
        //                        new { userName = loginDto.Username },
        //                        commandType: CommandType.StoredProcedure)
        //            ).FirstOrDefault();
        var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);

        if (user == null) return Unauthorized("Invalid Username.");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password.");
        }

        var userDto = new UserDto
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain == 1)?.Url
        };

        return Ok(userDto);
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //

    private async Task<bool> UserExists(string username)
    {
        var user = await db.QueryAsync<AppUser>("sp_getUserByUserName",
                                new { userName = username.ToLower() },
                                commandType: CommandType.StoredProcedure);

        return user.IsNullOrEmpty() ? false : true;
    }
}

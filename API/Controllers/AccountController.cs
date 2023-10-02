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
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private IDbConnection db;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public AccountController(IConfiguration configuration, 
                             ITokenService tokenService,
                             IUserRepository userRepository,
                             IMapper mapper)
    {
        // pasar a userRepo todo lo q ocupa db
        this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        _tokenService = tokenService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // POST: api/Account/register
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

        var user = _mapper.Map<AppUser>(registerDto);

        using var hmac = new HMACSHA512();

        user.UserName = registerDto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;
        //db.Insert(user); AHORA CON SP

        // creo user con sp y lo pongo con role 'Member'
        var parameters = new DynamicParameters();

        parameters.Add("@userName", user.UserName);
        parameters.Add("@knownAs", user.KnownAs);
        parameters.Add("@gender", user.Gender);
        parameters.Add("@dateOfBirth", user.DateOfBirth);
        parameters.Add("@city", user.City);
        parameters.Add("@country", user.Country);
        parameters.Add("@passwordHash", user.PasswordHash);
        parameters.Add("@passwordSalt", user.PasswordSalt);

        // retorno el userId creado
        var succes = await db.QueryAsync<int>("sp_createUser",
                                               parameters,
                                               commandType: CommandType.StoredProcedure);

        if (succes.FirstOrDefault() <= 0) return BadRequest("Problems creating the user.");

        var userDto = new UserDto
        {
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Gender = user.Gender,
            Token = await _tokenService.CreateToken(user)
        };

        return Ok(userDto);
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // POST: api/Account/login
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
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
            KnownAs = user.KnownAs,
            Gender = user.Gender,
            Token = await _tokenService.CreateToken(user),
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

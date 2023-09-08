using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository,
                           IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }



    //////////////////////////////////////////
    /////////////////////////////////////////////
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {// el está usando getMembers
        var users = await _userRepository.GetUsersAsync();
        var members = _mapper.Map<IEnumerable<MemberDto>>(users);

        return Ok(members);
    }


    //////////////////////////////////////////
    /////////////////////////////////////////////    
    [HttpGet("{username}")]
    public async Task<ActionResult<AppUser>> GetUser(string username)
    {// el está usando getMember
        var user = await _userRepository.GetUserByUsernameAsync(username);
        var member = _mapper.Map<MemberDto>(user);

        return Ok(member);
    }


    //////////////////////////////////////////////
    /////////////////////////////////////////////////
    // PUT api/Users
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        //var username = User.GetUsername();

        var user = await _userRepository.GetUserByUsernameAsync(username);

        if (user == null) return NotFound();

        // lo q esta em memberUpdateDto lo mete a user
        //                |---------->
        _mapper.Map(memberUpdateDto, user);

        // aùn y si no hay cambios me sobreescribe todo
        if (await _userRepository.UpdateAsync(user)) return NoContent();

        return BadRequest("Failed to update user.");
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // POST: api/Users/add-photo
}

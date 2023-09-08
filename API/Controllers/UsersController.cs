using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}

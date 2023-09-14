using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILikesRepository _likesRepository;

    public LikesController(IUserRepository userRepository,
                           ILikesRepository likesRepository)
    {
        _userRepository = userRepository;
        _likesRepository = likesRepository;
    }
    

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    [HttpPost("{username}")] // a quien se le da el like
    public async Task<ActionResult> AddLike(string username)
    {
        var likedUser = await _userRepository.GetUserByUsernameAsync(username);

        var sourceUserId = User.GetUserId(); // el que da el like
        var sourceUser = await _userRepository.GetUserByIdAsync(sourceUserId);

        if (likedUser == null) return NotFound();

        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself.");

        var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

        if (userLike != null) return BadRequest("You already like this user.");

        //userLike = new UserLike
        //{
        //    SourceUserId = sourceUserId,
        //    TargetUserId = likedUser.Id,
        //};

        //sourceUser.LikedUsers.Add(userLike); // aqui crea la entrada en la tabla UserLike

        //if (await _uow.Complete()) return Ok(); // el saveAllAsync

        if (await _likesRepository.AddLike(sourceUserId, likedUser.Id)) return Ok();

        return BadRequest("Failed to like user.");
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
    {
        var userId = User.GetUserId();

        var users = await _likesRepository.GetUserLikes(predicate, userId);

        
        return Ok(users);
    }

    //public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    //{
    //    likesParams.UserId = User.GetUserId();

    //    var pagedUsers = await _uow.LikesRepository.GetUserLikes(likesParams);

    //    Response.AddPaginationHeader(new PaginationHeader(pagedUsers.CurrentPage,
    //        pagedUsers.PageSize, pagedUsers.TotalCount, pagedUsers.TotalPages));

    //    return Ok(pagedUsers);
    //}
}

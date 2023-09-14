using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
    Task<bool> AddLike(int sourceUserId, int targetUserId);

    //Task<LikesPagedList> GetUserLikes(LikesParams likesParams);
    Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId);


    //Task<AppUser> GetUserWithLikes(int userId); no lo voy a coupar
}

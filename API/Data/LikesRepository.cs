using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace API.Data;

public class LikesRepository : ILikesRepository
{
    private IDbConnection db;
    private readonly IMapper _mapper;

    public LikesRepository(IConfiguration configuration,
                           IMapper mapper)
    {
        this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        _mapper = mapper;
    }



    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        //var like = await _context.Likes.FindAsync(sourceUserId, targetUserId);
        //return like;

        var like = await db.QueryAsync<UserLike>("sp_getLike",
                                    new { sourceUserId = sourceUserId, targetUserId = targetUserId },
                                    commandType: CommandType.StoredProcedure);

        return like.SingleOrDefault();
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<bool> AddLike(int sourceUserId, int targetUserId)
    {// antes de meterlo checo que no exista previamente el mismo like, asi q debe mandar 1 row added
        var succes = await db.QueryAsync<int>("sp_getLike",
                                    new { sourceUserId = sourceUserId, targetUserId = targetUserId },
                                    commandType: CommandType.StoredProcedure);

        return succes.FirstOrDefault() == 1 ? true : false ;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
    {
        // la lista de a quienes a dado like - userid = sourceuserid
        // la lista de quienes le han dado like - userid = likeduserid
        var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        // para los q este user ha dado like
        if (predicate == "liked")
        {
            likes = likes.Where(l => l.SourceUserId == userId);
            // selecciona en "users" solo a los q estan en la lista "likes"
            users = likes.Select(l => l.TargetUser);
        }

        // los q le han dado like al user actual
        if (predicate == "likedBy")
        {
            likes = likes.Where(l => l.TargetUserId == userId);
            // selecciona en "users" solo a los q estan en la lista "likes"
            users = likes.Select(l => l.SourceUser);
        }

        // al final users es a quienes doy like ( si "liked" ) o los q 
        // le han dado like ( si "likedBy" )

        var likedUsers = users.Select(u => new LikeDto
        {
            UserName = u.UserName,
            KnownAs = u.KnownAs,
            Age = u.DateOfBirth.CalculateAge(),
            PhotoUrl = u.Photos.FirstOrDefault(p => p.IsMain).Url,
            City = u.City,
            Id = u.Id,
        });

        return likedUsers.ToListAsync();
    }
    //public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
    //{
    //    // la lista de a quienes a dado like - userid = sourceuserid
    //    // la lista de quienes le han dado like - userid = likeduserid
    //    var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
    //    var likes = _context.Likes.AsQueryable();

    //    // para los q este user ha dado like
    //    if (likesParams.Predicate == "liked")
    //    {
    //        likes = likes.Where(l => l.SourceUserId == likesParams.UserId);
    //        // selecciona en "users" solo a los q estan en la lista "likes"
    //        users = likes.Select(l => l.TargetUser);
    //    }

    //    // los q le han dado like al user actual
    //    if (likesParams.Predicate == "likedBy")
    //    {
    //        likes = likes.Where(l => l.TargetUserId == likesParams.UserId);
    //        // selecciona en "users" solo a los q estan en la lista "likes"
    //        users = likes.Select(l => l.SourceUser);
    //    }

    //    var likedUsers = users.Select(u => new LikeDto
    //    {
    //        UserName = u.UserName,
    //        KnownAs = u.KnownAs,
    //        Age = u.DateOfBirth.CalculateAge(),
    //        PhotoUrl = u.Photos.FirstOrDefault(p => p.IsMain).Url,
    //        City = u.City,
    //        Id = u.Id,
    //    });

    //    var pagedResult = await PagedList<LikeDto>
    //                            .CreateAsync(likedUsers, likesParams.PageNumber,
    //                                         likesParams.PageSize);

    //    return pagedResult;
    //}

    /*
    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<AppUser> GetUserWithLikes(int userId) // NO LO VOY A OCUPAR
    { // lo ocupo p' en el controller en AddLike usar
      // sourceUser.LikedUsers.Add(userLike); // aqui crea la entrada en la tabla UserLike
        var user = await _context.Users
                                 .Include(u => u.LikedUsers) // LikedUsers a quienes les doy like
                                 .FirstOrDefaultAsync(u => u.Id == userId);

        return user;
    }
    */
}

using API.Interfaces;
using System.Data.SqlClient;
using System.Data;
using API.Entities;
using Dapper.Contrib.Extensions;
using Dapper;

namespace API.Data;

public class UserRepository : IUserRepository
{
    private IDbConnection db;

    public UserRepository(IConfiguration configuration)
    {
        this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }




    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        var user = await db.QueryAsync<AppUser>("sp_getUserById",
                                    new { userId = id },
                                    commandType: CommandType.StoredProcedure);

        return user.SingleOrDefault();
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        AppUser user;

        using (var lists = await db.QueryMultipleAsync("sp_getUserByUserName",
                                    new { userName = username },
                                    commandType: CommandType.StoredProcedure))
        {
            user = lists.Read<AppUser>().SingleOrDefault();
            user.Photos = lists.Read<Photo>().ToList();
        }

        return user;

        //var user = await db.QueryAsync<AppUser>("sp_getUserByUserName",
        //                          new { userName = username },
        //                          commandType: CommandType.StoredProcedure);

        //return user.SingleOrDefault();
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        List<AppUser> users;
        List<Photo> photos;

        using (var lists = await db.QueryMultipleAsync("sp_getAllUsersAndPhotos",
                                    commandType: CommandType.StoredProcedure))
        {
            users = lists.Read<AppUser>().ToList();
            photos = lists.Read<Photo>().ToList();
        }

        users.ForEach(u =>
        {
            u.Photos = photos.Where(p => p.AppUserId == u.Id)
                             .ToList();
        });

        return users;

        //var users = await db.QueryAsync<AppUser>("sp_getAllUsers",
        //                            commandType: CommandType.StoredProcedure);

        //return users.ToList();
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    public async Task<bool> SaveAllAsync()
    {
        //return await _context.SaveChangesAsync() > 0;
        return false;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    public async Task<bool> UpdateAsync(AppUser user)
    {
        var res = await db.UpdateAsync(user); // contrib

        return res;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //public async Task<MemberDto> GetMemberAsync(string username)
    //{
    //    // con la .ProjectTo NO necesito hacer el .include de las photos,
    //    // lo hace solito
    //    var member = await _context.Users
    //        .Where(u => u.UserName == username)
    //        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
    //        .FirstOrDefaultAsync();

    //    return member;


    //    // sin AutoMapper
    //    // return await _context.Users
    //    //                 .Where(u => u.UserName == username)
    //    //                 .Select(u => new MemberDto
    //    //                 {
    //    //                     Id = u.Id,
    //    //                     UserName = u.UserName,
    //    //                     KnownAs = u.KnownAs,
    //    //                 }).FirstOrDefaultAsync();
    //}

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    //{
    //    //var members = await _context.Users
    //    //    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
    //    //    .ToListAsync();

    //    //return members;

    //    var query = _context.Users.AsQueryable();

    //    query = query.Where(u => u.UserName != userParams.CurrentUsername);
    //    query = query.Where(u => u.Gender == userParams.Gender);

    //    // p' filtrar x la edad pasada
    //    var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
    //    var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
    //    query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

    //    query = userParams.OrderBy switch
    //    {
    //        "created" => query.OrderByDescending(u => u.Created),
    //        _ => query.OrderByDescending(u => u.LastActive)
    //    };

    //    return await PagedList<MemberDto>.CreateAsync(
    //                query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
    //                userParams.PageNumber, userParams.PageSize);
    //}

    /// <summary>
    /// Returns Id of photo inserted
    /// </summary>
    /// <param name="photo"></param>
    /// <returns>Id of photo inserted</returns>
    //public async Task<int> AddPhotoAsync(Photo photo)
    //{
    //    var res = await db.InsertAsync(photo);

    //    return res;
    //}

}

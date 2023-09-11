using API.Entities;
using Dapper;
using System.Data;

namespace API.Helpers;

public class AppUserPagedList : List<AppUser>
{
    public AppUserPagedList(IEnumerable<AppUser> items, int count,
                            int pageNumber, int pageSize)
    {
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
        TotalCount = count;
        AddRange(items);
    }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    // este metodo retorna una instancia de esta clase con la info
    // source es la query
    public static async Task<AppUserPagedList> CreateAsync(IDbConnection db, int pageNumber, int pageSize)
    {
        // ESTE
        var count = await db.QueryAsync<int>("sp_getUsersCount",
                                    commandType: CommandType.StoredProcedure);


        List<AppUser> users;
        List<Photo> photos;
        var parameters = new DynamicParameters();

        parameters.Add("@pageNumber", pageNumber);
        parameters.Add("@rowsOfPage", pageSize);
        //parameters.Add("@sortingCol", user.Id);
        //parameters.Add("@sortType", user.Id);

        using (var lists = await db.QueryMultipleAsync("sp_getSortedAndPagedUsers",
                                            parameters,
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

        return new AppUserPagedList(users, count.SingleOrDefault(), pageNumber, pageSize);
    }
}

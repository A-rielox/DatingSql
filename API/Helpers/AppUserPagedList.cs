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
    public static async Task<AppUserPagedList> CreateAsync(IDbConnection db, int pageNumber, int pageSize,
                                                             string currentUsername, string gender,
                                                             int minAge, int maxAge, string orderBy)
    {

        var minDob = DateTime.Today.AddYears(-maxAge - 1);
        var maxDob = DateTime.Today.AddYears(-minAge);

        // ESTE
        var parametersCount = new DynamicParameters();

        parametersCount.Add("@currentUsername", currentUsername);
        parametersCount.Add("@gender", gender);
        parametersCount.Add("@minDob", minDob);
        parametersCount.Add("@maxDob", maxDob);

        var count = await db.QueryAsync<int>("sp_getUsersCount",
                                             parametersCount,
                                             commandType: CommandType.StoredProcedure);

        List<AppUser> users;
        List<Photo> photos;
        var parameters = new DynamicParameters();

        parameters.Add("@pageNumber", pageNumber);
        parameters.Add("@rowsOfPage", pageSize);
        parameters.Add("@currentUsername", currentUsername);
        parameters.Add("@gender", gender);
        parameters.Add("@minDob", minDob);
        parameters.Add("@maxDob", maxDob);
        parameters.Add("@sortingCol", orderBy);

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

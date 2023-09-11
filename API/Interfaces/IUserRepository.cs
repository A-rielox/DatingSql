using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IUserRepository
{
    Task<bool> UpdateUserAsync(AppUser user);
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUserPagedList> GetPagedUsersAsync(UserParams userParams); // reemplaza a la de arriba
    Task<AppUser> GetUserByIdAsync(int id);
    Task<AppUser> GetUserByUsernameAsync(string username);

    //Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
    //Task<MemberDto> GetMemberAsync(string username);

    Task<int> AddPhotoAsync(Photo photo);    
    Task<bool> UpdatePhotos(SetMainPhoto setMainPhoto);
    Task<bool> DeletePhoto(int id);
}

// ANTIGUO
//Task<bool> UpdatePhotos(List<Photo> photos);
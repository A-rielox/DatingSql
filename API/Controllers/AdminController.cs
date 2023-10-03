using API.DTOs;
using API.Entities;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace API.Controllers;

public class AdminController : BaseApiController
{
    private IDbConnection db;

    public AdminController(IConfiguration configuration)
    {
        this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }

    ////////////////////////////////////////////////////////
    // GET: admin/users-with-roles
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult<List<FromDbUserForRoles>>> GetUsersWithRoles()
    {
        // de .Users tengo una related table ( userRoles, con una related table
        // q son los roles ) ( supongo q una navigation prop ) q va a los roles
        //var users = await _userManager.Users
        //            .OrderBy(u => u.UserName)
        //            .Select(u => new
        //            {
        //                u.Id,
        //                Username = u.UserName,
        //                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
        //            }).ToListAsync();


        //return Ok(users);

        List<FromDbUserForRoles> users;
        List<FromDbRoleForRoles> roles;

        using (var lists = await db.QueryMultipleAsync("sp_getUsersWithRoles",
                                    commandType: CommandType.StoredProcedure))
        {
            users = lists.Read<FromDbUserForRoles>().ToList();
            roles = lists.Read<FromDbRoleForRoles>().ToList();
        }

        users.ForEach(u =>
        {
            u.Roles = roles.Where(r => r.UserId == u.Id).Select(r => r.name).ToList();
        });

        return users;
    }

    ////////////////////////////////////////////////////////
    // POST: admin/edit-roles/{username}            CAMBIAR A Q SEA CON USERID EN LUGAR DE USERNAME
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")] // deberia ser PUT xq se esta actualizando
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {
        /**/
        if (string.IsNullOrEmpty(username)) return BadRequest("You must select at least one role.");

        //var selectedRoles = roles.Split(",").ToList();

        // arreglar estoy solo mandando la lista de nuevos roles

        List<FromDbUserForRoles> usersList;
        List<FromDbRoleForRoles> rolesList;

        using (var lists = await db.QueryMultipleAsync("sp_editRoles",
                                    // @userName NVARCHAR(50),
                                    //@rolesList NVARCHAR(500)
                                    new { userName = username, rolesList = roles },
                                    commandType: CommandType.StoredProcedure))
        {
            usersList = lists.Read<FromDbUserForRoles>().ToList();
            rolesList = lists.Read<FromDbRoleForRoles>().ToList();
        }

        usersList.ForEach(u =>
        {
            u.Roles = rolesList.Where(r => r.UserId == u.Id).Select(r => r.name).ToList();
        });

        
        //// retorno la lista actualizado de los roles q se tiene
        

        return Ok(usersList);
    }

    ////////////////////////////////////////////////////////
    // GET: admin/photos-to-mederate
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Solo Admins o Moderators.");
    }
}

//      Las Policy las configuro en IdentityServiceExtensions

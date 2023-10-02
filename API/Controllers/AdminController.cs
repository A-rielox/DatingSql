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

        var selectedRoles = roles.Split(",").ToArray();

        //var rolesForDb = selectedRoles.Select(r => new { username, role = r });

        var user = await _userManager.FindByNameAsync(username);

        // SIEMPRE q agarre cosas de un parameter TENGO q checar q no sea null
        if (user == null) return NotFound();

        // roles actuales
        var userRoles = await _userManager.GetRolesAsync(user);

        // añado los reles q se mandan en el query ( except p' no volver a poner los q ya tiene )
        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles.");

        // para quitar roles
        // si ya tenia algun role y no lo pasé como role en el query se va a remover
        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove from roles.");

        // retorno la lista actualizado de los roles q se tiene
        return Ok(await _userManager.GetRolesAsync(user));

        /**/

        return Ok();
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

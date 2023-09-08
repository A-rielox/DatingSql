using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    private IDbConnection db;

    public BuggyController(IConfiguration configuration)
    {
        this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // GET: api/buggy/auth
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        // para ver la respuesta de NO autorizado
        return "secret text";
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // GET: api/buggy/not-found
    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        // para respuesta de not found
        var thing = (db.Query<AppUser>("sp_getUserById",
                                    new { userId = -1 },
                                    commandType: CommandType.StoredProcedure))
                    .SingleOrDefault();

        if (thing == null) return NotFound();

        return Ok(thing);
    }

    //////////////////////////////////////////////// 54
    ///////////////////////////////////////////////////
    // GET: api/buggy/server-error
    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        // p' error null reference exception

        // me retorna null y al aplicarle un metodo ( .ToString() )
        // da una excepcion ( null reference exception )
        var thing = (db.Query<AppUser>("sp_getUserById",
                                    new { userId = -1 },
                                    commandType: CommandType.StoredProcedure))
                    .SingleOrDefault();

        var thingToReturn = thing.ToString();

        return thingToReturn;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // GET: api/buggy/bad-request
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("Bad Request");
    }


}

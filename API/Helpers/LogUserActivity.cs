using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

// en BaseApiController pongo la anotation [ServiceFilter(typeof(LogUserActivity))]

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // voy a ejecutar ___ despues de q el user haya hecho lo suyo ( cuando la accion en la api se halla completado )
        var resultContext = await next();

        // es true si el token q manda se pudo autenticar
        if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        var userId = resultContext.HttpContext.User.GetUserId();

        var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var user = await repo.GetUserByIdAsync(userId);

        user.LastActive = DateTime.Now;

        await repo.UpdateUserAsync(user);
    }

    /*
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // voy a ejecutar ___ despues de q el user haya hecho lo suyo ( cuando la accion en la api se halla completado )
        var resultContext = await next();

        // es true si el token q manda se pudo autenticar
        if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        var userId = resultContext.HttpContext.User.GetUserId();

        var uow = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var user = await uow.UserRepository.GetUserByIdAsync(userId);

        user.LastActive = DateTime.UtcNow;
        await uow.Complete();
    }
    */
}

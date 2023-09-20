using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddAplicationServices(
                            this IServiceCollection services,
                            IConfiguration config
                            )
    {
        services.AddCors();

        services.AddScoped<ITokenService, TokenService>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        //services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

        services.AddScoped<IUserRepository, UserRepository>();

        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

        services.AddScoped<IPhotoService, PhotoService>();

        services.AddScoped<LogUserActivity>();

        services.AddScoped<ILikesRepository, LikesRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        //services.AddScoped<IUserRepository, UserRepository>();


        return services;
    }
}

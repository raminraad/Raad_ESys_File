using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ESys.Application.Services.FileHandler;
using ESys.Persistence.FileSystem;

namespace ESys.Persistence;
/// <summary>
/// Extension methods needed for adding persistence layer services
/// </summary>
public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUploadHandlerService, UploadHandlerService>();
        return services;
    }

}

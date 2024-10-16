using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ESys.Application.Services.FileHandler;

namespace ESys.Persistence;
/// <summary>
/// Extension methods needed for adding persistence layer services
/// </summary>
public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUploadHandlerService, IUploadHandlerService>();
        return services;
    }

}

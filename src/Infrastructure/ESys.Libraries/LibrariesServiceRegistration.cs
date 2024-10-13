using ESys.Application.Contracts.Libraries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ESys.Libraries;

public static class LibrariesServiceRegistration
{
    public static IServiceCollection AddLibrariesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJsonHelper, IJsonHelper>();
        return services;
    }

}

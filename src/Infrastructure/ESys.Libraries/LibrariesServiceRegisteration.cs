using ESys.Application.Contracts.Libraries;
using ESys.Libraries.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ESys.Libraries;

public static class LibrariesServiceRegisteration
{
    public static IServiceCollection AddLibrariesServices(this IServiceCollection services)
    {
        services.AddScoped<IJsonHelper, JsonHelper>();
        services.AddScoped<IExpHelper, ExpHelper>();

        return services;
    }
}
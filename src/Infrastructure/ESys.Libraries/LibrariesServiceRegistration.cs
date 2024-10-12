using ESys.Application.Contracts.Libraries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ESys.Application.Contracts.Persistence;
using ESys.Application.Features.CalcForm.Queries.GetCalcFormInitialData;

namespace ESys.Libraries;

public static class LibrariesServiceRegistration
{
    public static IServiceCollection AddLibrariesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJsonHelper, IJsonHelper>();
        return services;
    }

}

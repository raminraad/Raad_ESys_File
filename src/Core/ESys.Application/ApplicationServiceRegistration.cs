using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ESys.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        return services;
    }
}


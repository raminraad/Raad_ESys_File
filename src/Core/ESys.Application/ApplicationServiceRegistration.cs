using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ESys.Application;

/// <summary>
/// Extension methods needed for adding application layer services
/// </summary>
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        return services;
    }
}


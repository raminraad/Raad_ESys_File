using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ESys.Application.Contracts.Persistence;
using ESys.Application.Features.BizForm;

namespace ESys.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        services.AddScoped<BizCalculator>();
        services.AddScoped<BizInitiator>();

        return services;
    }
}


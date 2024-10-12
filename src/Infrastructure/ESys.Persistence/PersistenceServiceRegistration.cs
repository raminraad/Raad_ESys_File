﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ESys.Application.Contracts.Persistence;
using ESys.Persistence.Dapper.Repositories;
using ESys.Application.Features;
using ESys.Application.Features.CalcForm.Queries.GetCalcFormInitialData;

namespace ESys.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBizRepository, BizRepository>();
        services.AddScoped<IExpressionRepository, ExpressionRepository>();
        services.AddScoped<GetCalcFormInitialDataQueryHandler, GetCalcFormInitialDataQueryHandler>();
        return services;
    }

}

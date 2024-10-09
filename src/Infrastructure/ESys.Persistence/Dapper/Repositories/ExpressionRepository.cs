using System.Data.SqlClient;
using System.Numerics;
using Dapper;
using ESys.Application.Contracts.Persistence;
using ESys.Domain.Entities;
using ESys.Persistence.Statics.SqlServerStatics;
using Microsoft.Extensions.Configuration;

namespace ESys.Persistence.Dapper.Repositories;
public class ExpressionRepository : IExpressionRepository
{

    private readonly IConfiguration _configuration;
    protected readonly string _connectionString = string.Empty;

    public ExpressionRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("EsysSqlServerConnectionString") ?? throw new ArgumentNullException();
    }
    public Task<Expression> AddAsync(Expression entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Expression entity)
    {
        throw new NotImplementedException();
    }

    public Task<Expression> GetExpressionWithXmls(string ExpressionId)
    {
        throw new NotImplementedException();
    }

    public Task<Expression> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Expression> GetByIdAsync(BigInteger id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Expression entity)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Expression>> ListAllAsync()
    {
        throw new NotImplementedException();
    }
}

using System.Data.SqlClient;
using System.Numerics;
using Dapper;
using ESys.Application.Contracts.Persistence;
using ESys.Domain.Entities;
using ESys.Persistence.Statics.SqlServerStatics;
using Microsoft.Extensions.Configuration;

namespace ESys.Persistence.Dapper.Repositories;
public class BizRepository : IBizRepository
{

    private readonly IConfiguration _configuration;
    protected readonly string _connectionString = string.Empty;

    public BizRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("EsysSqlServerConnectionString") ?? throw new ArgumentNullException();
    }
    public Task<Biz> AddAsync(Biz entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Biz entity)
    {
        throw new NotImplementedException();
    }

    public Task<Biz> GetBizWithXmls(string bizId)
    {
        throw new NotImplementedException();
    }

    public Task<Biz> GetByIdAsync(string id)
    {
        var idAsBigInteger = BigInteger.Parse(id);
        return GetByIdAsync(idAsBigInteger);
    }

    public async Task<Biz> GetByIdAsync(BigInteger id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string queryStatement = $"SELECT TOP 1 *  FROM {SqlServerStatics.BizTable} WHERE BizId = '{id}'";
            return await connection.QueryFirstOrDefaultAsync<Biz>(queryStatement);
        }
    }

    public async Task<IReadOnlyList<Biz>> ListAllAsync()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var Bizs = await connection.QueryAsync<Biz>($"SELECT * FROM {SqlServerStatics.BizTable}");
            return Bizs.ToList();
        }
    }

    public Task UpdateAsync(Biz entity)
    {
        throw new NotImplementedException();
    }
}

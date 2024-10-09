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
        throw new NotImplementedException();
    }

    public Task<Biz> GetByIdAsync(BigInteger id)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<Biz>> ListAllAsync()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var Bizs = await connection.QueryAsync<Biz>($"SELECT * FROM {SqlServerStatics.bizTable}");
            return Bizs.ToList();
        }
    }

    public Biz? LoadBizFromDB(string bizID)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string queryStatement = "SELECT Exp, Func, lookup  FROM " + SqlServerStatics.bizTable + " WHERE BizId = '" + bizID + "'";//queryBuilder(lookupStr);
            return connection.QueryFirst<Biz>(queryStatement);
        }
    }

    public Task UpdateAsync(Biz entity)
    {
        throw new NotImplementedException();
    }
}

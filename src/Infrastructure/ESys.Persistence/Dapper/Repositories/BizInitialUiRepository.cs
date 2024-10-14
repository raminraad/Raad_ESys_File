using System.Data.SqlClient;
using System.Numerics;
using Dapper;
using ESys.Application.Contracts.Persistence;
using ESys.Domain.Entities;
using ESys.Persistence.Statics.SqlServerStatics;
using Microsoft.Extensions.Configuration;

namespace ESys.Persistence.Dapper.Repositories;
public class BizInitialUiRepository : IBizInitialUiRepository
{

    private readonly IConfiguration _configuration;
    protected readonly string _connectionString = string.Empty;

    public BizInitialUiRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("EsysSqlServerConnectionString") ?? throw new ArgumentNullException();
    }
    public async Task<BizInitialUi> AddAsync(BizInitialUi entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(BizInitialUi entity)
    {
        throw new NotImplementedException();
    }

    public async Task<BizInitialUi> BizInitialUiressionWithXmls(string BizInitialUiressionId)
    {
        throw new NotImplementedException();
    }

    public async Task<BizInitialUi> GetByIdAsync(string id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string queryStatement = "SELECT TOP 1 *  FROM " + SqlServerStatics.BizInitialUiTable + " WHERE BizId = '" + id + "'";
            return await connection.QueryFirstOrDefaultAsync<BizInitialUi>(queryStatement);
        }
    }

    public async Task<BizInitialUi> GetByIdAsync(BigInteger id)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(BizInitialUi entity)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<BizInitialUi>> ListAllAsync()
    {
        throw new NotImplementedException();
    }
}

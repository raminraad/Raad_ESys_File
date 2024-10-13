using System.Data.SqlClient;
using System.Numerics;
using Dapper;
using ESys.Application.Contracts.Persistence;
using ESys.Domain.Entities;
using ESys.Persistence.Statics.SqlServerStatics;
using Microsoft.Extensions.Configuration;

namespace ESys.Persistence.Dapper.Repositories;

public class BizXmlRepository : IBizXmlRepository
{
    private readonly IConfiguration _configuration;
    protected readonly string _connectionString = string.Empty;

    public BizXmlRepository(IConfiguration configuration)
    {
        //todo: move cs to AsyncRepository
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("EsysSqlServerConnectionString") ??
                            throw new ArgumentNullException();
    }

    public Dictionary<string, string> RequestDBforXML(string bizId)
    {
        throw new NotImplementedException();
    }

    public Task<Exp> AddAsync(Exp entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Exp entity)
    {
        throw new NotImplementedException();
    }

    public Task<Exp> GetExpressionWithXmls(string ExpressionId)
    {
        throw new NotImplementedException();
    }

    public Task<Exp> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Exp> GetByIdAsync(BigInteger id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Exp entity)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Exp>> ListAllAsync()
    {
        throw new NotImplementedException();
    }

    Task<BizXml> IAsyncRepository<BizXml>.GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    Task<BizXml> IAsyncRepository<BizXml>.GetByIdAsync(BigInteger id)
    {
        throw new NotImplementedException();
    }

    Task<IReadOnlyList<BizXml>> IAsyncRepository<BizXml>.ListAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BizXml> AddAsync(BizXml entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(BizXml entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(BizXml entity)
    {
        throw new NotImplementedException();
    }


    public Dictionary<string, string> GetBizXmlAsDictionary(Biz biz, Dictionary<string, string> lookupStr)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
        BizXml bizXml;
            string tableQuery = "SELECT xmlTitles, xmlTags, xmlColCount, whereClause FROM " +
                                SqlServerStatics.XmlTable + " WHERE BizId = '" + biz.BizId + "' and tname = '" +
                                lookupStr["table"] + "'"; //queryBuilder(lookupStr);
            bizXml = connection.QueryFirst<BizXml>(tableQuery);

            // This line was in original code. Ask what it is for.
            //string xmlColCount = "1";

            string innerQuery = GenerateQueryForLookup(lookupStr, bizXml);
            var innerQueryResult = connection.Query(innerQuery).FirstOrDefault();
            Dictionary<string, string> finalResult = new();
            if (innerQueryResult is not null)
            {
                foreach (var property in innerQueryResult)
                {
                    finalResult.Add(property.Key, property.Value.ToString());
                }
            }

            return finalResult;
        }
    }


    public string GenerateQueryForLookup(Dictionary<string, string> lookupDic, BizXml bizXml)
    {
        string query = @"SELECT * FROM ( SELECT" +
                       bizXml.XmlTitles + " FROM(select * FROM " +
                       SqlServerStatics.XmlTable +
                       " WHERE BizId = '" +
                       bizXml.BizId +
                       "' AND tname = '" +
                       lookupDic["table"] +
                       "') e OUTER APPLY e.xml.nodes('" +
                       bizXml.XmlTags +
                       "') as X(Y) )T ";

        lookupDic.Remove("table");

        if (lookupDic.Count() > 0)
        {
            if (bizXml.WhereClause != null)
            {
                string tmpWhere = bizXml.WhereClause;
                foreach (var item in lookupDic)
                {
                    tmpWhere = tmpWhere.Replace("__" + item.Key, item.Value);
                }

                query += tmpWhere;
            }
        }

        return query;
    }
}
using ESys.Domain.Entities;

namespace ESys.Application.Contracts.Persistence;
public interface IBizXmlRepository : IAsyncRepository<BizXml>
{
    public Dictionary<string, string> GetBizXmlAsDictionary(Biz biz,Dictionary<string, string> lookupDic);
    string GenerateQueryForLookup(Dictionary<string, string> lookupDic, BizXml bizXml);
}

using ESys.Domain.Entities;

namespace ESys.Application.Contracts.Persistence;
public interface IBizXmlRepository : IAsyncRepository<BizXmls>
{
    public Dictionary<string, string> RequestDBforXML(string bizId);
}

using ESys.Domain.Entities;

namespace ESys.Application.Contracts.Persistence;
public interface IBizRepository : IAsyncRepository<Biz>
{
    Task<Biz> GetBizWithXmls(string bizId);
}

using System.Numerics;

namespace ESys.Application.Contracts.Persistence;
public interface IAsyncRepository<T> where T : class
{
    Task<T> GetByIdAsync(string id);
    Task<T> GetByIdAsync(BigInteger id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

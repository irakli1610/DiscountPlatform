using System.Linq.Expressions;

namespace offers.itacademy.ge.Application.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token);
        Task<T?> GetByIdAsync(int id, CancellationToken token);
        Task<T> AddAsync(T entity, CancellationToken token);
        T Update(T entity);
        void Remove(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken token);

    }
}

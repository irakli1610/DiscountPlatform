using Microsoft.EntityFrameworkCore;
using offers.itacademy.ge.Application.Repositories;
using offers.itacademy.ge.Persistance.Context;
using System.Linq.Expressions;

namespace offers.itacademy.ge.Infrastructure
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DiscountPlatformContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(DiscountPlatformContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> AddAsync(T entity, CancellationToken token)
        {
            await _dbSet.AddAsync(entity, token);
            return entity;
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken token)
        {
            return await _dbSet.AnyAsync(predicate, token);
        }

        public async Task<List<T>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token)
        {
            return await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken token)
        {
            return await _dbSet.FindAsync(id, token);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public T Update(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

    }
}

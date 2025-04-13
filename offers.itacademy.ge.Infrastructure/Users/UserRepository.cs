using Microsoft.EntityFrameworkCore;
using offers.itacademy.ge.Application.Repositories;
using offers.itacademy.ge.Domain.Users;
using offers.itacademy.ge.Persistance.Context;
using System.Linq.Expressions;

namespace offers.itacademy.ge.Infrastructure.Users
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DiscountPlatformContext context) : base(context)
        {
        }

        //  Get Companies, Admins, Customers
        public async Task<List<TUserEntity>> GetUsersAsync<TUserEntity>(int pageNumber, int pageSize, CancellationToken token) where TUserEntity : User
        {
            return await _dbSet
                .OfType<TUserEntity>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
        }

        public async Task<User?> GetUserByUserName(Expression<Func<User, bool>> predicate, CancellationToken token)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate, token);
        }

        public async Task<User?> GetUserWithCategoriesAsync(int userId, CancellationToken token)
        {
            return await _dbSet
                .OfType<Customer>()
                .Include(c => c.SelectedCategories)
                .FirstOrDefaultAsync(c => c.Id == userId, token);
        }
    }
}

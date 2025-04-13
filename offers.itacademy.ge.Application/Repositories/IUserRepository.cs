using offers.itacademy.ge.Domain.Users;
using System.Linq.Expressions;

namespace offers.itacademy.ge.Application.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByUserName(Expression<Func<User, bool>> predicate, CancellationToken token);
        Task<User?> GetUserWithCategoriesAsync(int userId, CancellationToken token);
        Task<List<TUserEntity>> GetUsersAsync<TUserEntity>(int pageNumber, int pageSize, CancellationToken token) where TUserEntity : User;
    }
}

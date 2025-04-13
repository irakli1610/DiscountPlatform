using offers.itacademy.ge.Domain.Categories;
using offers.itacademy.ge.Application.Repositories;
using offers.itacademy.ge.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace offers.itacademy.ge.Infrastructure.Categories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DiscountPlatformContext context) : base(context)
        {

        }

        public Task<List<Category>> GetByIdsAsync(List<int> categoryIds, CancellationToken token)
        {
            return _dbSet.Where(c => categoryIds.Contains(c.Id)).ToListAsync(token); // SELECT * FROM Categories WHERE Id IN (1, 2, 3);
        }
    }
}

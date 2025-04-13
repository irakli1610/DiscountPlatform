using offers.itacademy.ge.Domain.Categories;

namespace offers.itacademy.ge.Application.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<List<Category>> GetByIdsAsync(List<int> categoryIds, CancellationToken token);
    }
}

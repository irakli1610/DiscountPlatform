using offers.itacademy.ge.Application.Models.Categories;

namespace offers.itacademy.ge.Application.Interfaces.ServiceInterfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseModel>> GetAllCategoriesAsync(int pageNumber, int pageSize, CancellationToken token);
        Task<CategoryResponseModel> GetCategoryByIdAsync(int id, CancellationToken token);
        Task<CategoryResponseModel> CreateCategoryAsync(CategoryRequestModel categoryModel, CancellationToken token);
        Task<CategoryResponseModel> UpdateCategoryAsync(int id, CategoryRequestModel updateRequestCategory, CancellationToken token);
        Task DeleteCategoryAsync(int id, CancellationToken token);
    }
}

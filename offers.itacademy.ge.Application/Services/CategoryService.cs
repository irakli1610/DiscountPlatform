using Mapster;
using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Exceptions.Resources;
using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Categories;
using offers.itacademy.ge.Application.Utils;
using offers.itacademy.ge.Domain.Categories;

namespace offers.itacademy.ge.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryResponseModel>> GetAllCategoriesAsync(int pageNumber, int pageSize, CancellationToken token)
        {
            ValidationUtils.ValidatePagination(pageNumber, pageSize);

            var categories = await _unitOfWork.Categories.GetAllAsync(pageNumber, pageSize, token);
            return categories.Adapt<List<CategoryResponseModel>>();
        }

        public async Task<CategoryResponseModel> GetCategoryByIdAsync(int id, CancellationToken token)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, token)
                ?? throw new NotFoundException(AppExceptionMessages.CategoryNotFound);

            return category.Adapt<CategoryResponseModel>();
        }

        public async Task<CategoryResponseModel> CreateCategoryAsync(CategoryRequestModel categoryModel, CancellationToken token)
        {
#pragma warning disable CA1862 // Use of the 'StringComparison' method throws exception becouse EF can't translate it to SQL query, so use ToLower() insted. 
            //Translation of the 'string.Equals' overload with a 'StringComparison' parameter is not supported. See https://go.microsoft.com/fwlink/?linkid=2129535 for more information.
            bool doesCategoryExist = await _unitOfWork.Categories.ExistsAsync(x => x.Name.ToLower() == categoryModel.Name.ToLower(), token);

            if (doesCategoryExist)
            {
                throw new ObjectAlreadyExistsException(AppExceptionMessages.CategoryAlreadyExists);
            }

            var category = categoryModel.Adapt<Category>();

            var createdCategory = await _unitOfWork.Categories.AddAsync(category, token);
            await _unitOfWork.SaveChangesAsync(token);

            return createdCategory.Adapt<CategoryResponseModel>();
        }

        public async Task<CategoryResponseModel> UpdateCategoryAsync(int id, CategoryRequestModel updateRequestCategory, CancellationToken token)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, token)
                ?? throw new NotFoundException(AppExceptionMessages.CategoryNotFound);

            //  Prevent renaming a category to an already existing name
            bool nameExists = await _unitOfWork.Categories.ExistsAsync(c => c.Name.ToLower() == updateRequestCategory.Name.ToLower() && c.Id != id, token);
            if (nameExists)
            {
                throw new ObjectAlreadyExistsException(AppExceptionMessages.CategoryAlreadyExists);
            }

            category.Name = updateRequestCategory.Name;

            var updatedCategoryResult = _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync(token);

            return updatedCategoryResult.Adapt<CategoryResponseModel>();
        }

        public async Task DeleteCategoryAsync(int id, CancellationToken token)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, token)
                ?? throw new NotFoundException(AppExceptionMessages.CategoryNotFound);

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}


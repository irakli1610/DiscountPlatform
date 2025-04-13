using FluentValidation;
using offers.itacademy.ge.Application.Models.Categories;
using offers.itacademy.ge.Application.Validations.Resources;

namespace offers.itacademy.ge.Application.Validations
{
    public class CategoryRequestModelValidation : AbstractValidator<CategoryRequestModel>
    {
        public CategoryRequestModelValidation() 
        {
            RuleFor(x =>x.Name).NotEmpty().WithMessage(ValidationMessages.CategoryNameEmpty)
                .MaximumLength(100).WithMessage(ValidationMessages.MaximumLengthForCategoryName);
        }
        
    }
}

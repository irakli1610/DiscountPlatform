using FluentValidation;
using offers.itacademy.ge.Application.Models.Users.Company;
using offers.itacademy.ge.Application.Validations.Resources;

namespace offers.itacademy.ge.Application.Validations.Users.Companies
{
    public class CompanyRequestUpdateModelValidation : AbstractValidator<CompanyRequestUpdateModel>
    {
        public CompanyRequestUpdateModelValidation()
        {
            Include(new UserRequestModelValidation());

            RuleFor(x => x.Balance).NotEmpty().WithMessage(ValidationMessages.BalanceIsRequired)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.BalanceIsRequiredCanNotBeNegative);
        }
    }
}

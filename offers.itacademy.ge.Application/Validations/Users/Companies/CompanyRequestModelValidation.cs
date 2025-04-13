using FluentValidation;
using offers.itacademy.ge.Application.Models.Users.Company;
using offers.itacademy.ge.Application.Validations.Resources;

namespace offers.itacademy.ge.Application.Validations.Users.Companies
{
    public class CompanyRequestModelValidation : AbstractValidator<CompanyRequestModel>
    {
        public CompanyRequestModelValidation()
        {
            Include(new PasswordRequestModelValidation());

            RuleFor(x => x.Balance).NotEmpty().WithMessage(ValidationMessages.BalanceIsRequired)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.BalanceIsRequiredCanNotBeNegative);

        }
    }
}

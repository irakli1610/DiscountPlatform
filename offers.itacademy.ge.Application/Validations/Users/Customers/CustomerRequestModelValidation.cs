using FluentValidation;
using offers.itacademy.ge.Application.Models.Users.Customer;
using offers.itacademy.ge.Application.Validations.Resources;

namespace offers.itacademy.ge.Application.Validations.Users.Customers
{
    public class CustomerRequestModelValidation : AbstractValidator<CustomerRequestModel>
    {
        public CustomerRequestModelValidation()
        {
            Include(new PasswordRequestModelValidation());

            RuleFor(x => x.Balance).NotEmpty().WithMessage(ValidationMessages.BalanceIsRequired)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.BalanceIsRequiredCanNotBeNegative);

        }
    }
}

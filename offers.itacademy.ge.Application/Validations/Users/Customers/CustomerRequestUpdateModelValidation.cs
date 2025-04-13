using FluentValidation;
using offers.itacademy.ge.Application.Models.Users.Customer;
using offers.itacademy.ge.Application.Validations.Resources;

namespace offers.itacademy.ge.Application.Validations.Users.Customers
{
    public class CustomerRequestUpdateModelValidation : AbstractValidator<CustomerRequestUpdateModel>
    {
        public CustomerRequestUpdateModelValidation()
        {
            Include(new UserRequestModelValidation());

            RuleFor(x => x.Balance).NotEmpty().WithMessage(ValidationMessages.BalanceIsRequired)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.BalanceIsRequiredCanNotBeNegative);

        }
    }
}

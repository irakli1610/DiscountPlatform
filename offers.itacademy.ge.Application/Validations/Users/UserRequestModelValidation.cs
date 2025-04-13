using FluentValidation;
using offers.itacademy.ge.Application.Models.Users;
using offers.itacademy.ge.Application.Validations.Resources;

namespace offers.itacademy.ge.Application.Validations.Users
{
    public class UserRequestModelValidation : AbstractValidator<UserRequestModel>
    {
        public UserRequestModelValidation() 
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(ValidationMessages.UserNameRequired)
                .MaximumLength(50).WithMessage(ValidationMessages.MaximumLengthForUserName);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ValidationMessages.EmailRequired)
                .EmailAddress().WithMessage(ValidationMessages.InvalidEmailFormat);

        }
    }
}

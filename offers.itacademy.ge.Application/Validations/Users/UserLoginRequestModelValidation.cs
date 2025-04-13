using FluentValidation;
using offers.itacademy.ge.Application.Models.Users;
using offers.itacademy.ge.Application.Validations.Resources;

namespace offers.itacademy.ge.Application.Validations.Users
{
    public class UserLoginRequestModelValidation : AbstractValidator<UserLoginRequestModel>
    {
        public UserLoginRequestModelValidation()
        {
            RuleFor(x=>x.UserName).NotEmpty().WithMessage(ValidationMessages.UserNameRequired)
                .MaximumLength(50).WithMessage(ValidationMessages.MaximumLengthForUserName);

            RuleFor(x=>x.Password).NotEmpty().WithMessage(ValidationMessages.PasswordRequired)
                .MaximumLength(50).WithMessage(ValidationMessages.MaximumLengthForPassword);
        }
    }
}

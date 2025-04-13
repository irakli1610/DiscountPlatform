using FluentValidation;
using offers.itacademy.ge.Application.Models.Users;
using offers.itacademy.ge.Application.Validations.Resources;

namespace offers.itacademy.ge.Application.Validations.Users
{
    public class PasswordRequestModelValidation : AbstractValidator<PasswordRequestModel>
    {
        public PasswordRequestModelValidation()
        {
            Include(new UserRequestModelValidation());

            RuleFor(x => x.Password)
               .NotEmpty().WithMessage(ValidationMessages.PasswordRequired)
               .MinimumLength(6).WithMessage(ValidationMessages.MinimumpasswordLengts);

            RuleFor(x => x.ConfirmPassword)
                    .NotEmpty().WithMessage(ValidationMessages.ConfirmPasswordRequired)
                    .Equal(x => x.Password).WithMessage(ValidationMessages.PasswordsDoNotMatch);
        }
       
    }
}

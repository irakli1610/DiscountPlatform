using FluentValidation;
using offers.itacademy.ge.Application.Models.Users.Admin;

namespace offers.itacademy.ge.Application.Validations.Users.Admins
{
    public class AdminRequestModelValidation : AbstractValidator<AdminRequestModel>
    {
        public AdminRequestModelValidation()
        {
            Include(new PasswordRequestModelValidation());
        }
    }
}

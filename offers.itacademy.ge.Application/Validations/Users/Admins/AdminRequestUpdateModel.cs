using FluentValidation;
using offers.itacademy.ge.Application.Models.Users.Admin;

namespace offers.itacademy.ge.Application.Validations.Users.Admins
{
    public class AdminRequestUpdateModelValidation : AbstractValidator<AdminRequestUpdateModel>
    {
        public AdminRequestUpdateModelValidation()
        {
            Include(new UserRequestModelValidation());
        }
    }
}

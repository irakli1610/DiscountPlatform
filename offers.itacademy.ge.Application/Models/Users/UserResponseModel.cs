using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Application.Models.Users
{
    public class UserResponseModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}

namespace offers.itacademy.ge.Application.Models.Users
{
    public class PasswordRequestModel : UserRequestModel
    {
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

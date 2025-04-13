namespace offers.itacademy.ge.Application.Models.Users.Company
{
    public class CompanyResponseModel : UserResponseModel
    {
        public bool IsActivated { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Balance { get; set; }

    }
}

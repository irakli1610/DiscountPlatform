using offers.itacademy.ge.Domain.ProductOffers;

namespace offers.itacademy.ge.Domain.Users
{
    public class Company : User
    {
        public bool IsActivated { get; set; } = false;
        public string? ImageUrl { get; set; }
        public List<ProductOffer> ProductOffers { get; set; } = [];
        public decimal Balance { get; set; } = 0;
    }
}

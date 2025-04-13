using offers.itacademy.ge.Domain.Categories;
using offers.itacademy.ge.Domain.Purchases;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Domain.ProductOffers
{
    public class ProductOffer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public OfferStatus Status { get; set; }
        public List<Purchase> Purchases { get; set; } = [];
    }
}

using offers.itacademy.ge.Application.Models.Categories;
using offers.itacademy.ge.Application.Models.Users.Company;
using offers.itacademy.ge.Domain.ProductOffers;

namespace offers.itacademy.ge.Application.Models.ProductOffers
{
    public class ProductOfferResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public int CategoryId { get; set; }
        public CategoryResponseModel Category { get; set; }

        public int CompanyId { get; set; }
        public CompanyResponseModel Company { get; set; }
        public OfferStatus Status { get; set; }
        public string? ImageUrl { get; set; }
    }
}

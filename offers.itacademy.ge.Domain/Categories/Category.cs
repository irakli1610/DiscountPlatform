using offers.itacademy.ge.Domain.ProductOffers;

namespace offers.itacademy.ge.Domain.Categories
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ProductOffer> ProductOffers { get; set; } = [];
    }
}

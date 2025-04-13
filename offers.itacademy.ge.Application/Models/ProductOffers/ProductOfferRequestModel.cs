namespace offers.itacademy.ge.Application.Models.ProductOffers
{
    public class ProductOfferRequestModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpirationTime { get; set; }
        public int CategoryId { get; set; }

    }
}

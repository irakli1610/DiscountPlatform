using offers.itacademy.ge.Domain.ProductOffers;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Domain.Purchases
{
    public class Purchase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int? ProductOfferId { get; set; }
        public ProductOffer? ProductOffer { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public PurchaseStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

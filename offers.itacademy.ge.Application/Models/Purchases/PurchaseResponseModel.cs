using offers.itacademy.ge.Application.Models.ProductOffers;
using offers.itacademy.ge.Application.Models.Users.Customer;
using offers.itacademy.ge.Domain.Purchases;

namespace offers.itacademy.ge.Application.Models.Purchases
{
    public class PurchaseResponseModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public CustomerResponseModel Customer { get; set; }
        public int ProductOfferId { get; set; }
        public ProductOfferResponseModel ProductOffer { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public PurchaseStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

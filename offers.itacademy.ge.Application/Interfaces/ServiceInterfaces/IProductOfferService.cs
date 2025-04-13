using offers.itacademy.ge.Application.Models.Images;
using offers.itacademy.ge.Application.Models.ProductOffers;

namespace offers.itacademy.ge.Application.Interfaces.ServiceInterfaces
{
    public interface IProductOfferService
    {
        Task<List<ProductOfferResponseModel>> GetAllOffersAsync(int pageNumber, int pageSize, CancellationToken token);
        Task<List<ProductOfferResponseModel>> GetAllActiveOffersAsync(int pageNumber, int pageSize, CancellationToken token);
        Task<ProductOfferResponseModel> GetOfferByIdAsync(int id, CancellationToken token);
        Task<ProductOfferResponseModel> CreateOfferAsync(ProductOfferRequestModel offer, int companyId, CancellationToken token);
        Task DeleteOfferAsync(int offerId, int companyId, CancellationToken token);
        Task<ProductOfferResponseModel> UpdateOfferAsync(int productOfferId, ProductOfferRequestModel updatedOffer, int companyId, CancellationToken token);
        Task<bool> CancelOfferAsync(int offerId, int companyId, CancellationToken token);
        Task<List<ProductOfferResponseModel>> GetUserRelevantOffersAsync(int pageNumber, int pageSize, int userId, CancellationToken token);
        Task<string> UploadProductOfferImage(int companyId, int productOfferId, ImageRequestModel fileForm, CancellationToken token);
        Task<List<ProductOfferResponseModel>> GetCompanyOffersAsync(int pageNumber, int pageSize, int companyId, CancellationToken token);                 
    }
}

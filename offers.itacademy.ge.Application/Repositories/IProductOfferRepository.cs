using offers.itacademy.ge.Domain.Categories;
using offers.itacademy.ge.Domain.ProductOffers;

namespace offers.itacademy.ge.Application.Repositories
{
    public interface IProductOfferRepository : IBaseRepository<ProductOffer>
    {
        Task<List<ProductOffer>> GetActiveOffersAsync(int pageNumber, int pageSize, CancellationToken token);
        Task<ProductOffer?> GetCancellableOfferAsync(int offerId, int companyId, CancellationToken token);
        Task<List<ProductOffer>> GetActiveOffersByCategoriesAsync(int pageNumber, int pageSize, List<Category> selectedCategories, CancellationToken token);
        Task<List<ProductOffer>> GetCompanyOffersAsync(int pageNumber, int pageSize, int companyId, CancellationToken token);
        Task<List<ProductOffer>> GetExpiredOffersAsync(CancellationToken token);
        Task<List<ProductOffer>> GetAllAsyncIncludeDetails(int pageNumber, int pageSize, CancellationToken token);
        Task<ProductOffer?> GetByIdAsyncIncludingDetails(int id, CancellationToken token);
    }
}

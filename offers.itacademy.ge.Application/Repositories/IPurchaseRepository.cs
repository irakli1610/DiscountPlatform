using offers.itacademy.ge.Domain.Purchases;

namespace offers.itacademy.ge.Application.Repositories
{
    public interface IPurchaseRepository : IBaseRepository<Purchase>
    {
        Task<List<Purchase>> GetUserPurchasesAsync(int pageNumber, int pageSize, int userId, CancellationToken token);
        Task<List<Purchase>> GetOfferPurchasesAsync(int pageNumber, int pageSize, int offerId, CancellationToken token);
        Task<Purchase?> GetPurchaseWithDetailsAsync(int purchaseId, CancellationToken token);


    }
}

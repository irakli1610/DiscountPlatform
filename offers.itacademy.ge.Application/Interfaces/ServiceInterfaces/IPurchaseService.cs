using offers.itacademy.ge.Application.Models.Purchases;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Application.Interfaces.ServiceInterfaces
{
    public interface IPurchaseService
    {
        Task<PurchaseResponseModel> PurchaseOfferAsync(int customerId, int offerId, int quantity, CancellationToken token);
        Task<bool> CancelPurchaseAsync(int purchaseId, int customerId, CancellationToken token);
        Task<List<PurchaseResponseModel>> GetUserPurchasesAsync(int pageNumber, int pageSize, int userId, CancellationToken token);
        Task<List<PurchaseResponseModel>> GetOfferPurchasesAsync(int pageNumber, int pageSize, int offerId, int requestingUserId, UserRole requestingUserRole, CancellationToken token);
        Task<PurchaseResponseModel> GetPurchaseByIdAsync(int purchaseId, int customerId, CancellationToken token);
   }
}

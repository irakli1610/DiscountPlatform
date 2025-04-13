using Microsoft.EntityFrameworkCore;
using offers.itacademy.ge.Application.Repositories;
using offers.itacademy.ge.Domain.Purchases;
using offers.itacademy.ge.Persistance.Context;

namespace offers.itacademy.ge.Infrastructure.Purchases
{
    public class PurchaseRepository : BaseRepository<Purchase>, IPurchaseRepository
    {
        public PurchaseRepository(DiscountPlatformContext context) : base(context)
        {

        }

        public async Task<List<Purchase>> GetUserPurchasesAsync(int pageNumber, int pageSize, int userId, CancellationToken token)
        {
            return await _dbSet
                .Where(p => p.CustomerId == userId || p.ProductOffer!.CompanyId == userId) //  Works for both Customers & Companies
                .Include(p => p.ProductOffer)
                .ThenInclude(po => po!.Company)
                .Include(c => c.Customer)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
        }

        public async Task<Purchase?> GetPurchaseWithDetailsAsync(int purchaseId, CancellationToken token)
        {
            return await _dbSet
                .Include(p => p.ProductOffer)
                .ThenInclude(o => o!.Company)
                .FirstOrDefaultAsync(p => p.Id == purchaseId, token);
        }

        public async Task<List<Purchase>> GetOfferPurchasesAsync(int pageNumber, int pageSize, int offerId, CancellationToken token)
        {
            return await _dbSet
                .Where(p => p.ProductOfferId == offerId)
                .Include(p => p.Customer)
                 .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
        }
    }
}

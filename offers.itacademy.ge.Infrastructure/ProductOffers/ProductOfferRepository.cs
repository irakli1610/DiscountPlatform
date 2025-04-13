using Microsoft.EntityFrameworkCore;
using offers.itacademy.ge.Application.Repositories;
using offers.itacademy.ge.Domain.Categories;
using offers.itacademy.ge.Domain.ProductOffers;
using offers.itacademy.ge.Persistance.Context;

namespace offers.itacademy.ge.Infrastructure.ProductOffers
{
    public class ProductOfferRepository : BaseRepository<ProductOffer>, IProductOfferRepository
    {
        public ProductOfferRepository(DiscountPlatformContext context) : base(context)
        {

        }

        public async Task<ProductOffer?> GetCancellableOfferAsync(int offerId, int companyId, CancellationToken token)
        {
            return await _dbSet
                .Where(o => o.Id == offerId && o.CompanyId == companyId && o.Status == OfferStatus.Active)
                .Include(o => o.Purchases)
                .ThenInclude(p => p.Customer)
                .Include(o => o.Company)

                .FirstOrDefaultAsync(token);
        }

        public Task<List<ProductOffer>> GetActiveOffersByCategoriesAsync(int pageNumber, int pageSize, List<Category> selectedCategories, CancellationToken token)
        {
            var categoryIds = selectedCategories.Select(c => c.Id).ToList();
            return _dbSet
                .Where(o => o.Status == OfferStatus.Active && categoryIds.Contains(o.CategoryId))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.Company) // Eager load Company
                .Include(o => o.Category) // Eager load Category
                .ToListAsync(token);

        }

        public async Task<List<ProductOffer>> GetActiveOffersAsync(int pageNumber, int pageSize, CancellationToken token)
        {
            return await _dbSet
                .Where(x => x.Status == OfferStatus.Active)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.Company) // Eager load Company
                .Include(x => x.Category) // Eager load Category
                .ToListAsync(token);
        }

        public async Task<List<ProductOffer>> GetCompanyOffersAsync(int pageNumber, int pageSize, int companyId, CancellationToken token)
        {
            return await _dbSet
                .Where(x => x.CompanyId == companyId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.Category) // Eager load Category
                .Include(x => x.Company)
                .ToListAsync(token);
        }


        public async Task<List<ProductOffer>> GetExpiredOffersAsync(CancellationToken token)
        {
            return await _dbSet
                .Where(o => o.ExpirationTime <= DateTime.UtcNow && o.Status == OfferStatus.Active)
                .Include(o => o.Company) // Eager load Company
                .ToListAsync(token);
        }

        public async Task<List<ProductOffer>> GetAllAsyncIncludeDetails(int pageNumber, int pageSize, CancellationToken token)
        {
            return await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.Category) // Eager load Category
                .Include(x => x.Company)
                .ToListAsync(token);
        }

        public async Task<ProductOffer?> GetByIdAsyncIncludingDetails(int id, CancellationToken token)
        {
            return await _dbSet
                .Where(x => x.Id == id)
                .Include(x => x.Category) // Eager load Category
                .Include(x => x.Company)
                .FirstOrDefaultAsync(token);
        }
    }
}

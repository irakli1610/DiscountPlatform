using Microsoft.EntityFrameworkCore.Storage;
using offers.itacademy.ge.Application.Repositories;
using System.Data;
namespace offers.itacademy.ge.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IProductOfferRepository ProductOffers { get; }
        ICategoryRepository Categories { get; }
        IPurchaseRepository Purchases { get; }

        Task<int> SaveChangesAsync(CancellationToken token);
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken token = default);

        Task CommitTransactionAsync(CancellationToken token = default);

        Task RollbackTransactionAsync(CancellationToken token = default);


    }

}

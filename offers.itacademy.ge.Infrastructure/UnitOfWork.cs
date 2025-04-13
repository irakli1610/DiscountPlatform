using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Application.Repositories;
using offers.itacademy.ge.Persistance.Context;
using System.Data;

namespace offers.itacademy.ge.Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DiscountPlatformContext _context;
        private IDbContextTransaction _currentTransaction;

        public IUserRepository Users { get; }
        public IProductOfferRepository ProductOffers { get; }
        public IPurchaseRepository Purchases { get; }
        public ICategoryRepository Categories { get; }

        public UnitOfWork(DiscountPlatformContext context,
                        IUserRepository userRepository,
                        IProductOfferRepository productOfferRepository,
                        IPurchaseRepository purchaseRepository,
                        ICategoryRepository categoryRepository)
        {
            _context = context;
            Users = userRepository;
            ProductOffers = productOfferRepository;
            Purchases = purchaseRepository;
            Categories = categoryRepository;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken token = default)
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress");
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync(isolationLevel, token);
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(CancellationToken token = default)
        {
            try
            {
                if (_currentTransaction == null)
                {
                    throw new InvalidOperationException("No transaction to commit");
                }

                await _context.SaveChangesAsync(token);
                await _currentTransaction.CommitAsync(token);
            }
            catch
            {
                await RollbackTransactionAsync(token);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken token = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync(token);
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken token)
        {
            return await _context.SaveChangesAsync(token);
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
        }

        public bool HasActiveTransaction => _currentTransaction != null;
    }
}

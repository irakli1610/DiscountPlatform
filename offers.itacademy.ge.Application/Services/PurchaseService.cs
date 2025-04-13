using Mapster;
using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Exceptions.Resources;
using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Purchases;
using offers.itacademy.ge.Application.Utils;
using offers.itacademy.ge.Domain.Purchases;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PurchaseResponseModel> PurchaseOfferAsync(int customerId, int offerId, int quantity, CancellationToken token)
        {
            var customer = await _unitOfWork.Users.GetByIdAsync(customerId, token) as Customer;
            var offer = await _unitOfWork.ProductOffers.GetByIdAsync(offerId, token);
            
           
            ValidationUtils.ValidateOfferForPurchase(offer, quantity);

            var company = await _unitOfWork.Users.GetByIdAsync(offer!.CompanyId, token) as Company
               ?? throw new NotFoundException(AppExceptionMessages.CompanyNotFound);

            decimal totalCost = offer.Price * quantity;

            ValidationUtils.ValidateCustomerForPurchase(customer, totalCost);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(token:token);
            try
            {
                customer!.Balance -= totalCost;
                company.Balance += totalCost;
                offer.Quantity -= quantity;

                var purchase = new Purchase
                {
                    CustomerId = customerId,
                    ProductOfferId = offerId,
                    Quantity = quantity,
                    PurchaseDate = DateTime.UtcNow,
                    Status = PurchaseStatus.Active,
                    TotalPrice = totalCost
                };

                _unitOfWork.Users.Update(customer);
                _unitOfWork.Users.Update(company);
                _unitOfWork.ProductOffers.Update(offer);
                await _unitOfWork.Purchases.AddAsync(purchase, token);
                await _unitOfWork.SaveChangesAsync(token);

                await transaction.CommitAsync(token);
                return purchase.Adapt<PurchaseResponseModel>();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(token);
                throw new PurchaseException(ex.Message);
            }
        }

        public async Task<bool> CancelPurchaseAsync(int purchaseId, int customerId, CancellationToken token)
        {
            var purchase = await _unitOfWork.Purchases.GetPurchaseWithDetailsAsync(purchaseId, token);

            var customer = await _unitOfWork.Users.GetByIdAsync(customerId, token) as Customer
                ?? throw new NotFoundException(AppExceptionMessages.CustomerNotFound);

            ValidationUtils.ValidatePurchaseForCancellation(purchase, customerId);

            var company = purchase!.ProductOffer!.Company;

            await using var transaction = await _unitOfWork.BeginTransactionAsync(token: token);
            try
            {
                decimal refundAmount = purchase.TotalPrice;
                customer.Balance += refundAmount;
                company.Balance -= refundAmount;
                purchase.ProductOffer.Quantity += purchase.Quantity;
                purchase.Status = PurchaseStatus.Cancelled;

                _unitOfWork.Users.Update(customer);
                _unitOfWork.Users.Update(company);
                _unitOfWork.ProductOffers.Update(purchase.ProductOffer);
                _unitOfWork.Purchases.Update(purchase);
                await _unitOfWork.SaveChangesAsync(token);
                await transaction.CommitAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(token);
                throw new InvalidCancellationRequestException(ex.Message);
            }
        }

        public async Task<List<PurchaseResponseModel>> GetUserPurchasesAsync(int pageNumber, int pageSize, int userId, CancellationToken token)
        {
            ValidationUtils.ValidatePagination(pageNumber, pageSize);

            if (!await _unitOfWork.Users.ExistsAsync(x => x.Id == userId, token))
            {
                throw new NotFoundException(AppExceptionMessages.UserNotFound);
            }

            var userpurchases = await _unitOfWork.Purchases.GetUserPurchasesAsync(pageNumber, pageSize, userId, token);
            return userpurchases.Adapt<List<PurchaseResponseModel>>();
        }

        public async Task<List<PurchaseResponseModel>> GetOfferPurchasesAsync(int pageNumber, int pageSize, int offerId, int requestingUserId, UserRole requestingUserRole, CancellationToken token)
        {
            ValidationUtils.ValidatePagination(pageNumber, pageSize);

            var offer = await _unitOfWork.ProductOffers.GetByIdAsync(offerId, token)
                ?? throw new NotFoundException(AppExceptionMessages.OfferNotFound);

            //  Check authorization: Allow only Admins or the Company that owns the offer
            if (requestingUserRole != UserRole.Admin && offer.CompanyId != requestingUserId)
            {
                throw new UnauthorizedAccessException(AppExceptionMessages.UnauthorizedOfferAccess);
            }

            var offerPurchases = await _unitOfWork.Purchases.GetOfferPurchasesAsync(pageNumber, pageSize, offerId, token);
            return offerPurchases.Adapt<List<PurchaseResponseModel>>();
        }

        public async Task<PurchaseResponseModel> GetPurchaseByIdAsync(int purchaseId, int customerId, CancellationToken token)
        {
            var purchase = await _unitOfWork.Purchases.GetPurchaseWithDetailsAsync(purchaseId, token)
                ?? throw new NotFoundException(AppExceptionMessages.PurchaseNotFound);

            if (purchase.CustomerId != customerId)
                throw new UnauthorizedAccessException(AppExceptionMessages.UnauthorizedAccess);

            return purchase.Adapt<PurchaseResponseModel>();
        }
    }
}
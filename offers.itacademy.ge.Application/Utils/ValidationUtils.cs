using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Exceptions.Resources;
using offers.itacademy.ge.Domain.ProductOffers;
using offers.itacademy.ge.Domain.Purchases;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Application.Utils
{
    public static class ValidationUtils
    {

        //==================pagination validations=======================
        public static void ValidatePagination(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new PaginationException(AppExceptionMessages.PageNumberOrPageSizeMustBePositive);
            }
        }

        //==================productOffer validations=======================

        public static void ValidateProductOfferForUpdate(ProductOffer? productOffer, int companyId)
        {

            if (productOffer == null)
            {
                throw new NotFoundException(AppExceptionMessages.ProductNotFound);
            }

            if (productOffer.CompanyId != companyId)
                throw new UnauthorizedAccessException(AppExceptionMessages.UnauthorizedOfferModification);

            if (productOffer.Status != OfferStatus.Active)
                throw new InvalidOperationException(AppExceptionMessages.OfferNotActive);
        }

        public static void ValidateOfferForCancel(ProductOffer? productOffer)
        {
            if (productOffer == null)
            {
                throw new NotFoundException(AppExceptionMessages.OfferNotFound);
            }

            if ((DateTime.UtcNow - productOffer.CreateTime).TotalMinutes > 10)
            {
                throw new OfferCancellationException(AppExceptionMessages.OfferNotCancellable);
            }

            if (productOffer.Company == null || productOffer.Purchases == null)
            {
                throw new InvalidOperationException(AppExceptionMessages.InvalidPurchaseState);
            }
        }


        // ==================purchase validations=======================
        public static void ValidatePurchaseForCancellation(Purchase? purchase, int customerId)
        {
            if (purchase == null)
                throw new NotFoundException(AppExceptionMessages.PurchaseNotFound);

            if (purchase.CustomerId != customerId)
                throw new UnauthorizedAccessException(AppExceptionMessages.UnauthorizedAccess);

            if (purchase.ProductOffer == null || purchase.ProductOffer.Company == null)
                throw new InvalidCancellationRequestException(AppExceptionMessages.ProductMissesEssentialDetails);

            if ((DateTime.UtcNow - purchase.PurchaseDate).TotalMinutes > 5)
                throw new InvalidCancellationRequestException(AppExceptionMessages.CancellationWindowExpired);

            if(purchase.Status != PurchaseStatus.Active)
                throw new InvalidCancellationRequestException(AppExceptionMessages.PurchaseNotActive);
        }

        public static void ValidateOfferForPurchase(ProductOffer? offer, int quantity)
        {
            if (offer == null)
            {
                throw new NotFoundException(AppExceptionMessages.OfferNotFound);
            }

            if (offer.Quantity < quantity)
            {
                throw new PurchaseException(AppExceptionMessages.InsufficientStocks);
            }

            if (offer.Status != OfferStatus.Active)
                throw new PurchaseException(AppExceptionMessages.OfferisNotActiveAnymore);
        }

        public static void ValidateCustomerForPurchase(Customer? customer, decimal totalCost)
        {
            if (customer == null)
            {
                throw new NotFoundException(AppExceptionMessages.CustomerNotFound);
            }

            if (customer.Balance < totalCost)
                throw new PurchaseException(AppExceptionMessages.InsufficientFunds);

        }
    }
}

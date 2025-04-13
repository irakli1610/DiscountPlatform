using FluentAssertions;
using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Exceptions.Resources;
using offers.itacademy.ge.Application.Utils;
using offers.itacademy.ge.Domain.ProductOffers;
using offers.itacademy.ge.Domain.Purchases;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Application.Tests
{
    public class ValidationUtilsTests
    {
        #region ValidatePagination Tests

        [Theory]
        [InlineData(0, 10)]   // Invalid pageNumber
        [InlineData(-1, 10)]  // Negative pageNumber
        [InlineData(1, 0)]    // Invalid pageSize
        [InlineData(1, -5)]   // Negative pageSize
        public void ValidatePagination_ShouldThrow_WhenPageNumberOrSizeIsInvalid(int pageNumber, int pageSize)
        {
            var act = () => ValidationUtils.ValidatePagination(pageNumber, pageSize);

            act.Should().Throw<PaginationException>();
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 5)]
        [InlineData(100, 1)]
        public void ValidatePagination_ShouldNotThrow_WhenPageNumberAndSizeAreValid(int pageNumber, int pageSize)
        {
            var act = () => ValidationUtils.ValidatePagination(pageNumber, pageSize);

            act.Should().NotThrow();
        }

        #endregion

        #region ValidateOfferForPurchase Tests

        [Fact]
        public void ValidateOfferForPurchase_ShouldThrowNotFound_WhenOfferIsNull()
        {
            var act = () => ValidationUtils.ValidateOfferForPurchase(null, 1);

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void ValidateOfferForPurchase_ShouldThrowPurchaseException_WhenQuantityExceedsStock()
        {
            var offer = new ProductOffer { Quantity = 5, Status = OfferStatus.Active };

            var act = () => ValidationUtils.ValidateOfferForPurchase(offer, 10);

            act.Should().Throw<PurchaseException>();
        }

        [Theory]
        [InlineData(OfferStatus.Archived)]
        [InlineData(OfferStatus.Cancelled)]
        public void ValidateOfferForPurchase_ShouldThrowPurchaseException_WhenOfferNotActive(OfferStatus status)
        {
            var offer = new ProductOffer { Quantity = 10, Status = status };

            var act = () => ValidationUtils.ValidateOfferForPurchase(offer, 5);

            act.Should().Throw<PurchaseException>();
        }

        [Fact]
        public void ValidateOfferForPurchase_ShouldNotThrow_WhenValid()
        {
            var offer = new ProductOffer { Quantity = 10, Status = OfferStatus.Active };

            var act = () => ValidationUtils.ValidateOfferForPurchase(offer, 5);

            act.Should().NotThrow();
        }

        #endregion

        #region ValidateCustomerForPurchase Tests
        [Fact]
        public void ValidateCustomerForPurchase_ShouldThrowNotFound_WhenCustomerIsNull()
        {
            var act = () => ValidationUtils.ValidateCustomerForPurchase(null, 100m);

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void ValidateCustomerForPurchase_ShouldThrowPurchaseException_WhenInsufficientFunds()
        {
            var customer = new Customer { Balance = 50m };

            var act = () => ValidationUtils.ValidateCustomerForPurchase(customer, 100m);

            act.Should().Throw<PurchaseException>();
        }

        [Fact]
        public void ValidateCustomerForPurchase_ShouldNotThrow_WhenValid()
        {
            var customer = new Customer { Balance = 200m };

            var act = () => ValidationUtils.ValidateCustomerForPurchase(customer, 100m);

            act.Should().NotThrow();
        }

        #endregion

        #region ValidatePurchaseForCancellation Tests

        [Fact]
        public void ValidatePurchaseForCancellation_ShouldThrowNotFound_WhenPurchaseIsNull()
        {
            var act = () => ValidationUtils.ValidatePurchaseForCancellation(null, 1);

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void ValidatePurchaseForCancellation_ShouldThrowUnauthorized_WhenCustomerIdMismatch()
        {
            var purchase = new Purchase { CustomerId = 2 };

            var act = () => ValidationUtils.ValidatePurchaseForCancellation(purchase, 1);

            act.Should().Throw<UnauthorizedAccessException>();
        }

        [Theory]
        [InlineData(true, false)]  // Missing ProductOffer
        [InlineData(false, true)]  // Missing Company
        public void ValidatePurchaseForCancellation_ShouldThrowInvalidCancellation_WhenDetailsMissing(bool missingProductOffer, bool missingCompany)
        {
            var purchase = new Purchase
            {
                CustomerId = 1,
                ProductOffer = missingProductOffer ? null : new ProductOffer
                {
                    Company = missingCompany ? null : new Company()
                }
            };

            var act = () => ValidationUtils.ValidatePurchaseForCancellation(purchase, 1);

            act.Should().Throw<InvalidCancellationRequestException>();
        }

        [Fact]
        public void ValidatePurchaseForCancellation_ShouldThrowInvalidCancellation_WhenWindowExpired()
        {
            var purchase = new Purchase
            {
                CustomerId = 1,
                PurchaseDate = DateTime.UtcNow.AddMinutes(-10), // Beyond 5-minute window
                ProductOffer = new ProductOffer { Company = new Company() }
            };

            var act = () => ValidationUtils.ValidatePurchaseForCancellation(purchase, 1);

            act.Should().Throw<InvalidCancellationRequestException>();
        }

        [Fact]
        public void ValidatePurchaseForCancellation_ShouldThrowInvalidCancellation_WhenAlreadyCancelled()
        {
            var purchase = new Purchase
            {
                CustomerId = 1,
                ProductOffer = new ProductOffer { Company = new Company() },
                Status = PurchaseStatus.Cancelled
            };

            var act = () => ValidationUtils.ValidatePurchaseForCancellation(purchase, 1);

            act.Should().Throw<InvalidCancellationRequestException>();
        }

        [Fact]
        public void ValidatePurchaseForCancellation_ShouldNotThrow_WhenValid()
        {
            var purchase = new Purchase
            {
                CustomerId = 1,
                PurchaseDate = DateTime.UtcNow.AddMinutes(-3), // Within 5-minute window
                ProductOffer = new ProductOffer { Company = new Company() }
            };

            var act = () => ValidationUtils.ValidatePurchaseForCancellation(purchase, 1);

            act.Should().NotThrow();
        }

        #endregion

        #region ValidateProductOfferForUpdate tests

        [Fact]
        public void ValidateProductOfferForUpdate_ShouldPass_WhenOfferIsValid()
        {
            var offer = new ProductOffer { Id = 1, CompanyId = 1, Status = OfferStatus.Active };

            Action act = () => ValidationUtils.ValidateProductOfferForUpdate(offer, 1);

            act.Should().NotThrow();
        }

        [Fact]
        public void ValidateProductOfferForUpdate_ShouldThrowNotFound_WhenOfferIsNull()
        {
            Action act = () => ValidationUtils.ValidateProductOfferForUpdate(null, 1);

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void ValidateProductOfferForUpdate_ShouldThrowUnauthorized_WhenCompanyIdDoesNotMatch()
        {
            var offer = new ProductOffer { Id = 1, CompanyId = 2, Status = OfferStatus.Active };

            Action act = () => ValidationUtils.ValidateProductOfferForUpdate(offer, 1);

            act.Should().Throw<UnauthorizedAccessException>();
        }

        [Theory]
        [InlineData(OfferStatus.Cancelled)]
        [InlineData(OfferStatus.Archived)]
        public void ValidateProductOfferForUpdate_ShouldThrowInvalidOperation_WhenOfferIsNotActive(OfferStatus status)
        {
            var offer = new ProductOffer { Id = 1, CompanyId = 1, Status = status };

            Action act = () => ValidationUtils.ValidateProductOfferForUpdate(offer, 1);

            act.Should().Throw<InvalidOperationException>();
        }

        #endregion

        #region ValidateOfferForCancel tests
        [Fact]
        public void ValidateOfferForCancel_ShouldPass_WhenOfferIsValid()
        {
            var offer = new ProductOffer
            {
                Id = 1,
                CreateTime = DateTime.UtcNow.AddMinutes(-5),
                Company = new Company { Id = 1 },
                Purchases = new List<Purchase>()
            };

            Action act = () => ValidationUtils.ValidateOfferForCancel(offer);

            act.Should().NotThrow();
        }

        [Fact]
        public void ValidateOfferForCancel_ShouldThrowNotFound_WhenOfferIsNull()
        {
            Action act = () => ValidationUtils.ValidateOfferForCancel(null);

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void ValidateOfferForCancel_ShouldThrowCancellationException_WhenTimeLimitExceeded()
        {
            var offer = new ProductOffer
            {
                Id = 1,
                CreateTime = DateTime.UtcNow.AddMinutes(-15),
                Company = new Company { Id = 1 },
                Purchases = new List<Purchase>()
            };

            Action act = () => ValidationUtils.ValidateOfferForCancel(offer);

            act.Should().Throw<OfferCancellationException>();
        }

        [Fact]
        public void ValidateOfferForCancel_ShouldThrowInvalidOperation_WhenCompanyIsNull()
        {
            var offer = new ProductOffer
            {
                Id = 1,
                CreateTime = DateTime.UtcNow.AddMinutes(-5),
                Company = null,
                Purchases = new List<Purchase>()
            };

            Action act = () => ValidationUtils.ValidateOfferForCancel(offer);

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ValidateOfferForCancel_ShouldThrowInvalidOperation_WhenPurchasesIsNull()
        {
            var offer = new ProductOffer
            {
                Id = 1,
                CreateTime = DateTime.UtcNow.AddMinutes(-5),
                Company = new Company { Id = 1 },
                Purchases = null
            };

            Action act = () => ValidationUtils.ValidateOfferForCancel(offer);

            act.Should().Throw<InvalidOperationException>();
        }

        #endregion
    }
}
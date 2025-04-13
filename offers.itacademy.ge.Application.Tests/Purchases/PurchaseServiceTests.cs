using Castle.Core.Resource;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Application.Models.Purchases;
using offers.itacademy.ge.Application.Repositories;
using offers.itacademy.ge.Application.Services;
using offers.itacademy.ge.Domain.ProductOffers;
using offers.itacademy.ge.Domain.Purchases;
using offers.itacademy.ge.Domain.Users;
using System.Linq.Expressions;

namespace offers.itacademy.ge.Application.Tests.Purchases
{
    public class PurchaseServiceTests : IClassFixture<UnitOfWorkFixture>
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PurchaseService _purchaseService;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IProductOfferRepository> _productOfferRepositoryMock;
        private readonly Mock<IPurchaseRepository> _purchaseRepositoryMock;
        private readonly Mock<IDbContextTransaction> _transactionMock;

        public PurchaseServiceTests(UnitOfWorkFixture fixture)
        {
            _unitOfWorkMock = fixture.UnitOfWorkMock;
            _userRepositoryMock = fixture.UserRepositoryMock;
            _productOfferRepositoryMock = fixture.ProductOfferRepositoryMock;
            _purchaseRepositoryMock = fixture.PurchaseRepositoryMock;
            _transactionMock = new Mock<IDbContextTransaction>();

            // Setup repositories
            _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.ProductOffers).Returns(_productOfferRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Purchases).Returns(_purchaseRepositoryMock.Object);

            // Setup transaction mock
            _transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                            .Returns(Task.CompletedTask);
            _transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                            .Returns(Task.CompletedTask);
            _transactionMock.Setup(t => t.DisposeAsync())
                            .Returns(ValueTask.CompletedTask);

            // Mock BeginTransactionAsync to return transaction mock
            _unitOfWorkMock
                .Setup(u => u.BeginTransactionAsync(
                    It.IsAny<System.Data.IsolationLevel>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);

            _purchaseService = new PurchaseService(_unitOfWorkMock.Object);

            // Clear invocations
            _unitOfWorkMock.Invocations.Clear();
            _productOfferRepositoryMock.Invocations.Clear();
            _userRepositoryMock.Invocations.Clear();
            _purchaseRepositoryMock.Invocations.Clear();
        }


        #region 1 - PurchaseOfferAsync

        [Fact]
        public async Task PurchaseOfferAsync_ShouldThrowNotFound_WhenOfferNotFound()
        {
            _productOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(null as ProductOffer);

            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseService.PurchaseOfferAsync(1, 1, 1, CancellationToken.None));

            _productOfferRepositoryMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PurchaseOfferAsync_ShouldThrowNotFound_WhenCompanyNotFound()
        {
            var offer = new ProductOffer
            {
                Id = 1,
                Status = OfferStatus.Active,
                CompanyId = 2,
                Quantity = 10, 
                Price = 100  
            };

            _productOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(null as User);

            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseService.PurchaseOfferAsync(1, 1, 1, CancellationToken.None));

            _productOfferRepositoryMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PurchaseOfferAsync_ShouldThrowNotFound_WhenCustomerNotFound()
        {
            var offer = new ProductOffer { Id = 1, CompanyId = 2, Status = OfferStatus.Active, Price = 100, Quantity = 10 };
            var company = new Company { Id = 2 };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(null as Customer);
            _productOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(company);

            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseService.PurchaseOfferAsync(1, 1, 1, CancellationToken.None));

            _userRepositoryMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _productOfferRepositoryMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PurchaseOfferAsync_ShouldSucceed_WhenValid()
        {
            var customer = new Customer { Id = 1, Balance = 500 };
            var company = new Company { Id = 2, Balance = 0 };
            var offer = new ProductOffer
            {
                Id = 1,
                CompanyId = 2,
                Price = 100,
                Quantity = 10,
                Status = OfferStatus.Active
            };
            var purchase = new Purchase
            {
                Id = 1,
                CustomerId = 1,
                ProductOfferId = 1,
                Quantity = 2,
                TotalPrice = 200,
                PurchaseDate = DateTime.UtcNow,
                Status = PurchaseStatus.Active,
                ProductOffer = offer
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _productOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _userRepositoryMock.Setup(repo => repo.Update(It.IsAny<Customer>()));
            _userRepositoryMock.Setup(repo => repo.Update(It.IsAny<Company>()));
            _productOfferRepositoryMock.Setup(repo => repo.Update(It.IsAny<ProductOffer>()));
            _purchaseRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Purchase>(), It.IsAny<CancellationToken>())).ReturnsAsync(purchase);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _purchaseService.PurchaseOfferAsync(1, 1, 2, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.TotalPrice.Should().Be(200);
                customer.Balance.Should().Be(300);
                company.Balance.Should().Be(200);
                offer.Quantity.Should().Be(8);
            }

            _userRepositoryMock.Verify(x => x.Update(It.IsAny<Customer>()), Times.Once);
            _userRepositoryMock.Verify(x => x.Update(It.IsAny<Company>()), Times.Once);
            _productOfferRepositoryMock.Verify(x => x.Update(It.IsAny<ProductOffer>()), Times.Once);
            _purchaseRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Purchase>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PurchaseOfferAsync_ShouldThrow_WhenTransactionFails()
        {
            var customer = new Customer { Id = 1, Balance = 500 };
            var company = new Company { Id = 2, Balance = 0 };
            var offer = new ProductOffer { Id = 1, CompanyId = 2, Price = 100, Quantity = 10, Status = OfferStatus.Active };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _productOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<PurchaseException>(() => _purchaseService.PurchaseOfferAsync(1, 1, 2, CancellationToken.None));

            _userRepositoryMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>()), Times.Once);
            _productOfferRepositoryMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region 2 - CancelPurchaseAsync

        [Fact]
        public async Task CancelPurchaseAsync_ShouldThrow_WhenCustomerNotFound()
        {
            var purchase = new Purchase { CustomerId = 1 };
            _purchaseRepositoryMock.Setup(repo => repo.GetPurchaseWithDetailsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(purchase);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as User);

            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseService.CancelPurchaseAsync(1, 1, CancellationToken.None));

            _purchaseRepositoryMock.Verify(x => x.GetPurchaseWithDetailsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelPurchaseAsync_ShouldThrow_WhenTransactionFails()
        {
            var purchase = new Purchase
            {
                Id = 1,
                CustomerId = 1,
                PurchaseDate = DateTime.UtcNow.AddMinutes(-3),
                TotalPrice = 100,
                Quantity = 2,
                Status = PurchaseStatus.Active,
                ProductOffer = new ProductOffer { Id = 1, Quantity = 5, Company = new Company { Id = 2, Balance = 500 } }
            };
            var customer = new Customer { Id = 1, Balance = 50 };

            _purchaseRepositoryMock.Setup(repo => repo.GetPurchaseWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(purchase);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<InvalidCancellationRequestException>(() => _purchaseService.CancelPurchaseAsync(1, 1, CancellationToken.None));

            _purchaseRepositoryMock.Verify(x => x.GetPurchaseWithDetailsAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelPurchaseAsync_ShouldSucceed_WhenAllConditionsMet()
        {
            var purchase = new Purchase
            {
                Id = 1,
                CustomerId = 1,
                PurchaseDate = DateTime.UtcNow.AddMinutes(-3),
                TotalPrice = 100,
                Quantity = 2,
                Status = PurchaseStatus.Active,
                ProductOffer = new ProductOffer
                {
                    Id = 1,
                    Quantity = 5,
                    Company = new Company { Id = 2, Balance = 500 }
                }
            };

            var customer = new Customer { Id = 1, Balance = 50 };
            var company = purchase.ProductOffer.Company;

            _purchaseRepositoryMock.Setup(repo => repo.GetPurchaseWithDetailsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(purchase);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.Is<int>(id => id == 1), It.IsAny<CancellationToken>())).ReturnsAsync(customer);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.Is<int>(id => id == 2), It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _purchaseRepositoryMock.Setup(repo => repo.Update(It.IsAny<Purchase>()));
            _userRepositoryMock.Setup(repo => repo.Update(It.IsAny<Customer>()));
            _userRepositoryMock.Setup(repo => repo.Update(It.IsAny<Company>()));
            _productOfferRepositoryMock.Setup(repo => repo.Update(It.IsAny<ProductOffer>()));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1); // Ensure success

            var result = await _purchaseService.CancelPurchaseAsync(1, 1, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().BeTrue();
                customer.Balance.Should().Be(150);
                company.Balance.Should().Be(400);
                purchase.ProductOffer.Quantity.Should().Be(7);
                purchase.Status.Should().Be(PurchaseStatus.Cancelled);
            }

            _userRepositoryMock.Verify(repo => repo.Update(customer), Times.Once);
            _userRepositoryMock.Verify(repo => repo.Update(company), Times.Once);
            _productOfferRepositoryMock.Verify(repo => repo.Update(purchase.ProductOffer), Times.Once);
            _purchaseRepositoryMock.Verify(repo => repo.Update(purchase), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region 3 - GetUserPurchasesAsync

        [Fact]
        public async Task GetUserPurchasesAsync_ShouldThrow_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseService.GetUserPurchasesAsync(1, 10, 1, CancellationToken.None));
            
            _userRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task GetUserPurchasesAsync_ShouldReturn_PurchaseList()
        {
            var purchases = new List<Purchase> { new Purchase { Id = 1 }, new Purchase { Id = 2 } };

            _userRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _purchaseRepositoryMock.Setup(repo => repo.GetUserPurchasesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(purchases);

            var result = await _purchaseService.GetUserPurchasesAsync(1, 10, 1, CancellationToken.None);
            result.Should().HaveCount(2);

            _userRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
            _purchaseRepositoryMock.Verify(repo => repo.GetUserPurchasesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),times: Times.Once);
        }

        #endregion

        #region 4 - GetOfferPurchasesAsync
        [Fact]
        public async Task GetOfferPurchasesAsync_ShouldThrow_WhenOfferNotFound()
        {
            _productOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as ProductOffer);

            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseService.GetOfferPurchasesAsync(1, 10, 1, 1, UserRole.Admin, CancellationToken.None));
            
            _productOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), times: Times.Once);

        }

        [Fact]
        public async Task GetOfferPurchasesAsync_ShouldThrow_WhenUnauthorizedAccess()
        {
            var offer = new ProductOffer { Id = 1, CompanyId = 99 }; // Different Company

            _productOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(offer);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _purchaseService.GetOfferPurchasesAsync(1, 10, 1, 1, UserRole.Company, CancellationToken.None));

            _productOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), times: Times.Once);

        }

        [Fact]
        public async Task GetOfferPurchasesAsync_ShouldReturn_PurchaseList_WhenAuthorized()
        {
            var offer = new ProductOffer { Id = 1, CompanyId = 1 }; // Same Company
            var purchases = new List<Purchase> { new Purchase { Id = 1 }, new Purchase { Id = 2 } };

            _productOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(offer);
            _purchaseRepositoryMock.Setup(repo => repo.GetOfferPurchasesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(purchases);

            var result = await _purchaseService.GetOfferPurchasesAsync(1, 10, 1, 1, UserRole.Company, CancellationToken.None);
            result.Should().HaveCount(2);

            _productOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), times: Times.Once);
            _purchaseRepositoryMock.Verify(repo => repo.GetOfferPurchasesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), times: Times.Once);

        }

        [Fact]
        public async Task GetOfferPurchasesAsync_ShouldReturnPurchaseList_WhenAuthorizedAsAdmin()
        {
            var offer = new ProductOffer { Id = 1, CompanyId = 99 }; // Different Company, but Admin
            var purchases = new List<Purchase> { new Purchase { Id = 1 }, new Purchase { Id = 2 } };
            _productOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _purchaseRepositoryMock.Setup(repo => repo.GetOfferPurchasesAsync(1, 10, 1, It.IsAny<CancellationToken>())).ReturnsAsync(purchases);


            var result = await _purchaseService.GetOfferPurchasesAsync(1, 10, 1, 1, UserRole.Admin, CancellationToken.None);


            result.Should().HaveCount(2);

            _productOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once());
            _purchaseRepositoryMock.Verify(repo => repo.GetOfferPurchasesAsync(1, 10, 1, It.IsAny<CancellationToken>()), Times.Once());
        }

        #endregion

        #region 5 - GetPurchaseByIdAsync

        [Fact]
        public async Task GetPurchaseByIdAsync_ShouldReturnPurchase_WhenValid()
        {
            // Arrange
            var purchase = new Purchase { Id = 1, CustomerId = 1, Quantity = 2, TotalPrice = 20m };
            _purchaseRepositoryMock.Setup(repo => repo.GetPurchaseWithDetailsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(purchase);

            // Act
            var result = await _purchaseService.GetPurchaseByIdAsync(1, 1, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeOfType<PurchaseResponseModel>();
                result.Id.Should().Be(1);
                result.CustomerId.Should().Be(1);
                result.Quantity.Should().Be(2); // Assuming PurchaseResponseModel has these
                result.TotalPrice.Should().Be(20m);
            }

            // Verify
            _purchaseRepositoryMock.Verify(repo => repo.GetPurchaseWithDetailsAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPurchaseByIdAsync_ShouldThrowNotFound_WhenPurchaseMissing()
        {
            // Arrange
            _purchaseRepositoryMock.Setup(repo => repo.GetPurchaseWithDetailsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Purchase);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseService.GetPurchaseByIdAsync(1, 1, CancellationToken.None));

            // Verify
            _purchaseRepositoryMock.Verify(repo => repo.GetPurchaseWithDetailsAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPurchaseByIdAsync_ShouldThrowUnauthorized_WhenNotOwner()
        {
            // Arrange
            var purchase = new Purchase { Id = 1, CustomerId = 2 }; // Different customer
            _purchaseRepositoryMock.Setup(repo => repo.GetPurchaseWithDetailsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(purchase);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _purchaseService.GetPurchaseByIdAsync(1, 1, CancellationToken.None));

            // Verify
            _purchaseRepositoryMock.Verify(repo => repo.GetPurchaseWithDetailsAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }


        #endregion
    }
}

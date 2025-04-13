using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Moq;
using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Images;
using offers.itacademy.ge.Application.Models.ProductOffers;
using offers.itacademy.ge.Application.Services;
using offers.itacademy.ge.Application.Services.ImageServices;
using offers.itacademy.ge.Domain.Categories;
using offers.itacademy.ge.Domain.ProductOffers;
using offers.itacademy.ge.Domain.Purchases;
using offers.itacademy.ge.Domain.Users;
using System.Data;
using System.Linq.Expressions;

namespace offers.itacademy.ge.Application.Tests.ProductOffers
{
    public class ProductOfferServiceTests : IClassFixture<UnitOfWorkFixture>
    {
        private readonly UnitOfWorkFixture _fixture;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IOptions<ImageUploadSettings>> _optionsMock;
        private readonly ProductOfferService _service;

        public ProductOfferServiceTests(UnitOfWorkFixture fixture)
        {
            _fixture = fixture;
            _fileServiceMock = new Mock<IFileService>();
            _optionsMock = new Mock<IOptions<ImageUploadSettings>>();
            _optionsMock.Setup(o => o.Value).Returns(new ImageUploadSettings
            {
                DefaultOfferImageName = "/default.jpg",
                DefaultOfferImagePath = "/default.jpg",
                OfferImagePath = "/offers"
            });
            _service = new ProductOfferService(_fixture.UnitOfWorkMock.Object, _fileServiceMock.Object, _optionsMock.Object);


            _fixture.UnitOfWorkMock.Invocations.Clear();
            _fixture.UserRepositoryMock.Invocations.Clear();
            _fixture.ProductOfferRepositoryMock.Invocations.Clear();
            _fixture.PurchaseRepositoryMock.Invocations.Clear();
            _fixture.CategoryRepositoryMock.Invocations.Clear();
            _fileServiceMock.Invocations.Clear();
        }


        #region 1 - GetAllOffersAsync

        [Fact]
        public async Task GetAllOffersAsync_ShouldReturnOffers_WhenOffersExist()
        {
            var offers = new List<ProductOffer>
            {
                new ProductOffer { Id = 1, Name = "Offer 1" },
                new ProductOffer { Id = 2, Name = "Offer 2" }
            };
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetAllAsyncIncludeDetails(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offers);

            var result = await _service.GetAllOffersAsync(1, 10, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result[0].Id.Should().Be(1);
                result[0].Name.Should().Be("Offer 1");
                result[1].Id.Should().Be(2);
                result[1].Name.Should().Be("Offer 2");
            }
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetAllAsyncIncludeDetails(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllOffersAsync_ShouldReturnEmptyList_WhenNoOffersExist()
        {
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetAllAsyncIncludeDetails(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProductOffer>());

            var result = await _service.GetAllOffersAsync(1, 10, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetAllAsyncIncludeDetails(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region 2 - GetAllActiveOffersAsync

        [Fact]
        public async Task GetAllActiveOffersAsync_ShouldReturnActiveOffers_WhenActiveOffersExist()
        {
            var activeOffers = new List<ProductOffer>
            {
                new ProductOffer { Id = 1, Name = "Active Offer 1", Status = OfferStatus.Active },
                new ProductOffer { Id = 2, Name = "Active Offer 2", Status = OfferStatus.Active }
            };
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetActiveOffersAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(activeOffers);

            var result = await _service.GetAllActiveOffersAsync(1, 10, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result[0].Id.Should().Be(1);
                result[0].Name.Should().Be("Active Offer 1");
                result[1].Id.Should().Be(2);
                result[1].Name.Should().Be("Active Offer 2");
            }
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetActiveOffersAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllActiveOffersAsync_ShouldReturnEmptyList_WhenNoActiveOffersExist()
        {
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetActiveOffersAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProductOffer>());

            var result = await _service.GetAllActiveOffersAsync(1, 10, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetActiveOffersAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region 3 - GetOfferByIdAsync

        [Fact]
        public async Task GetOfferByIdAsync_ShouldReturnOffer_WhenOfferExists()
        {
            var offer = new ProductOffer { Id = 1, Name = "Test Offer" };
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsyncIncludingDetails(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offer);

            var result = await _service.GetOfferByIdAsync(1, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Id.Should().Be(1);
                result.Name.Should().Be("Test Offer");
            }
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsyncIncludingDetails(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetOfferByIdAsync_ShouldThrowNotFound_WhenOfferDoesNotExist()
        {
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsyncIncludingDetails(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as ProductOffer);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetOfferByIdAsync(1, CancellationToken.None));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsyncIncludingDetails(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region 4 - CreateOfferAsync

        [Fact]
        public async Task CreateOfferAsync_ShouldThrowNotFound_WhenCompanyDoesNotExist()
        {
            var request = new ProductOfferRequestModel { Name = "New Offer", CategoryId = 1 };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(null as Company);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateOfferAsync(request, 1, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.CategoryRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ProductOffer>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateOfferAsync_ShouldThrowInvalidOperation_WhenCompanyIsNotActivated()
        {
            var request = new ProductOfferRequestModel { Name = "New Offer", CategoryId = 1 };
            var company = new Company { Id = 1, IsActivated = false };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(company);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateOfferAsync(request, 1, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.CategoryRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ProductOffer>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateOfferAsync_ShouldThrowNotFound_WhenCategoryDoesNotExist()
        {
            var request = new ProductOfferRequestModel { Name = "New Offer", CategoryId = 1 };
            var company = new Company { Id = 1, IsActivated = true };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fixture.CategoryRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateOfferAsync(request, 1, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.CategoryRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ProductOffer>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateOfferAsync_ShouldThrow_WhenSaveChangesFails()
        {
            var request = new ProductOfferRequestModel { Name = "New Offer", CategoryId = 1 };
            var company = new Company { Id = 1, IsActivated = true };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fixture.CategoryRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<ProductOffer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductOffer offer, CancellationToken token) => offer);
            _fixture.UnitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.CreateOfferAsync(request, 1, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.CategoryRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ProductOffer>(), It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region 5 - DeleteOfferAsync

        [Fact]
        public async Task DeleteOfferAsync_ShouldDeleteOffer_WhenAuthorizedAndOfferExists()
        {
            var offer = new ProductOffer { Id = 1, CompanyId = 1 };
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.Remove(It.IsAny<ProductOffer>())); // No Returns for void method
            _fixture.UnitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            await _service.DeleteOfferAsync(1, 1, CancellationToken.None);

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Remove(It.Is<ProductOffer>(o => o.Id == 1 && o.CompanyId == 1)), Times.Once);
            _fixture.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteOfferAsync_ShouldThrowNotFound_WhenOfferDoesNotExist()
        {
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(null as ProductOffer);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteOfferAsync(1, 1, CancellationToken.None));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Remove(It.IsAny<ProductOffer>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteOfferAsync_ShouldThrowUnauthorized_WhenCompanyIdDoesNotMatch()
        {
            var offer = new ProductOffer { Id = 1, CompanyId = 2 };
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteOfferAsync(1, 1, CancellationToken.None));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Remove(It.IsAny<ProductOffer>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteOfferAsync_ShouldThrow_WhenSaveChangesFails()
        {
            var offer = new ProductOffer { Id = 1, CompanyId = 1 };
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.Remove(It.IsAny<ProductOffer>())); // No Returns for void method
            _fixture.UnitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.DeleteOfferAsync(1, 1, CancellationToken.None));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Remove(It.Is<ProductOffer>(o => o.Id == 1 && o.CompanyId == 1)), Times.Once);
            _fixture.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        #endregion

        #region 6 - UpdateOfferAsync

        [Fact]
        public async Task UpdateOfferAsync_ShouldUpdateOffer_WhenInputsAreValid()
        {
            var existingOffer = new ProductOffer { Id = 1, CompanyId = 1, Status = OfferStatus.Active, Name = "Old Offer", CategoryId = 1 };
            var updatedRequest = new ProductOfferRequestModel { Name = "Updated Offer", CategoryId = 2 };

            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingOffer);
            _fixture.CategoryRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.Update(It.IsAny<ProductOffer>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.UpdateOfferAsync(1, updatedRequest, 1, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Name.Should().Be("Updated Offer");
                result.CategoryId.Should().Be(2);
                result.CompanyId.Should().Be(1);
                result.Status.Should().Be(OfferStatus.Active);
            }
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.CategoryRepositoryMock.Verify(repo => repo.ExistsAsync(It.Is<Expression<Func<Category, bool>>>(expr => expr.Compile()(new Category { Id = 2 })), It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.Is<ProductOffer>(o => o.Name == "Updated Offer" && o.CategoryId == 2 && o.CompanyId == 1 && o.Status == OfferStatus.Active)), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOfferAsync_ShouldThrowNotFound_WhenCategoryDoesNotExist()
        {
            var existingOffer = new ProductOffer { Id = 1, CompanyId = 1, Status = OfferStatus.Active };
            var updatedRequest = new ProductOfferRequestModel { Name = "Updated Offer", CategoryId = 2 };

            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingOffer);
            _fixture.CategoryRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateOfferAsync(1, updatedRequest, 1, CancellationToken.None));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.CategoryRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateOfferAsync_ShouldThrow_WhenSaveChangesFails()
        {
            var existingOffer = new ProductOffer { Id = 1, CompanyId = 1, Status = OfferStatus.Active, Name = "Old Offer", CategoryId = 1 };
            var updatedRequest = new ProductOfferRequestModel { Name = "Updated Offer", CategoryId = 2 };

            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingOffer);
            _fixture.CategoryRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.Update(It.IsAny<ProductOffer>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.UpdateOfferAsync(1, updatedRequest, 1, CancellationToken.None));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.CategoryRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOfferAsync_ShouldThrowOperationCanceled_WhenTokenIsCanceled()
        {
            var cts = new CancellationTokenSource();
            await cts.CancelAsync();
            var updatedRequest = new ProductOfferRequestModel { Name = "Updated Offer", CategoryId = 2 };

            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, cts.Token)).ThrowsAsync(new OperationCanceledException());

            await Assert.ThrowsAsync<OperationCanceledException>(() => _service.UpdateOfferAsync(1, updatedRequest, 1, cts.Token));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, cts.Token), Times.Once);
            _fixture.CategoryRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

            cts.Dispose();
        }

        #endregion

        #region 7 - CancelOfferAsync

        [Fact]
        public async Task CancelOfferAsync_ShouldCancelOfferAndRefund_WhenWithinTimeAndPurchasesExist()
        {
            var offer = new ProductOffer
            {
                Id = 1,
                CompanyId = 1,
                CreateTime = DateTime.UtcNow.AddMinutes(-5),
                Quantity = 10,
                Company = new Company { Id = 1, Balance = 100m },
                Purchases = new List<Purchase>
                {
                    new Purchase { Id = 1, CustomerId = 2, TotalPrice = 20m, Quantity = 2, Status = PurchaseStatus.Active, Customer = new Customer { Id = 2, Balance = 50m } }
                }
            };
            var transactionMock = new Mock<IDbContextTransaction>();
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _fixture.UnitOfWorkMock.Setup(uow => uow.BeginTransactionAsync(IsolationLevel.ReadCommitted, It.IsAny<CancellationToken>())).ReturnsAsync(transactionMock.Object);
            _fixture.UserRepositoryMock.Setup(repo => repo.Update(It.IsAny<User>()));
            _fixture.PurchaseRepositoryMock.Setup(repo => repo.Update(It.IsAny<Purchase>()));
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.Update(It.IsAny<ProductOffer>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CancelOfferAsync(1, 1, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().BeTrue();
                offer.Status.Should().Be(OfferStatus.Cancelled);
                offer.Purchases[0].Status.Should().Be(PurchaseStatus.Cancelled);
                offer.Purchases[0].Customer!.Balance.Should().Be(70m);
                offer.Company.Balance.Should().Be(80m);
                offer.Quantity.Should().Be(12);
            }
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.Is<Customer>(u => u.Id == 2 && u.Balance == 70m)), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.Is<Company>(u => u.Id == 1 && u.Balance == 80m)), Times.Once);
            _fixture.PurchaseRepositoryMock.Verify(repo => repo.Update(It.Is<Purchase>(p => p.Id == 1 && p.Status == PurchaseStatus.Cancelled)), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.Is<ProductOffer>(o => o.Id == 1 && o.Status == OfferStatus.Cancelled)), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelOfferAsync_ShouldThrowNotFound_WhenOfferDoesNotExist()
        {
            var transactionMock = new Mock<IDbContextTransaction>();
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(null as ProductOffer);
            _fixture.UnitOfWorkMock.Setup(uow => uow.BeginTransactionAsync(IsolationLevel.ReadCommitted, It.IsAny<CancellationToken>())).ReturnsAsync(transactionMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.CancelOfferAsync(1, 1, CancellationToken.None));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
            _fixture.PurchaseRepositoryMock.Verify(repo => repo.Update(It.IsAny<Purchase>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelOfferAsync_ShouldThrowCancellationException_WhenTimeLimitExceeded()
        {
            var offer = new ProductOffer
            {
                Id = 1,
                CompanyId = 1,
                CreateTime = DateTime.UtcNow.AddMinutes(-15)
            };
            var transactionMock = new Mock<IDbContextTransaction>();
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _fixture.UnitOfWorkMock.Setup(uow => uow.BeginTransactionAsync(IsolationLevel.ReadCommitted, It.IsAny<CancellationToken>())).ReturnsAsync(transactionMock.Object);

            await Assert.ThrowsAsync<OfferCancellationException>(() => _service.CancelOfferAsync(1, 1, CancellationToken.None));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
            _fixture.PurchaseRepositoryMock.Verify(repo => repo.Update(It.IsAny<Purchase>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelOfferAsync_ShouldCancelOffer_WhenNoPurchasesExist()
        {
            var offer = new ProductOffer
            {
                Id = 1,
                CompanyId = 1,
                CreateTime = DateTime.UtcNow.AddMinutes(-5),
                Quantity = 10,
                Company = new Company { Id = 1, Balance = 100m },
                Purchases = new List<Purchase>()
            };
            var transactionMock = new Mock<IDbContextTransaction>();
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _fixture.UnitOfWorkMock.Setup(uow => uow.BeginTransactionAsync(IsolationLevel.ReadCommitted, It.IsAny<CancellationToken>())).ReturnsAsync(transactionMock.Object);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.Update(It.IsAny<ProductOffer>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CancelOfferAsync(1, 1, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().BeTrue();
                offer.Status.Should().Be(OfferStatus.Cancelled);
                offer.Company.Balance.Should().Be(100m);
                offer.Quantity.Should().Be(10);
            }
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
            _fixture.PurchaseRepositoryMock.Verify(repo => repo.Update(It.IsAny<Purchase>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.Is<ProductOffer>(o => o.Id == 1 && o.Status == OfferStatus.Cancelled)), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelOfferAsync_ShouldThrowInvalidOperation_WhenCustomerIsNull()
        {
            var offer = new ProductOffer
            {
                Id = 1,
                CompanyId = 1,
                CreateTime = DateTime.UtcNow.AddMinutes(-5),
                Quantity = 10,
                Company = new Company { Id = 1, Balance = 100m },
                Purchases = new List<Purchase>
                {
                    new Purchase { Id = 1, CustomerId = 2, TotalPrice = 20m, Quantity = 2, Status = PurchaseStatus.Active, Customer = null }
                }
            };
            var transactionMock = new Mock<IDbContextTransaction>();
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _fixture.UnitOfWorkMock.Setup(uow => uow.BeginTransactionAsync(IsolationLevel.ReadCommitted, It.IsAny<CancellationToken>())).ReturnsAsync(transactionMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CancelOfferAsync(1, 1, CancellationToken.None));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
            _fixture.PurchaseRepositoryMock.Verify(repo => repo.Update(It.IsAny<Purchase>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelOfferAsync_ShouldThrowInvalidOperation_WhenInsufficientCompanyBalance()
        {
            var offer = new ProductOffer
            {
                Id = 1,
                CompanyId = 1,
                CreateTime = DateTime.UtcNow.AddMinutes(-5),
                Quantity = 10,
                Company = new Company { Id = 1, Balance = 10m },
                Purchases = new List<Purchase>
                {
                    new Purchase { Id = 1, CustomerId = 2, TotalPrice = 20m, Quantity = 2, Status = PurchaseStatus.Active, Customer = new Customer { Id = 2, Balance = 50m } }
                }
            };
            var transactionMock = new Mock<IDbContextTransaction>();
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _fixture.UnitOfWorkMock.Setup(uow => uow.BeginTransactionAsync(IsolationLevel.ReadCommitted, It.IsAny<CancellationToken>())).ReturnsAsync(transactionMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CancelOfferAsync(1, 1, CancellationToken.None));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
            _fixture.PurchaseRepositoryMock.Verify(repo => repo.Update(It.IsAny<Purchase>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelOfferAsync_ShouldRollback_WhenSaveChangesFails()
        {
            var offer = new ProductOffer
            {
                Id = 1,
                CompanyId = 1,
                CreateTime = DateTime.UtcNow.AddMinutes(-5),
                Quantity = 10,
                Company = new Company { Id = 1, Balance = 100m },
                Purchases = new List<Purchase>
                {
                    new Purchase { Id = 1, CustomerId = 2, TotalPrice = 20m, Quantity = 2, Status = PurchaseStatus.Active, Customer = new Customer { Id = 2, Balance = 50m } }
                }
            };
            var transactionMock = new Mock<IDbContextTransaction>();
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _fixture.UnitOfWorkMock.Setup(uow => uow.BeginTransactionAsync(IsolationLevel.ReadCommitted, It.IsAny<CancellationToken>())).ReturnsAsync(transactionMock.Object);
            _fixture.UserRepositoryMock.Setup(repo => repo.Update(It.IsAny<User>()));
            _fixture.PurchaseRepositoryMock.Setup(repo => repo.Update(It.IsAny<Purchase>()));
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.Update(It.IsAny<ProductOffer>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.CancelOfferAsync(1, 1, CancellationToken.None));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetCancellableOfferAsync(1, 1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Exactly(2));
            _fixture.PurchaseRepositoryMock.Verify(repo => repo.Update(It.IsAny<Purchase>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelOfferAsync_ShouldThrowOperationCanceled_WhenTokenIsCanceled()
        {
            var cts = new CancellationTokenSource();
            await cts.CancelAsync();
            var transactionMock = new Mock<IDbContextTransaction>();
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetCancellableOfferAsync(1, 1, cts.Token)).ThrowsAsync(new OperationCanceledException());
            _fixture.UnitOfWorkMock.Setup(uow => uow.BeginTransactionAsync(IsolationLevel.ReadCommitted, cts.Token)).ReturnsAsync(transactionMock.Object);

            await Assert.ThrowsAsync<OperationCanceledException>(() => _service.CancelOfferAsync(1, 1, cts.Token));

            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetCancellableOfferAsync(1, 1, cts.Token), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
            _fixture.PurchaseRepositoryMock.Verify(repo => repo.Update(It.IsAny<Purchase>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

            cts.Dispose();
        }

        #endregion

        #region 8 - GetUserRelevantOffersAsync

        [Fact]
        public async Task GetUserRelevantOffersAsync_ShouldReturnOffers_WhenCustomerAndOffersExist()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                SelectedCategories = new List<Category> { new Category { Id = 1 }, new Category { Id = 2 } }
            };
            var offers = new List<ProductOffer>
            {
                new ProductOffer { Id = 1, Name = "Offer 1", CategoryId = 1, Status = OfferStatus.Active },
                new ProductOffer { Id = 2, Name = "Offer 2", CategoryId = 2, Status = OfferStatus.Active }
            };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetActiveOffersByCategoriesAsync(1, 10, customer.SelectedCategories, It.IsAny<CancellationToken>())).ReturnsAsync(offers);

            // Act
            var result = await _service.GetUserRelevantOffersAsync(1, 10, 1, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result[0].Name.Should().Be("Offer 1");
                result[0].CategoryId.Should().Be(1);
                result[1].Name.Should().Be("Offer 2");
                result[1].CategoryId.Should().Be(2);
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>()), Times.Once());
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetActiveOffersByCategoriesAsync(1, 10, customer.SelectedCategories, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetUserRelevantOffersAsync_ShouldThrowNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            _fixture.UserRepositoryMock.Setup(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Customer?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetUserRelevantOffersAsync(1, 10, 1, CancellationToken.None));

            // Verify
            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>()), Times.Once());
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetActiveOffersByCategoriesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<Category>>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task GetUserRelevantOffersAsync_ShouldReturnEmptyList_WhenNoRelevantOffersExist()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                SelectedCategories = new List<Category> { new Category { Id = 1 } }
            };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetActiveOffersByCategoriesAsync(1, 10, customer.SelectedCategories, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProductOffer>());

            // Act
            var result = await _service.GetUserRelevantOffersAsync(1, 10, 1, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>()), Times.Once());
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetActiveOffersByCategoriesAsync(1, 10, customer.SelectedCategories, It.IsAny<CancellationToken>()), Times.Once());
        }


        #endregion

        #region 9 - GetCompanyOffersAsync

        [Fact]
        public async Task GetCompanyOffersAsync_ShouldReturnOffers_WhenCompanyAndOffersExist()
        {
            // Arrange
            var offers = new List<ProductOffer>
    {
        new ProductOffer { Id = 1, Name = "Offer 1", CompanyId = 1 },
        new ProductOffer { Id = 2, Name = "Offer 2", CompanyId = 1 }
    };
            _fixture.UserRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetCompanyOffersAsync(1, 10, 1, It.IsAny<CancellationToken>())).ReturnsAsync(offers);

            // Act
            var result = await _service.GetCompanyOffersAsync(1, 10, 1, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result[0].Name.Should().Be("Offer 1");
                result[0].CompanyId.Should().Be(1);
                result[1].Name.Should().Be("Offer 2");
                result[1].CompanyId.Should().Be(1);
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.ExistsAsync(It.Is<Expression<Func<User, bool>>>(expr => expr.Compile()(new Company { Id = 1 })), It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetCompanyOffersAsync(1, 10, 1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCompanyOffersAsync_ShouldThrowNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            _fixture.UserRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCompanyOffersAsync(1, 10, 1, CancellationToken.None));

            // Verify
            _fixture.UserRepositoryMock.Verify(repo => repo.ExistsAsync(It.Is<Expression<Func<User, bool>>>(expr => expr.Compile()(new Company { Id = 1 })), It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetCompanyOffersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetCompanyOffersAsync_ShouldReturnEmptyList_WhenNoOffersExist()
        {
            // Arrange
            _fixture.UserRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetCompanyOffersAsync(1, 10, 1, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProductOffer>());

            // Act
            var result = await _service.GetCompanyOffersAsync(1, 10, 1, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.ExistsAsync(It.Is<Expression<Func<User, bool>>>(expr => expr.Compile()(new Company { Id = 1 })), It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetCompanyOffersAsync(1, 10, 1, It.IsAny<CancellationToken>()), Times.Once);
        }


        #endregion

        #region 10 - UploadProductOfferImage

        [Fact]
        public async Task UploadProductOfferImage_ShouldUploadImage_WhenInputsAreValid()
        {
            var company = new Company { Id = 1 };
            var offer = new ProductOffer { Id = 1, CompanyId = 1, ImageUrl = "/old.jpg" };
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            var fileForm = new ImageRequestModel { File = fileMock.Object };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _fileServiceMock.Setup(fs => fs.UploadImageAsync(fileMock.Object, "/offers", It.IsAny<CancellationToken>())).ReturnsAsync("/new.jpg");
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.Update(It.IsAny<ProductOffer>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _fileServiceMock.Setup(fs => fs.DeleteImageAsync("/old.jpg", "/offers", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _service.UploadProductOfferImage(1, 1, fileForm, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().Be("/new.jpg");
                offer.ImageUrl.Should().Be("/new.jpg");
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.UploadImageAsync(fileMock.Object, "/offers", It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.Is<ProductOffer>(o => o.ImageUrl == "/new.jpg")), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync("/old.jpg", "/offers", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UploadProductOfferImage_ShouldThrowNotFound_WhenCompanyDoesNotExist()
        {
            var fileMock = new Mock<IFormFile>();
            var fileForm = new ImageRequestModel { File = fileMock.Object };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(null as Company);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.UploadProductOfferImage(1, 1, fileForm, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _fileServiceMock.Verify(fs => fs.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Never);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UploadProductOfferImage_ShouldThrowNotFound_WhenOfferDoesNotExist()
        {
            var company = new Company { Id = 1 };
            var fileMock = new Mock<IFormFile>();
            var fileForm = new ImageRequestModel { File = fileMock.Object };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(null as ProductOffer);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.UploadProductOfferImage(1, 1, fileForm, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Never);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UploadProductOfferImage_ShouldThrowUnauthorized_WhenCompanyDoesNotOwnOffer()
        {
            var company = new Company { Id = 1 };
            var offer = new ProductOffer { Id = 1, CompanyId = 2, ImageUrl = "/old.jpg" };
            var fileMock = new Mock<IFormFile>();
            var fileForm = new ImageRequestModel { File = fileMock.Object };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UploadProductOfferImage(1, 1, fileForm, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Never);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UploadProductOfferImage_ShouldRollback_WhenSaveChangesFails()
        {
            var company = new Company { Id = 1 };
            var offer = new ProductOffer { Id = 1, CompanyId = 1, ImageUrl = "/old.jpg" };
            var fileMock = new Mock<IFormFile>();
            var fileForm = new ImageRequestModel { File = fileMock.Object };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _fileServiceMock.Setup(fs => fs.UploadImageAsync(fileMock.Object, "/offers", It.IsAny<CancellationToken>())).ReturnsAsync("/new.jpg");
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.Update(It.IsAny<ProductOffer>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB Failure"));
            _fileServiceMock.Setup(fs => fs.DeleteImageAsync("/new.jpg", "/offers", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            await Assert.ThrowsAsync<Exception>(() => _service.UploadProductOfferImage(1, 1, fileForm, CancellationToken.None));

            using (new AssertionScope())
            {
                offer.ImageUrl.Should().Be("/new.jpg");
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.UploadImageAsync(fileMock.Object, "/offers", It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.Is<ProductOffer>(o => o.ImageUrl == "/new.jpg")), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync("/new.jpg", "/offers", It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync("/old.jpg", "/offers", It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UploadProductOfferImage_ShouldUploadImage_WhenNoOldImageExists()
        {
            var company = new Company { Id = 1 };
            var offer = new ProductOffer { Id = 1, CompanyId = 1, ImageUrl = null };
            var fileMock = new Mock<IFormFile>();
            var fileForm = new ImageRequestModel { File = fileMock.Object };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _fileServiceMock.Setup(fs => fs.UploadImageAsync(fileMock.Object, "/offers", It.IsAny<CancellationToken>())).ReturnsAsync("/new.jpg");
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.Update(It.IsAny<ProductOffer>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.UploadProductOfferImage(1, 1, fileForm, CancellationToken.None);

            using (new AssertionScope())
            {
                result.Should().Be("/new.jpg");
                offer.ImageUrl.Should().Be("/new.jpg");
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.UploadImageAsync(fileMock.Object, "/offers", It.IsAny<CancellationToken>()), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.Is<ProductOffer>(o => o.ImageUrl == "/new.jpg")), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UploadProductOfferImage_ShouldThrowOperationCanceled_WhenTokenIsCanceled()
        {
            var cts = new CancellationTokenSource();
            await cts.CancelAsync();
            var company = new Company { Id = 1 };
            var fileMock = new Mock<IFormFile>();
            var fileForm = new ImageRequestModel { File = fileMock.Object };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, cts.Token)).ReturnsAsync(company);
            _fixture.ProductOfferRepositoryMock.Setup(repo => repo.GetByIdAsync(1, cts.Token)).ThrowsAsync(new OperationCanceledException());

            await Assert.ThrowsAsync<OperationCanceledException>(() => _service.UploadProductOfferImage(1, 1, fileForm, cts.Token));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, cts.Token), Times.Once);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.GetByIdAsync(1, cts.Token), Times.Once);
            _fileServiceMock.Verify(fs => fs.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.ProductOfferRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductOffer>()), Times.Never);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

            cts.Dispose();
        }

        #endregion
    }
}

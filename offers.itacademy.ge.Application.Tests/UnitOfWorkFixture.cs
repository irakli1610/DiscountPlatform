using Moq;
using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Application.Repositories;

namespace offers.itacademy.ge.Application.Tests
{
    public class UnitOfWorkFixture : IDisposable
    {
        public Mock<IUnitOfWork> UnitOfWorkMock { get; }
        public Mock<IUserRepository> UserRepositoryMock { get; }
        public Mock<IPurchaseRepository> PurchaseRepositoryMock { get; }
        public Mock<IProductOfferRepository> ProductOfferRepositoryMock { get; }
        public Mock<ICategoryRepository> CategoryRepositoryMock { get; }


        public UnitOfWorkFixture()
        {
            UnitOfWorkMock = new Mock<IUnitOfWork>();
            UserRepositoryMock = new Mock<IUserRepository>();
            PurchaseRepositoryMock = new Mock<IPurchaseRepository>();
            ProductOfferRepositoryMock = new Mock<IProductOfferRepository>();
            CategoryRepositoryMock = new Mock<ICategoryRepository>();

            // Set up UoW to return mock repositories
            UnitOfWorkMock.Setup(uow => uow.Users).Returns(UserRepositoryMock.Object);
            UnitOfWorkMock.Setup(uow => uow.Purchases).Returns(PurchaseRepositoryMock.Object);
            UnitOfWorkMock.Setup(uow => uow.ProductOffers).Returns(ProductOfferRepositoryMock.Object);
            UnitOfWorkMock.Setup(uow => uow.Categories).Returns(CategoryRepositoryMock.Object);
            UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        }

        public void Dispose()
        {
            // Clean up if needed
        }


    }
}

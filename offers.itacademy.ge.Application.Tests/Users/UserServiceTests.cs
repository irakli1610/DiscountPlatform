using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Images;
using offers.itacademy.ge.Application.Models.Users;
using offers.itacademy.ge.Application.Models.Users.Admin;
using offers.itacademy.ge.Application.Models.Users.Company;
using offers.itacademy.ge.Application.Models.Users.Customer;
using offers.itacademy.ge.Application.Services;
using offers.itacademy.ge.Application.Services.ImageServices;
using offers.itacademy.ge.Application.Utils;
using offers.itacademy.ge.Application.Utils.Auth.JWT;
using offers.itacademy.ge.Domain.Categories;
using offers.itacademy.ge.Domain.Users;
using System.Linq.Expressions;
using System.Reflection;

namespace offers.itacademy.ge.Application.Tests.Users
{
    public class UserServiceTests : IClassFixture<UnitOfWorkFixture>
    {
        private readonly UnitOfWorkFixture _fixture;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IOptions<JWTConfiguration>> _jwtOptionsMock;
        private readonly Mock<IOptions<ImageUploadSettings>> _imageOptionsMock;
        private readonly UserService _service;

        public UserServiceTests(UnitOfWorkFixture fixture)
        {
            _fixture = fixture;

            _fileServiceMock = new Mock<IFileService>();
            _jwtOptionsMock = new Mock<IOptions<JWTConfiguration>>();
            _imageOptionsMock = new Mock<IOptions<ImageUploadSettings>>();

            _jwtOptionsMock.Setup(o => o.Value).Returns(new JWTConfiguration
            {
                Secret = "SuperSecretKey1234567890123456",
                Issuer = "testIssuer",
                Audience = "testAudience",
                ExpirationTimeInMinutes = 60
            });
            _imageOptionsMock.Setup(o => o.Value).Returns(new ImageUploadSettings
            {
                CompanyImagePath = "/company/images"
            });
            _service = new UserService(
                _fixture.UnitOfWorkMock.Object,
                _jwtOptionsMock.Object,
                _fileServiceMock.Object,
                _imageOptionsMock.Object
            );

            _fixture.UnitOfWorkMock.Invocations.Clear();
            _fixture.UserRepositoryMock.Invocations.Clear();
            _fixture.CategoryRepositoryMock.Invocations.Clear();
        }


        #region 1 - GetCompaniesAsync

        [Fact]
        public async Task GetUsersAsync_ShouldReturnCompanies_WhenValid()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company { Id = 1, UserName = "Company1", Email = "comp1@example.com", Balance = 1000m, IsActivated = true },
                new Company { Id = 2, UserName = "Company2", Email = "comp2@example.com", Balance = 2000m, IsActivated = false }
            };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetUsersAsync<Company>(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(companies);

            // Act
            var result = await _service.GetUsersAsync<Company, CompanyResponseModel>(1, 10, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result[0].Id.Should().Be(1);
                result[0].UserName.Should().Be("Company1");
                result[0].Email.Should().Be("comp1@example.com");
                result[0].Balance.Should().Be(1000m);
                result[0].IsActivated.Should().BeTrue();
                result[1].Id.Should().Be(2);
                result[1].UserName.Should().Be("Company2");
                result[1].Email.Should().Be("comp2@example.com");
                result[1].Balance.Should().Be(2000m);
                result[1].IsActivated.Should().BeFalse();
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.GetUsersAsync<Company>(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUsersAsync_ShouldReturnCustomers_WhenValid()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { Id = 1, UserName = "Customer1", Email = "cust1@example.com", Balance = 500m },
                new Customer { Id = 2, UserName = "Customer2", Email = "cust2@example.com", Balance = 750m }
            };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetUsersAsync<Customer>(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(customers);

            // Act
            var result = await _service.GetUsersAsync<Customer, CustomerResponseModel>(1, 10, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result[0].Id.Should().Be(1);
                result[0].UserName.Should().Be("Customer1");
                result[0].Email.Should().Be("cust1@example.com");
                result[0].Balance.Should().Be(500m);
                result[1].Id.Should().Be(2);
                result[1].UserName.Should().Be("Customer2");
                result[1].Email.Should().Be("cust2@example.com");
                result[1].Balance.Should().Be(750m);
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.GetUsersAsync<Customer>(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUsersAsync_ShouldReturnAdmins_WhenValid()
        {
            // Arrange
            var admins = new List<Admin>
            {
                new Admin { Id = 1, UserName = "Admin1", Email = "admin1@example.com" },
                new Admin { Id = 2, UserName = "Admin2", Email = "admin2@example.com" }
            };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetUsersAsync<Admin>(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(admins);

            // Act
            var result = await _service.GetUsersAsync<Admin, AdminResponseModel>(1, 10, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result[0].Id.Should().Be(1);
                result[0].UserName.Should().Be("Admin1");
                result[0].Email.Should().Be("admin1@example.com");
                result[1].Id.Should().Be(2);
                result[1].UserName.Should().Be("Admin2");
                result[1].Email.Should().Be("admin2@example.com");
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.GetUsersAsync<Admin>(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region 2 - ActivateCompanyAsync

        [Fact]
        public async Task ActivateCompanyAsync_ShouldActivate_WhenCompanyExistsAndNotActivated()
        {
            // Arrange
            var company = new Company { Id = 1, UserName = "Company1", IsActivated = false };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fixture.UserRepositoryMock.Setup(repo => repo.Update(It.IsAny<Company>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _service.ActivateCompanyAsync(1, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                company.IsActivated.Should().BeTrue();
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once());
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(company), Times.Once());
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task ActivateCompanyAsync_ShouldThrowNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(null as Company);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.ActivateCompanyAsync(1, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once());
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<Company>()), Times.Never());
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task ActivateCompanyAsync_ShouldThrowInvalidOperation_WhenCompanyAlreadyActivated()
        {
            // Arrange
            var company = new Company { Id = 1, UserName = "Company1", IsActivated = true };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(company);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ActivateCompanyAsync(1, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once());
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<Company>()), Times.Never());
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        #endregion

        #region 3 - UpdateCustomerCategoriesAsync

        [Fact]
        public async Task UpdateCustomerCategoriesAsync_ShouldUpdate_WhenCustomerAndCategoriesExist()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                UserName = "Customer1",
                SelectedCategories = new List<Category> { new Category { Id = 3, Name = "OldCat" } } // Initial category to be cleared
            };
            var categories = new List<Category>
    {
        new Category { Id = 1, Name = "Cat1" },
        new Category { Id = 2, Name = "Cat2" }
    };
            var categoryIds = new List<int> { 1, 2 };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
            _fixture.CategoryRepositoryMock.Setup(repo => repo.GetByIdsAsync(categoryIds, It.IsAny<CancellationToken>())).ReturnsAsync(categories);
            _fixture.UserRepositoryMock.Setup(repo => repo.Update(It.IsAny<Customer>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateCustomerCategoriesAsync(1, categoryIds, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                customer.SelectedCategories.Should().HaveCount(2);
                customer.SelectedCategories.Should().Contain(c => c.Id == 1 && c.Name == "Cat1");
                customer.SelectedCategories.Should().Contain(c => c.Id == 2 && c.Name == "Cat2");
                customer.SelectedCategories.Should().NotContain(c => c.Id == 3); // Ensure old category is cleared
            }
            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.CategoryRepositoryMock.Verify(repo => repo.GetByIdsAsync(categoryIds, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.Is<Customer>(c =>
                c.Id == 1 &&
                c.SelectedCategories.Count == 2 &&
                c.SelectedCategories.All(cat => categoryIds.Contains(cat.Id))
            )), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerCategoriesAsync_ShouldThrowNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var categoryIds = new List<int> { 1, 2 };

            _fixture.UserRepositoryMock.Setup(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(null as Customer);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateCustomerCategoriesAsync(1, categoryIds, CancellationToken.None));

            // Verify
            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.CategoryRepositoryMock.Verify(repo => repo.GetByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<Customer>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCustomerCategoriesAsync_ShouldThrowNotFound_WhenSomeCategoriesAreMissing()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                UserName = "Customer1",
                SelectedCategories = new List<Category>()
            };
            var categories = new List<Category> { new Category { Id = 1, Name = "Cat1" } }; // Only one of two requested categories exists
            var categoryIds = new List<int> { 1, 2 }; // Requesting two categories

            _fixture.UserRepositoryMock.Setup(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
            _fixture.CategoryRepositoryMock.Setup(repo => repo.GetByIdsAsync(categoryIds, It.IsAny<CancellationToken>())).ReturnsAsync(categories);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateCustomerCategoriesAsync(1, categoryIds, CancellationToken.None));

            // Verify
            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserWithCategoriesAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.CategoryRepositoryMock.Verify(repo => repo.GetByIdsAsync(categoryIds, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<Customer>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        #endregion

        #region 4 - AuthenticateAsync

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnJwtToken_WhenCredentialsAreValid()
        {
            // Arrange
            var customer = new Customer { Id = 1, UserName = "testUser", Password = AuthUtils.GeneratePasswordHash("password123"), Role = UserRole.Customer };
            var loginModel = new UserLoginRequestModel { UserName = "testUser", Password = "password123" };

            _fixture.UserRepositoryMock
                .Setup(repo => repo.GetUserByUserName(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer); // Return a concrete Customer object

            var jwtConfig = new JWTConfiguration
            {
                Secret = "SuperSecretKey1234567890123456vghvbjhbj",
                ExpirationTimeInMinutes = 60,
                Issuer = "issuer",
                Audience = "audience"
            };
            _jwtOptionsMock.Setup(o => o.Value).Returns(jwtConfig);

            // Act
            var result = await _service.AuthenticateAsync(loginModel, CancellationToken.None);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Split('.').Length.Should().Be(3); // JWT has 3 parts (header.payload.signature)

            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserByUserName(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            var loginModel = new UserLoginRequestModel { UserName = "nonexistentUser", Password = "password123" };

            _fixture.UserRepositoryMock
                .Setup(repo => repo.GetUserByUserName(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User); // Simulate user not found

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.AuthenticateAsync(loginModel, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserByUserName(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowUnauthorized_WhenPasswordIsIncorrect()
        {
            // Arrange
            var company = new Company { Id = 1, UserName = "testUser", Password = AuthUtils.GeneratePasswordHash("correctPassword"), Role = UserRole.Company };
            var loginModel = new UserLoginRequestModel { UserName = "testUser", Password = "wrongPassword" };

            _fixture.UserRepositoryMock
                .Setup(repo => repo.GetUserByUserName(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(company); // Return a concrete Company object

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.AuthenticateAsync(loginModel, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserByUserName(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
        }


        #endregion

        #region 5 - RegisterAsync

        [Theory]
        [InlineData(typeof(AdminRequestModel), typeof(Admin), UserRole.Admin)]
        [InlineData(typeof(CompanyRequestModel), typeof(Company), UserRole.Company)]
        [InlineData(typeof(CustomerRequestModel), typeof(Customer), UserRole.Customer)]
        public async Task RegisterAsync_ShouldRegisterUser_WhenValidDataProvided(Type requestType, Type userType, UserRole expectedRole)
        {
            // Arrange
            var request = (PasswordRequestModel)Activator.CreateInstance(requestType)!;
            request.UserName = "newUser";
            request.Password = "SecurePassword123";

            var user = (User)Activator.CreateInstance(userType)!;
            user.UserName = request.UserName;
            user.Password = AuthUtils.GeneratePasswordHash(request.Password);
            user.Role = expectedRole;
            user.Id = 1;

            _fixture.UserRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _fixture.UserRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _fixture.UnitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.RegisterAsync<PasswordRequestModel, UserResponseModel>(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be("newUser");
            result.Id.Should().Be(1);

            _fixture.UserRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
            _fixture.UserRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once());
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenUserAlreadyExists()
        {
            // Arrange
            var request = new CompanyRequestModel
            {
                UserName = "existingCompany",
                Password = "SecurePassword123"
            };

            _fixture.UserRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);   // User already exists

            // Act & Assert
            await Assert.ThrowsAsync<ObjectAlreadyExistsException>(() =>
                _service.RegisterAsync<CompanyRequestModel, UserResponseModel>(request, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
            _fixture.UserRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never());
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }


        #endregion

        #region 6 - UpdateUserAsync

        [Fact]
        public async Task UpdateUserAsync_ShouldThrowNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var request = new CustomerRequestUpdateModel { UserName = "UpdatedUser", Email = "updated@example.com" };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(null as User);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateUserAsync<CustomerRequestUpdateModel, UserResponseModel>(userId, request, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateAdmin_WhenValidDataProvided()
        {
            // Arrange
            var userId = 1;
            var request = new AdminRequestUpdateModel { UserName = "UpdatedAdmin", Email = "updatedadmin@example.com" };
            var admin = new Admin { Id = 1, UserName = "ExistingAdmin", Email = "admin@example.com" };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(admin);
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateUserAsync<AdminRequestUpdateModel, AdminResponseModel>(userId, request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be("UpdatedAdmin");
            result.Email.Should().Be("updatedadmin@example.com");

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(admin), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateCompany_WhenValidDataProvided()
        {
            // Arrange
            var userId = 1;
            var request = new CompanyRequestUpdateModel { UserName = "UpdatedCompany", Email = "updatedcompany@example.com", Balance = 5000 };
            var company = new Company { Id = 1, UserName = "ExistingCompany", Email = "company@example.com", Balance = 1000 };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateUserAsync<CompanyRequestUpdateModel, CompanyResponseModel>(userId, request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be("UpdatedCompany");
            result.Email.Should().Be("updatedcompany@example.com");
            company.Balance.Should().Be(5000);

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(company), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateCustomer_WhenValidDataProvided()
        {
            // Arrange
            var userId = 1;
            var request = new CustomerRequestUpdateModel { UserName = "UpdatedCustomer", Email = "updatedcustomer@example.com", Balance = 300 };
            var customer = new Customer { Id = 1, UserName = "ExistingCustomer", Email = "customer@example.com", Balance = 100 };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateUserAsync<CustomerRequestUpdateModel, CustomerResponseModel>(userId, request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be("UpdatedCustomer");
            result.Email.Should().Be("updatedcustomer@example.com");
            customer.Balance.Should().Be(300);

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(customer), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldNotUpdateBalance_WhenTypesMismatch()
        {
            // Arrange
            var userId = 1;
            var request = new CustomerRequestUpdateModel { UserName = "UpdatedCompany", Email = "updatedcompany@example.com", Balance = 5000 };
            var company = new Company { Id = 1, UserName = "ExistingCompany", Email = "company@example.com", Balance = 1000 };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateUserAsync<CustomerRequestUpdateModel, CompanyResponseModel>(userId, request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be("UpdatedCompany");
            result.Email.Should().Be("updatedcompany@example.com");
            company.Balance.Should().Be(1000); // Balance should not change due to type mismatch

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(company), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region 7 - UploadCompanyImage

        [Fact]
        public async Task UploadCompanyImage_ShouldUploadAndUpdate_WhenValid()
        {
            // Arrange
            var companyId = 1;
            var fileForm = new ImageRequestModel { File = new Mock<IFormFile>().Object };
            var company = new Company { Id = companyId, UserName = "company1", ImageUrl = "/old/image.jpg" };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(companyId, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fileServiceMock.Setup(fs => fs.UploadImageAsync(fileForm.File, "/company/images", It.IsAny<CancellationToken>())).ReturnsAsync("/new/image.jpg");
            _fixture.UserRepositoryMock.Setup(repo => repo.Update(It.IsAny<Company>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _fileServiceMock.Setup(fs => fs.DeleteImageAsync("/old/image.jpg", "/company/images", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UploadCompanyImage(companyId, fileForm, CancellationToken.None);

            // Assert
            result.Should().Be("/new/image.jpg");
            company.ImageUrl.Should().Be("/new/image.jpg");

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(companyId, It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.UploadImageAsync(fileForm.File, "/company/images", It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(company), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync("/old/image.jpg", "/company/images", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UploadCompanyImage_ShouldThrowNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            var companyId = 1;
            var fileForm = new ImageRequestModel { File = new Mock<IFormFile>().Object };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(companyId, It.IsAny<CancellationToken>())).ReturnsAsync(null as Company);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UploadCompanyImage(companyId, fileForm, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(companyId, It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(It.IsAny<Company>()), Times.Never);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UploadCompanyImage_ShouldDeleteUploadedImage_WhenSaveFails()
        {
            // Arrange
            var companyId = 1;
            var fileForm = new ImageRequestModel { File = new Mock<IFormFile>().Object };
            var company = new Company { Id = companyId, UserName = "company1", ImageUrl = "/old/image.jpg" };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(companyId, It.IsAny<CancellationToken>())).ReturnsAsync(company);
            _fileServiceMock.Setup(fs => fs.UploadImageAsync(fileForm.File, "/company/images", It.IsAny<CancellationToken>())).ReturnsAsync("/new/image.jpg");
            _fixture.UserRepositoryMock.Setup(repo => repo.Update(It.IsAny<Company>()));
            _fixture.UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Save failed"));
            _fileServiceMock.Setup(fs => fs.DeleteImageAsync("/new/image.jpg", "/company/images", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UploadCompanyImage(companyId, fileForm, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(companyId, It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.UploadImageAsync(fileForm.File, "/company/images", It.IsAny<CancellationToken>()), Times.Once);
            _fixture.UserRepositoryMock.Verify(repo => repo.Update(company), Times.Once);
            _fixture.UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync("/new/image.jpg", "/company/images", It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(fs => fs.DeleteImageAsync("/old/image.jpg", "/company/images", It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region 8 - GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCustomer_WhenCustomerExists()
        {
            // Arrange
            var userId = 1;
            var customer = new Customer { Id = userId, UserName = "customer1", Email = "customer1@example.com", Balance = 100 };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(customer);

            // Act
            var result = await _service.GetByIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CustomerResponseModel>();
            result.Id.Should().Be(userId);
            result.UserName.Should().Be("customer1");
            result.Email.Should().Be("customer1@example.com");
            ((CustomerResponseModel)result).Balance.Should().Be(100);

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCompany_WhenCompanyExists()
        {
            // Arrange
            var userId = 1;
            var company = new Company { Id = userId, UserName = "company1", Email = "company1@example.com", Balance = 1000, IsActivated = true };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(company);

            // Act
            var result = await _service.GetByIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CompanyResponseModel>();
            result.Id.Should().Be(userId);
            result.UserName.Should().Be("company1");
            result.Email.Should().Be("company1@example.com");
            ((CompanyResponseModel)result).Balance.Should().Be(1000);
            ((CompanyResponseModel)result).IsActivated.Should().BeTrue();

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnAdmin_WhenAdminExists()
        {
            // Arrange
            var userId = 1;
            var admin = new Admin { Id = userId, UserName = "admin1", Email = "admin1@example.com" };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(admin);

            // Act
            var result = await _service.GetByIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AdminResponseModel>();
            result.Id.Should().Be(userId);
            result.UserName.Should().Be("admin1");
            result.Email.Should().Be("admin1@example.com");

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            _fixture.UserRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(null as User);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(userId, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }


        #endregion

        #region 9 - GetCustomerWithCategories

        [Fact]
        public async Task GetCustomerWithCategories_ShouldReturnCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer
            {
                Id = customerId,
                UserName = "customer1",
                SelectedCategories = new List<Category>
        {
            new Category { Id = 1, Name = "Cat1" },
            new Category { Id = 2, Name = "Cat2" }
        }
            };
            _fixture.UserRepositoryMock.Setup(repo => repo.GetUserWithCategoriesAsync(customerId, It.IsAny<CancellationToken>())).ReturnsAsync(customer);

            // Act
            var result = await _service.GetCustomerWithCategories(customerId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(customerId);
            result.UserName.Should().Be("customer1");
            result.SelectedCategories.Should().HaveCount(2);
            result.SelectedCategories.Should().Contain(c => c.Id == 1 && c.Name == "Cat1");
            result.SelectedCategories.Should().Contain(c => c.Id == 2 && c.Name == "Cat2");

            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserWithCategoriesAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCustomerWithCategories_ShouldThrowNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = 1;
            _fixture.UserRepositoryMock.Setup(repo => repo.GetUserWithCategoriesAsync(customerId, It.IsAny<CancellationToken>())).ReturnsAsync(null as Customer);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCustomerWithCategories(customerId, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserWithCategoriesAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCustomerWithCategories_ShouldThrowNotFound_WhenUserIsNotCustomer()
        {
            // Arrange
            var customerId = 1;
            var company = new Company { Id = customerId, UserName = "company1" }; // Not a Customer
            _fixture.UserRepositoryMock.Setup(repo => repo.GetUserWithCategoriesAsync(customerId, It.IsAny<CancellationToken>())).ReturnsAsync(company);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCustomerWithCategories(customerId, CancellationToken.None));

            _fixture.UserRepositoryMock.Verify(repo => repo.GetUserWithCategoriesAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);
        }


        #endregion

        #region 10 - GenerateUser

        private static User InvokeGenerateUser(UserRequestModel model)
        {
            var method = typeof(UserService).GetMethod("GenerateUser", BindingFlags.NonPublic | BindingFlags.Static);
            return (User)method.Invoke(null, new object[] { model });
        }

        [Fact]
        public void GenerateUser_ShouldReturnAdmin_WhenAdminRequestModel()
        {
            // Arrange
            var model = new AdminRequestModel { UserName = "admin1", Email = "admin1@example.com" };

            // Act
            var result = InvokeGenerateUser(model);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<Admin>();
                var admin = (Admin)result;
                admin.UserName.Should().Be("admin1");
                admin.Email.Should().Be("admin1@example.com");
            }
        }

        [Fact]
        public void GenerateUser_ShouldReturnCompany_WhenCompanyRequestModel()
        {
            // Arrange
            var model = new CompanyRequestModel { UserName = "company1", Email = "company1@example.com" };

            // Act
            var result = InvokeGenerateUser(model);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<Company>();
                var company = (Company)result;
                company.UserName.Should().Be("company1");
                company.Email.Should().Be("company1@example.com");
            }
        }

        [Fact]
        public void GenerateUser_ShouldReturnCustomer_WhenCustomerRequestModel()
        {
            // Arrange
            var model = new CustomerRequestModel { UserName = "customer1", Email = "customer1@example.com" };

            // Act
            var result = InvokeGenerateUser(model);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<Customer>();
                var customer = (Customer)result;
                customer.UserName.Should().Be("customer1");
                customer.Email.Should().Be("customer1@example.com");
            }
        }

        [Fact]
        public void GenerateUser_ShouldThrowArgumentException_WhenInvalidModelType()
        {
            // Arrange
            var model = new UserRequestModel { UserName = "user1", Email = "user1@example.com" }; // Base type

            // Act & Assert
            var exception = Assert.Throws<TargetInvocationException>(() => InvokeGenerateUser(model));
            exception.InnerException.Should().BeOfType<ArgumentException>();
        }

        #endregion

    }
}

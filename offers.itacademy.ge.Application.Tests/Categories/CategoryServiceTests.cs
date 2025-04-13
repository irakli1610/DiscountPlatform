using FluentAssertions;
using FluentAssertions.Execution;
using Mapster;
using Moq;
using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Application.Models.Categories;
using offers.itacademy.ge.Application.Repositories;
using offers.itacademy.ge.Application.Services;
using offers.itacademy.ge.Domain.Categories;
using System.Linq.Expressions;
namespace offers.itacademy.ge.Application.Tests.Categories
{

    public class CategoryServiceTests :  IClassFixture<UnitOfWorkFixture>
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests(UnitOfWorkFixture fixture)
        {


            _unitOfWorkMock = fixture.UnitOfWorkMock;
            _categoryRepositoryMock = fixture.CategoryRepositoryMock;
            _categoryService = new CategoryService(_unitOfWorkMock.Object);

            _unitOfWorkMock.Invocations.Clear();
            _categoryRepositoryMock.Invocations.Clear();
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnList_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
        {
            new() { Id = 1, Name = "Electronics" },
            new() { Id = 2, Name = "Clothing" }
        };

            _categoryRepositoryMock
                .Setup(repo => repo.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync(1, 10, CancellationToken.None);

            // Assert
            using( new AssertionScope())
            /*
             * Normally, if one assertion fails (e.g., result.Should().NotBeNull()), 
             * the test stops immediately, and you don’t see results for subsequent assertions.
             * With AssertionScope, all assertions within the scope are executed, and if any fail, 
             * FluentAssertions collects all failures into a single exception message, making debugging easier.
              */
            {
                result.Should().NotBeNull().And.HaveCount(2);
                result[0].Name.Should().Be("Electronics");
                result[1].Name.Should().Be("Clothing");
               
            }
           _categoryRepositoryMock.Verify(x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once());

        }


        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnEmptyList_WhenNoCategoriesExist()
        {
            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Category>());
            
            var result = await _categoryService.GetAllCategoriesAsync(1, 10, CancellationToken.None);
            
            result.Should().BeEmpty();
            _categoryRepositoryMock.Verify(x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once());

        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Electronics" };

            _categoryRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(1, CancellationToken.None);

            // Assert
            using(new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Id.Should().Be(1);
                result.Name.Should().Be("Electronics");
            }
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),Times.Once());
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            _categoryRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Category);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _categoryService.GetCategoryByIdAsync(1, CancellationToken.None));
            
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldReturnCreatedCategory_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryRequest = new CategoryRequestModel { Name = "New Category" };
            var category = categoryRequest.Adapt<Category>();
            category.Id = 1;

            _categoryRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny< Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _categoryRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _categoryService.CreateCategoryAsync(categoryRequest, CancellationToken.None);

            // Assert
            using(new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Id.Should().Be(1);
                result.Name.Should().Be("New Category");
            }

            _categoryRepositoryMock.Verify(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
            _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldThrowException_WhenCategoryAlreadyExists()
        {
            // Arrange
            var categoryRequest = new CategoryRequestModel { Name = "Existing Category" };

            _categoryRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ObjectAlreadyExistsException>(
                () => _categoryService.CreateCategoryAsync(categoryRequest, CancellationToken.None));
            
            _categoryRepositoryMock.Verify(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
        }


        [Fact]
        public async Task CreateCategoryAsync_ShouldBeCaseInsensitive_WhenCheckingExistingCategory()
        {
            _categoryRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var categoryRequest = new CategoryRequestModel { Name = "electronics" };
            await Assert.ThrowsAsync<ObjectAlreadyExistsException>(() => _categoryService.CreateCategoryAsync(categoryRequest, CancellationToken.None));

            _categoryRepositoryMock.Verify(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());

        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldDelete_WhenCategoryExists()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Category to Delete" };

            _categoryRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            // Act
            await _categoryService.DeleteCategoryAsync(1, CancellationToken.None);

            // Assert
            _categoryRepositoryMock.Verify(repo => repo.Remove(category), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            _categoryRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Category);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _categoryService.DeleteCategoryAsync(1, CancellationToken.None));

            _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldReturnUpdatedCategory_WhenCategoryExists()
        {
            // Arrange
            var existingCategory = new Category { Id = 1, Name = "Old Name" };
            var updatedRequest = new CategoryRequestModel { Name = "Updated Name" };

            _categoryRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            _categoryRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _categoryRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<Category>()))
                .Returns(existingCategory);

            // Act
            var result = await _categoryService.UpdateCategoryAsync(1, updatedRequest, CancellationToken.None);

            // Assert
            using(new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Id.Should().Be(1);
                result.Name.Should().Be("Updated Name");
            }
            _categoryRepositoryMock.Verify(x=>x.GetByIdAsync(1, It.IsAny<CancellationToken>()),Times.Once());
            _categoryRepositoryMock.Verify(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            _categoryRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Category);

            var updateRequest = new CategoryRequestModel { Name = "Updated Name" };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _categoryService.UpdateCategoryAsync(1, updateRequest, CancellationToken.None));
            
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once());

        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldThrowObjectAlreadyExistsException_WhenNameAlreadyExists()
        {
            // Arrange
            var existingCategory = new Category { Id = 1, Name = "Old Name" };
            var updateRequest = new CategoryRequestModel { Name = "Updated Name" };

            _categoryRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            _categoryRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ObjectAlreadyExistsException>(
                () => _categoryService.UpdateCategoryAsync(1, updateRequest, CancellationToken.None));

            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once());
            _categoryRepositoryMock.Verify(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());

        }

    }
}


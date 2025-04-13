using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Services.ImageServices;

namespace offers.itacademy.ge.Application.Tests.Images
{
    public class ImageServiceTests
    {
        private readonly Mock<IOptions<ImageUploadSettings>> _optionsMock;
        private readonly ImageService _service;

        public ImageServiceTests()
        {
            _optionsMock = new Mock<IOptions<ImageUploadSettings>>();

            _optionsMock.Setup(o => o.Value).Returns(new ImageUploadSettings
            {
                CompanyImagePath = Path.Combine(Path.GetTempPath(), "company_images"),
                OfferImagePath = Path.Combine(Path.GetTempPath(), "offer_images"),
                MaxFileSize = 5 * 1024 * 1024, // 5MB
                AllowedExtensions = [".jpg", ".png"],
                DefaultCompanyImagePath = Path.Combine(Path.GetTempPath(), "default_company.jpg"),
                DefaultOfferImagePath = Path.Combine(Path.GetTempPath(), "default_offer.jpg")
            });

            _service = new ImageService(_optionsMock.Object);

            Directory.CreateDirectory(_optionsMock.Object.Value.CompanyImagePath);
            Directory.CreateDirectory(_optionsMock.Object.Value.OfferImagePath);
        }

        #region UploadImageAsync

        [Fact]
        public async Task UploadImageAsync_ShouldUploadAndReturnPath_WhenFileIsValid()
        {
            var uploadPath = _optionsMock.Object.Value.CompanyImagePath;
            var fileMock = new Mock<IFormFile>();

            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) =>
                {
                    using (var ms = new MemoryStream(new byte[1024]))
                        return ms.CopyToAsync(stream, token);
                });

            var result = await _service.UploadImageAsync(fileMock.Object, uploadPath, CancellationToken.None);

            result.Should().NotBeNullOrEmpty();
            result.Should().EndWith(".jpg");
            File.Exists(Path.Combine(uploadPath, result)).Should().BeTrue();

            Cleanup(Path.Combine(uploadPath, result));
        }

        [Fact]
        public async Task UploadImageAsync_ShouldThrowFileUploadException_WhenFileIsNull()
        {
            var uploadPath = _optionsMock.Object.Value.CompanyImagePath;

            await Assert.ThrowsAsync<FileUploadException>(() =>
                _service.UploadImageAsync(file: null, uploadPath, CancellationToken.None));
        }

        [Fact]
        public async Task UploadImageAsync_ShouldThrowFileUploadException_WhenFileIsEmpty()
        {
            var uploadPath = _optionsMock.Object.Value.CompanyImagePath;
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);
            fileMock.Setup(f => f.FileName).Returns("test.jpg");

            await Assert.ThrowsAsync<FileUploadException>(() =>
                _service.UploadImageAsync(fileMock.Object, uploadPath, CancellationToken.None));
        }

        [Fact]
        public async Task UploadImageAsync_ShouldThrowFileUploadException_WhenFileExceedsMaxSize()
        {
            var uploadPath = _optionsMock.Object.Value.CompanyImagePath;
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(6 * 1024 * 1024);
            fileMock.Setup(f => f.FileName).Returns("test.jpg");

            await Assert.ThrowsAsync<FileUploadException>(() =>
                _service.UploadImageAsync(fileMock.Object, uploadPath, CancellationToken.None));
        }

        [Fact]
        public async Task UploadImageAsync_ShouldThrowFileUploadException_WhenExtensionIsInvalid()
        {
            var uploadPath = _optionsMock.Object.Value.CompanyImagePath;
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.FileName).Returns("test.exe");

            await Assert.ThrowsAsync<FileUploadException>(() =>
                _service.UploadImageAsync(fileMock.Object, uploadPath, CancellationToken.None));
        }

        [Fact]
        public async Task UploadImageAsync_ShouldThrowFileUploadException_WhenCopyToAsyncFails()
        {
            // Arrange
            var uploadPath = _optionsMock.Object.Value.CompanyImagePath;
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new IOException("Simulated file copy failure")); // Simulate failure

            // Act & Assert
            await Assert.ThrowsAsync<FileUploadException>(() =>
                _service.UploadImageAsync(fileMock.Object, uploadPath, CancellationToken.None));
        }

        [Fact]
        public async Task UploadImageAsync_ShouldCreateDirectories_WhenTheyDoNotExist()
        {
            // Arrange
            var tempPath = Path.GetTempPath();
            var customCompanyPath = Path.Combine(tempPath, "test_company_images");
            var customOfferPath = Path.Combine(tempPath, "test_offer_images");
            var uploadPath = customCompanyPath;

            // Clean up any existing directories
            if (Directory.Exists(customCompanyPath)) Directory.Delete(customCompanyPath, true);
            if (Directory.Exists(customOfferPath)) Directory.Delete(customOfferPath, true);

            // Update options mock with new paths
            _optionsMock.Setup(o => o.Value).Returns(new ImageUploadSettings
            {
                CompanyImagePath = customCompanyPath,
                OfferImagePath = customOfferPath,
                MaxFileSize = 5 * 1024 * 1024,
                AllowedExtensions = [".jpg", ".png"],
                DefaultCompanyImagePath = Path.Combine(tempPath, "default_company.jpg"),
                DefaultOfferImagePath = Path.Combine(tempPath, "default_offer.jpg")
            });

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) =>
                {
                    using (var ms = new MemoryStream(new byte[1024]))
                        return ms.CopyToAsync(stream, token);
                });

            // Act
            var result = await _service.UploadImageAsync(fileMock.Object, uploadPath, CancellationToken.None);

            // Assert
            Directory.Exists(customCompanyPath).Should().BeTrue();
            Directory.Exists(customOfferPath).Should().BeTrue();
            result.Should().NotBeNullOrEmpty();
            result.Should().EndWith(".jpg");
            File.Exists(Path.Combine(uploadPath, result)).Should().BeTrue();

            // Cleanup
            Cleanup(Path.Combine(uploadPath, result));
            Directory.Delete(customCompanyPath, true);
            Directory.Delete(customOfferPath, true);
        }

        #endregion

        #region DeleteImageAsync

        [Fact]
        public async Task DeleteImageAsync_ShouldDeleteFile_WhenFileExistsAndNotDefault()
        {
            var basePath = _optionsMock.Object.Value.CompanyImagePath;
            var fileName = "test_delete.jpg";
            var filePath = Path.Combine(basePath, fileName);
            await File.WriteAllBytesAsync(filePath, new byte[1024]);

            await _service.DeleteImageAsync(fileName, basePath, CancellationToken.None);

            File.Exists(filePath).Should().BeFalse();
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldNotThrow_WhenFileDoesNotExist()
        {
            var basePath = _optionsMock.Object.Value.CompanyImagePath;
            var fileName = "nonexistent.jpg";
            var filePath = Path.Combine(basePath, fileName);

            await _service.DeleteImageAsync(fileName, basePath, CancellationToken.None);
            File.Exists(filePath).Should().BeFalse();
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldNotDelete_WhenFileIsDefaultCompanyImage()
        {
            var basePath = Path.GetTempPath();
            var fileName = "default_company.jpg";
            var filePath = Path.Combine(basePath, fileName);
            await File.WriteAllBytesAsync(filePath, new byte[1024]);

            await _service.DeleteImageAsync(fileName, basePath, CancellationToken.None);

            File.Exists(filePath).Should().BeTrue();

            Cleanup(filePath);
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldNotDelete_WhenFileIsDefaultOfferImage()
        {
            var basePath = Path.GetTempPath();
            var fileName = "default_offer.jpg";
            var filePath = Path.Combine(basePath, fileName);
            await File.WriteAllBytesAsync(filePath, new byte[1024]);

            await _service.DeleteImageAsync(fileName, basePath, CancellationToken.None);

            File.Exists(filePath).Should().BeTrue();

            Cleanup(filePath);
        }

        #endregion

        private static void Cleanup(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
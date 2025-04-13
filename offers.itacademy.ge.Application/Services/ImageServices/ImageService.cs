using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Exceptions.Resources;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;

namespace offers.itacademy.ge.Application.Services.ImageServices
{
    public class ImageService : IFileService
    {
        private readonly IOptions<ImageUploadSettings> _options;
        public ImageService(IOptions<ImageUploadSettings> options)
        {
            _options = options;
        }


        public async Task<string> UploadImageAsync(IFormFile file, string uploadPath, CancellationToken token)
        {
            if (file == null || file.Length == 0)
                throw new FileUploadException(AppExceptionMessages.InvalidFile);

            if (file.Length > _options.Value.MaxFileSize)
                throw new FileUploadException(AppExceptionMessages.FileSizeLimitViolation);

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (fileExtension == null || !_options.Value.AllowedExtensions.Contains(fileExtension))
                throw new FileUploadException(AppExceptionMessages.InvalidFileType);

            //ensuring that directory exists
            if (!Directory.Exists(_options.Value.CompanyImagePath) || !Directory.Exists(_options.Value.OfferImagePath))
            {
                Directory.CreateDirectory(_options.Value.CompanyImagePath);
                Directory.CreateDirectory(_options.Value.OfferImagePath);
            }

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream, token);
                }
            }
            catch
            {
                throw new FileUploadException(AppExceptionMessages.ImageUploadException);
            }

            return fileName; // Return only the file name
        }

        public async Task DeleteImageAsync(string fileName, string basePath, CancellationToken token)
        {
            string imagePath = Path.Combine(basePath, fileName);

            string normalizedImagePath = Path.GetFullPath(imagePath).Replace('\\', '/');
            string normalizedDefaultCompanyPath = Path.GetFullPath(_options.Value.DefaultCompanyImagePath).Replace('\\', '/');
            string normalizedDefaultOfferPath = Path.GetFullPath(_options.Value.DefaultOfferImagePath).Replace('\\', '/');

            if (File.Exists(imagePath) &&
                normalizedImagePath != normalizedDefaultCompanyPath &&
                normalizedImagePath != normalizedDefaultOfferPath)
            {
                await Task.Run(() => File.Delete(imagePath), token);
            }
        }

    }
}

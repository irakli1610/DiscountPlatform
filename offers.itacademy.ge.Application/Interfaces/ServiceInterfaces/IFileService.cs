using Microsoft.AspNetCore.Http;

namespace offers.itacademy.ge.Application.Interfaces.ServiceInterfaces
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file, string uploadPath, CancellationToken token);
        Task DeleteImageAsync(string fileName, string basePath, CancellationToken token);
    }
}

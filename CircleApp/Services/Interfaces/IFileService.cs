using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CircleApp.Services.Interfaces
{
    public interface IFileService
    {
        Task<string?> UploadFileAsync(IFormFile? file, string subFolder, string? oldRelativePath = null, string[]? allowedExtensions = null, long maxFileSizeBytes = 5 * 1024 * 1024);
        Task<string?> ReplaceFileAsync(IFormFile? newFile, string subFolder, string? oldRelativePath, string[]? allowedExtensions = null, long maxFileSizeBytes = 5 * 1024 * 1024);
        void DeleteFile(string? relativePath);
        bool ValidateExtension(IFormFile file, string[]? allowedExtensions = null);
        bool ValidateSize(IFormFile file, long maxFileSizeBytes = 5 * 1024 * 1024);
    }
}

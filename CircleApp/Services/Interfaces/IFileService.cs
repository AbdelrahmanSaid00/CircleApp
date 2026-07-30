using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CircleApp.Services.Interfaces
{
    public interface IFileService
    {
        Task<string?> UploadFileAsync(IFormFile? file, string subFolder);
        void DeleteFile(string? relativePath);
    }
}

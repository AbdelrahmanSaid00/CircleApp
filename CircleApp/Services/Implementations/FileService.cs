using CircleApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CircleApp.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }

        public async Task<string?> UploadFileAsync(IFormFile? file, string subFolder)
        {
            if (file == null || file.Length == 0) return null;

            try
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string targetDirectory = Path.Combine(rootFolderPath, subFolder.Trim('/', '\\'));
                Directory.CreateDirectory(targetDirectory);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(targetDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string relativePath = "/" + subFolder.Trim('/', '\\').Replace('\\', '/') + "/" + fileName;
                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to subfolder {SubFolder}", subFolder);
                return null;
            }
        }

        public void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            try
            {
                string cleanRelativePath = relativePath.TrimStart('/', '\\');
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanRelativePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting physical file at {RelativePath}", relativePath);
            }
        }
    }
}

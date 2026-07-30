using CircleApp.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileService> _logger;

        private static readonly string[] DefaultAllowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg" };

        public FileService(IWebHostEnvironment webHostEnvironment, ILogger<FileService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<string?> UploadFileAsync(
            IFormFile? file,
            string subFolder,
            string? oldRelativePath = null,
            string[]? allowedExtensions = null,
            long maxFileSizeBytes = 5 * 1024 * 1024)
        {
            if (file == null || file.Length == 0) return null;

            if (!ValidateExtension(file, allowedExtensions))
            {
                _logger.LogWarning("File upload rejected: Invalid file extension for file {FileName}", file.FileName);
                return null;
            }

            if (!ValidateSize(file, maxFileSizeBytes))
            {
                _logger.LogWarning("File upload rejected: File size {Length} exceeds max allowed size {MaxSize} bytes", file.Length, maxFileSizeBytes);
                return null;
            }

            try
            {
                // Delete old image if specified
                if (!string.IsNullOrWhiteSpace(oldRelativePath))
                {
                    DeleteFile(oldRelativePath);
                }

                string webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
                string targetDirectory = Path.Combine(webRootPath, subFolder.Trim('/', '\\'));
                Directory.CreateDirectory(targetDirectory);

                string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                string fileName = Guid.NewGuid().ToString() + fileExtension;
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
                _logger.LogError(ex, "Error uploading file {FileName} to subfolder {SubFolder}", file.FileName, subFolder);
                return null;
            }
        }

        public async Task<string?> ReplaceFileAsync(
            IFormFile? newFile,
            string subFolder,
            string? oldRelativePath,
            string[]? allowedExtensions = null,
            long maxFileSizeBytes = 5 * 1024 * 1024)
        {
            return await UploadFileAsync(newFile, subFolder, oldRelativePath, allowedExtensions, maxFileSizeBytes);
        }

        public void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            try
            {
                string webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
                string cleanRelativePath = relativePath.TrimStart('/', '\\');
                string fullPath = Path.Combine(webRootPath, cleanRelativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("Deleted file at {FullPath}", fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting physical file at {RelativePath}", relativePath);
            }
        }

        public bool ValidateExtension(IFormFile file, string[]? allowedExtensions = null)
        {
            if (file == null || string.IsNullOrWhiteSpace(file.FileName)) return false;

            var extensionsToUse = allowedExtensions ?? DefaultAllowedExtensions;
            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return extensionsToUse.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public bool ValidateSize(IFormFile file, long maxFileSizeBytes = 5 * 1024 * 1024)
        {
            if (file == null) return false;
            return file.Length > 0 && file.Length <= maxFileSizeBytes;
        }
    }
}

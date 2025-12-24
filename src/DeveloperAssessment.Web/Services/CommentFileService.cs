using DeveloperAssessment.Common.Models.Blog;
using DeveloperAssessment.Services.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace DeveloperAssessment.Services.Services
{
    public class CommentFileService : ICommentFileService
    {
        private const long MaxBytes = 5 * 1024 * 1024; // 5MB limitation - pretty normal
        private static readonly HashSet<string> AllowedExtensions =
        [
            ".png",
            ".jpg"
        ];

        private readonly IWebHostEnvironment _env;

        public CommentFileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<CommentAttachmentItem> SaveAsync(IFormFile file)
        {
            if (file is null || file.Length == 0) 
            {
                throw new InvalidOperationException("No file uploaded.");
            }

            if (file.Length > MaxBytes) 
            {
                throw new InvalidOperationException("File too large (max 5MB).");
            }

            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext)) 
            {
                throw new InvalidOperationException($"File type '{ext}' is not allowed.");
            }

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "comments");
            Directory.CreateDirectory(uploadsDir);
            var storedFileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsDir, storedFileName);

            await using (var stream = File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }

            return new CommentAttachmentItem
            {
                OriginalFileName = Path.GetFileName(file.FileName),
                StoredFileName = storedFileName,
                RelativeUrl = $"/uploads/comments/{storedFileName}",
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                SizeBytes = file.Length
            };
        }
    }
}

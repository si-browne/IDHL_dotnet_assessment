using DeveloperAssessment.Common.Models.Blog;

namespace DeveloperAssessment.Services.Contracts
{
    // (ISP) Interface Segregation Principle - present here, Exercise 5
    public interface ICommentFileService
    {
        Task<CommentAttachmentItem> SaveAsync(IFormFile file);
    }
}

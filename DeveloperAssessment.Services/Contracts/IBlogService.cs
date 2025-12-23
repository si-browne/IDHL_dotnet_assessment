
using DeveloperAssessment.Common.Models.Blog;

namespace DeveloperAssessment.Services.Contracts
{
    public interface IBlogService
    {
        // (ISP) - This interface is segregated to a degree where I believe its clean enough
        Task<IReadOnlyList<BlogPostItem>> GetPostsAsync();
        Task<BlogPostItem?> GetPostByIdAsync(int id);
        Task AddCommentAsync(int postId, CommentItem comment);
    }
}

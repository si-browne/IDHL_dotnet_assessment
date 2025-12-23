using DeveloperAssessment.Common.Models.Blog;

namespace DeveloperAssessment.Common.ViewModels
{
    public class BlogPostPageViewModel
    {
        public BlogPostItem Post { get; set; } = new();
        public CommentInputModel NewComment { get; set; } = new();
    }
}

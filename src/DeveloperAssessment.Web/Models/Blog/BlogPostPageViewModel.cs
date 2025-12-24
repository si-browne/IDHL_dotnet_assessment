using DeveloperAssessment.Common.Models.Blog;

namespace DeveloperAssessment.Web.Models.Blog
{
    public class BlogPostPageViewModel
    {
        public BlogPostItem Post { get; set; } = new();
        public CommentInputModel NewComment { get; set; } = new();
        public ReplyInputModel NewReply { get; set; } = new();
    }
}

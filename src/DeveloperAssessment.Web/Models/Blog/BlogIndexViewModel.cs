using DeveloperAssessment.Common.Models.Blog;

namespace DeveloperAssessment.Web.Models.Blog
{
    public class BlogIndexViewModel
    {
        public IReadOnlyList<BlogPostItem> Posts { get; set; } = Array.Empty<BlogPostItem>();

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrev => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}

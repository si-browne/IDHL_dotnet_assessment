using System.Text.Json.Serialization;

namespace DeveloperAssessment.Common.Models.Blog
{
    public class BlogPostDocument
    {
        [JsonPropertyName("blogPosts")]
        public List<BlogPostItem> BlogPosts { get; set; } = new();
    }
}

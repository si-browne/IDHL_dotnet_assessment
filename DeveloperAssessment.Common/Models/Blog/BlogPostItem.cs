using DeveloperAssessment.Common.Base;
using System.Text.Json.Serialization;

namespace DeveloperAssessment.Common.Models.Blog
{
    public class BlogPostItem : ItemBase
    {

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("htmlContent")]
        public string HtmlContent { get; set; } = "";

        [JsonPropertyName("comments")]
        public List<CommentItem> Comments { get; set; } = new();
    }
}

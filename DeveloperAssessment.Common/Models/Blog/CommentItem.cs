using DeveloperAssessment.Common.Base;
using System.Text.Json.Serialization;

namespace DeveloperAssessment.Common.Models.Blog
{
    public class CommentItem : ItemBase
    {

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; } = "";

        [JsonPropertyName("message")]
        public string Message { get; set; } = "";

        // Exercise 4
        [JsonPropertyName("replies")]
        public List<CommentReplyItem> Replies { get; set; } = new();

        // Exercise 5
        [JsonPropertyName("attachments")]
        public List<CommentAttachmentItem> Attachments { get; set; } = new();
    }
}

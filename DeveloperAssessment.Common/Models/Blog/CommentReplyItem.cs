using DeveloperAssessment.Common.Base;
using System.Text.Json.Serialization;

namespace DeveloperAssessment.Common.Models.Blog
{
    public class CommentReplyItem : ItemBase
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; } = "";

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = "";
    }
}

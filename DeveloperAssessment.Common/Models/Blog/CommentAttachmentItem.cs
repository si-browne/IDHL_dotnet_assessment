using System.Text.Json.Serialization;

namespace DeveloperAssessment.Common.Models.Blog
{
    public class CommentAttachmentItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("originalFileName")]
        public string OriginalFileName { get; set; } = "";

        [JsonPropertyName("storedFileName")]
        public string StoredFileName { get; set; } = "";

        [JsonPropertyName("relativeUrl")]
        public string RelativeUrl { get; set; } = "";

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; } = "";

        [JsonPropertyName("sizeBytes")]
        public long SizeBytes { get; set; }
    }
}

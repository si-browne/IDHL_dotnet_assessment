using System.Text.Json.Serialization;

namespace DeveloperAssessment.Common.Base
{
    public abstract class ItemBase
    {
        // (LSP) - Liskov present here through usage of abstract base class, substitutability is present to a basic degree
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}

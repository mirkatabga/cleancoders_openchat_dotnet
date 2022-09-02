using System.Text.Json.Serialization;

namespace OpenChat.Api.Models
{
    public class PostResponse
    {
        public Guid? PostId { get; set; }

        public Guid? UserId { get; set; }

        public string? Text { get; set; }

        [JsonPropertyName("dateTime")]
        public DateTime? CreatedDate { get; set; }
    }
}
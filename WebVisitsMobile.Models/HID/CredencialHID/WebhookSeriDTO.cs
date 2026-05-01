using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.CredencialHID
{
    public class WebhookSeriDTO
    {
        [JsonPropertyName("specversion")]
        public string SpecVersion { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("data")]
        public JsonElement Data { get; set; }

        [JsonPropertyName("datacontenttype")]
        public string DataContentType { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }
    }
}
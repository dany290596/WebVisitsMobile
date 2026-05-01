using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class SearchUserHIDDTO
    {
        [JsonPropertyName("schemas")]
        public List<string> Schemas { get; set; }

        [JsonPropertyName("attributes")]
        public List<string> Attributes { get; set; }

        [JsonPropertyName("filter")]
        public string Filter { get; set; }

        [JsonPropertyName("sortOrder")]
        public string SortOrder { get; set; }

        [JsonPropertyName("startIndex")]
        public int StartIndex { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("Resources")]
        public List<ScimUserResourceDto>? Resources { get; set; }
    }
}
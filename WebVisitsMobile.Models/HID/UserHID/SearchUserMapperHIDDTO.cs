using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class SearchUserMapperHIDDTO
    {
        [JsonPropertyName("schemas")]
        public List<string> Schemas { get; set; }

        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonPropertyName("startIndex")]
        public int StartIndex { get; set; }

        [JsonPropertyName("Resources")]
        public List<ScimUserResourceDto> Resources { get; set; }
    }

    public class ScimUserResourceDto
    {
        [JsonPropertyName("meta")]
        public ScimMetaDto Meta { get; set; }

        [JsonPropertyName("name")]
        public ScimNameDto Name { get; set; }

        [JsonPropertyName("emails")]
        public List<ScimEmailDto> Emails { get; set; }
    }

    public class ScimMetaDto
    {
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
    }

    public class ScimNameDto
    {
        [JsonPropertyName("familyName")]
        public string FamilyName { get; set; }

        [JsonPropertyName("givenName")]
        public string GivenName { get; set; }
    }

    public class ScimEmailDto
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
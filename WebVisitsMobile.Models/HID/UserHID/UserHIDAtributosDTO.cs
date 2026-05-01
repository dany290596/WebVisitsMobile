using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class UserHIDAtributosDTO
    {
        [JsonPropertyName("meta")]
        public MetaUserHIDAtributos Meta { get; set; }

        [JsonPropertyName("schemas")]
        public string[] Schemas { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("externalId")]
        public string ExternalId { get; set; }

        [JsonPropertyName("name")]
        public NameUserHIDAtributos Name { get; set; }

        [JsonPropertyName("emails")]
        public List<EmailUserHIDAtributos> Emails { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class MetaUserHIDAtributos
    {
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
    }

    public class NameUserHIDAtributos
    {
        [JsonPropertyName("familyName")]
        public string FamilyName { get; set; }

        [JsonPropertyName("givenName")]
        public string GivenName { get; set; }
    }

    public class EmailUserHIDAtributos
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
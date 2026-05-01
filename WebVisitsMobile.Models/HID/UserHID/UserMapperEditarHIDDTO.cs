using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class UserMapperEditarHIDDTO
    {
        [JsonPropertyName("schemas")]
        public string[] Schemas { get; set; }

        [JsonPropertyName("externalId")]
        public string ExternalId { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("emails")]
        public List<Email> Emails { get; set; }

        [JsonPropertyName("meta")]
        public MetaUserHIDEditar Meta { get; set; }
    }

    public class MetaUserHIDEditar
    {
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }
    }
}
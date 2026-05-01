using Newtonsoft.Json;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class SDKVersionDTO
    {
        [JsonProperty("schemas")]
        public List<string> Schemas { get; set; }

        [JsonProperty("urn:hid:scim:api:ma:2.2:SDKVersion")]
        public List<VersionDetail> SDKVersions { get; set; }

        [JsonProperty("meta")]
        public MetaSDKVersion Meta { get; set; }
    }

    public class VersionDetail
    {
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class MetaSDKVersion
    {
        [JsonProperty("resourceType")]
        public string ResourceType { get; set; }

        [JsonProperty("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }
    }
}
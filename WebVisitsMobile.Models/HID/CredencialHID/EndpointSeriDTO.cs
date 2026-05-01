using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.CredencialHID
{
    public class EndpointSeriDTO
    {
        [JsonPropertyName("appletStatus")]
        public string AppletStatus { get; set; }

        [JsonPropertyName("applicationVersion")]
        public string ApplicationVersion { get; set; }

        [JsonPropertyName("bleCapability")]
        public string BleCapability { get; set; }

        [JsonPropertyName("cardletVersion")]
        public string CardletVersion { get; set; }

        [JsonPropertyName("endpointId")]
        public string EndpointId { get; set; }

        [JsonPropertyName("endpointManufacturer")]
        public string EndpointManufacturer { get; set; }

        [JsonPropertyName("endpointModel")]
        public string EndpointModel { get; set; }

        [JsonPropertyName("hceCapability")]
        public string HceCapability { get; set; }

        [JsonPropertyName("invitationCode")]
        public string InvitationCode { get; set; }

        [JsonPropertyName("invitationExpiryMin")]
        public int InvitationExpiryMin { get; set; }

        [JsonPropertyName("invitationId")]
        public string InvitationId { get; set; }

        [JsonPropertyName("invitationStatus")]
        public string InvitationStatus { get; set; }

        [JsonPropertyName("jailBroken")]
        public string JailBroken { get; set; }

        [JsonPropertyName("mobileSDKVersion")]
        public string MobileSDKVersion { get; set; }

        [JsonPropertyName("nfcCapability")]
        public string NfcCapability { get; set; }

        [JsonPropertyName("organizationId")]
        public string OrganizationId { get; set; }

        [JsonPropertyName("osVersion")]
        public string OsVersion { get; set; }

        [JsonPropertyName("simOperator")]
        public string SimOperator { get; set; }

        [JsonPropertyName("sourceTimestamp")]
        public DateTime SourceTimestamp { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }
    }
}
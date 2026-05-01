using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.CredencialHID
{
    public class InvitationSeriDTO
    {
        [JsonPropertyName("invitationCode")]
        public string InvitationCode { get; set; }

        [JsonPropertyName("invitationExpiryMin")]
        public int InvitationExpiryMin { get; set; }

        [JsonPropertyName("invitationId")]
        public string InvitationId { get; set; }

        [JsonPropertyName("organizationId")]
        public string OrganizationId { get; set; }

        [JsonPropertyName("sourceTimestamp")]
        public DateTime SourceTimestamp { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }
    }
}
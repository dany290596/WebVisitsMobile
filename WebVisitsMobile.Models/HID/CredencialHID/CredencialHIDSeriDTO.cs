using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.CredencialHID
{
    public class CredencialHIDSeriDTO
    {
        [JsonPropertyName("cardNumber")]
        public int CardNumber { get; set; }

        [JsonPropertyName("endpointId")]
        public string EndpointId { get; set; }

        [JsonPropertyName("invitationId")]
        public string InvitationId { get; set; }

        [JsonPropertyName("mid")]
        public int Mid { get; set; }

        [JsonPropertyName("midTypeId")]
        public string MidTypeId { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

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
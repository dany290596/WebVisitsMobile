using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class InvitacionYCredencialHIDDTO
    {
        [JsonPropertyName("schemas")]
        public List<string> Schemas { get; set; }

        [JsonPropertyName("urn:hid:scim:api:ma:2.2:UserInvitation")]
        public List<UserInvitation> UserInvitations { get; set; }

        [JsonPropertyName("urn:hid:scim:api:ma:2.2:Credential")]
        public List<Credential> Credentials { get; set; }
    }

    public class Meta
    {
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
    }

    public class UserInvitation
    {
        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("invitationCode")]
        public string InvitationCode { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("expirationDate")]
        public DateTime ExpirationDate { get; set; }
    }

    public class Credential
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("partNumber")]
        public string PartNumber { get; set; }

        [JsonPropertyName("partnumberFriendlyName")]
        public string PartnumberFriendlyName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("credentialType")]
        public string CredentialType { get; set; }

        [JsonPropertyName("cardNumber")]
        public string CardNumber { get; set; }
    }
}
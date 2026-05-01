using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class UserMapperRespHIDDTO
    {
        [JsonPropertyName("meta")]
        public MetaUserHID Meta { get; set; }

        [JsonPropertyName("schemas")]
        public List<string> Schemas { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("externalId")]
        public string ExternalId { get; set; }

        [JsonPropertyName("name")]
        public NameUserHID Name { get; set; }

        [JsonPropertyName("emails")]
        public List<EmailUserHID> Emails { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("urn:hid:scim:api:ma:2.2:UserInvitation")]
        public List<UserInvitationUserHID> UserInvitations { get; set; }

        [JsonPropertyName("urn:hid:scim:api:ma:2.2:Credential")]
        public List<CredentialUserHID> Credentials { get; set; }
    }

    public class MetaUserHID
    {
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
    }

    public class NameUserHID
    {
        [JsonPropertyName("familyName")]
        public string FamilyName { get; set; }

        [JsonPropertyName("givenName")]
        public string GivenName { get; set; }
    }

    public class EmailUserHID
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class UserInvitationUserHID
    {
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

    public class CredentialUserHID
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
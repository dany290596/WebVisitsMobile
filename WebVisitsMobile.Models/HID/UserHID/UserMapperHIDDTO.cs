using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class UserMapperHIDDTO
    {
        [JsonPropertyName("meta")]
        public MetaUser Meta { get; set; }

        [JsonPropertyName("schemas")]
        public string[] Schemas { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("externalId")]
        public string ExternalId { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("emails")]
        public List<Email> Emails { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("urn:hid:scim:api:ma:2.2:UserInvitation")]
        public List<UserInvitationUser> UserInvitations { get; set; }

        [JsonPropertyName("urn:hid:scim:api:ma:2.2:CredentialContainer")]
        public List<CredentialContainer?> CredentialContainers { get; set; }

        [JsonPropertyName("urn:hid:scim:api:ma:2.2:LicenseInfo")]
        public List<LicenseInfo> LicenseInfo { get; set; }
    }

    public class MetaUser
    {
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
    }

    public class Name
    {
        [JsonPropertyName("familyName")]
        public string FamilyName { get; set; }

        [JsonPropertyName("givenName")]
        public string GivenName { get; set; }
    }

    public class Email
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class UserInvitationUser
    {
        [JsonPropertyName("invitationCode")]
        public string InvitationCode { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("expirationDate")]
        public DateTime ExpirationDate { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class LicenseInfo
    {
        [JsonPropertyName("partNumber")]
        public string PartNumber { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("consumedQty")]
        public int ConsumedQty { get; set; }
    }

    public class CredentialContainer
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("osVersion")]
        public string OsVersion { get; set; }

        [JsonPropertyName("secureElementType")]
        public string SecureElementType { get; set; }

        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("applicationVersion")]
        public string ApplicationVersion { get; set; }

        [JsonPropertyName("simOperator")]
        public string SimOperator { get; set; }

        [JsonPropertyName("bluetoothCapability")]
        public string BluetoothCapability { get; set; }

        [JsonPropertyName("nfcCapability")]
        public string NfcCapability { get; set; }

        [JsonPropertyName("hceCapability")]
        public string HceCapability { get; set; }

        [JsonPropertyName("urn:hid:scim:api:ma:2.2:Credential")]
        public List<CredentialUser> Credentials { get; set; }
    }

    public class CredentialUser
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("partNumber")]
        public string PartNumber { get; set; }

        [JsonPropertyName("partnumberFriendlyName")]
        public string PartnumberFriendlyName { get; set; }

        [JsonPropertyName("cardNumber")]
        public string CardNumber { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("credentialType")]
        public string CredentialType { get; set; }
    }
}
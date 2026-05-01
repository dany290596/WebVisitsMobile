using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class UserMapperCrearHIDDTO
    {
        [JsonPropertyName("schemas")]
        public List<string> Schemas { get; set; } = new List<string>
        {
            "urn:hid:scim:api:ma:2.2:UserAction",
            "urn:ietf:params:scim:schemas:core:2.0:User"
        };

        [JsonPropertyName("externalId")]
        public string ExternalId { get; set; }

        [JsonPropertyName("name")]
        public UserName Name { get; set; }

        [JsonPropertyName("emails")]
        public List<UserEmail> Emails { get; set; } = new List<UserEmail>();

        [JsonPropertyName("urn:hid:scim:api:ma:2.2:UserAction")]
        public UserActionSchema UserAction { get; set; } = new UserActionSchema();

        [JsonPropertyName("meta")]
        public UserMeta Meta { get; set; } = new UserMeta();
    }

    public class UserActionSchema
    {
        [JsonPropertyName("createInvitationCode")]
        public string CreateInvitationCode { get; set; } = "Y";

        [JsonPropertyName("sendInvitationEmail")]
        public string SendInvitationEmail { get; set; } = "N";

        [JsonPropertyName("assignCredential")]
        public string AssignCredential { get; set; } = "Y";

        [JsonPropertyName("partNumber")]
        public string PartNumber { get; set; }

        [JsonPropertyName("credential")]
        public string Credential { get; set; } = string.Empty;
    }

    public class UserMeta
    {
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; } = "PACSUser";
    }

    public class UserName
    {
        [JsonPropertyName("familyName")]
        public string FamilyName { get; set; }

        [JsonPropertyName("givenName")]
        public string GivenName { get; set; }
    }

    public class UserEmail
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
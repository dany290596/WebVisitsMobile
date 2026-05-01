using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class InvitacionYCredencialHIDCrearDTO
    {
        public string[] Schemas { get; set; }

        [JsonPropertyName("urn:hid:scim:api:ma:2.2:UserAction")]
        public UserActionDetails UserActionDetails { get; set; }

        public MetaIYC Meta { get; set; }
    }

    public class UserActionDetails
    {
        public string CreateInvitationCode { get; set; }
        public string SendInvitationEmail { get; set; }
        public string AssignCredential { get; set; }
        public string PartNumber { get; set; }
        public string Credential { get; set; }
    }

    public class MetaIYC
    {
        public string ResourceType { get; set; }
    }
}
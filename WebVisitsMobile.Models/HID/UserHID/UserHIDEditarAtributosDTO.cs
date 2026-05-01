using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class UserHIDEditarAtributosDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [JsonPropertyName("user_ext_id")]
        public string UserExternalId { get; set; }

        [Required]
        [JsonPropertyName("user_family_name")]
        public string UserFamilyName { get; set; }

        [Required]
        [JsonPropertyName("user_given_name")]
        public string UserGivenName { get; set; }

        [Required]
        [EmailAddress]
        [JsonPropertyName("user_email")]
        public string UserEmail { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Models.HID.CredencialHID
{
    public class CredencialHIDReqDTO
    {
        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string? TipoCredencial { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string? CredencialValor { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string? Validity { get; set; }

        public Guid? ExternalId { get; set; }
    }
}
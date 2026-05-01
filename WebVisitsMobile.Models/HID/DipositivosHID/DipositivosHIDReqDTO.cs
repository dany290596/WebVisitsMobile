using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Models.HID.DipositivosHID
{
    public class DipositivosHIDReqDTO
    {
        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string? SistemaOperativo { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string? NombreDispositivo { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string? EndpointId { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string? SdkVersion { get; set; }
    }
}
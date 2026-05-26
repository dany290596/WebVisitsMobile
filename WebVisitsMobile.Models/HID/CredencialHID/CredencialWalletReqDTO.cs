using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Models.HID.CredencialHID
{
    public class CredencialWalletReqDTO
    {
        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public Guid? Usuarioid { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string TipoCredencial { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public Guid ExternalId { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string CredencialValor { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public int Status { get; set; }
    }
}
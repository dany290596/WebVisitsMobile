using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Models.HID.TipoCredencial
{
    public class TipoCredencialReqDTO
    {
        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string Nombre { get; set; }
    }
}
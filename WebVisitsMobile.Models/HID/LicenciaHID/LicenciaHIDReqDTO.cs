using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Models.HID.LicenciaHID
{
    public class LicenciaHIDReqDTO
    {
        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string NumeroParte { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string Nombre { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public Guid? EmpresaClienteId { get; set; }

        public int? CantidadTotal { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public int CantidadDisponible { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public int CantidadConsumida { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public DateTime FechaFin { get; set; }

        public string? EstadoLicencia { get; set; }

        public string? EstadoPeriodo { get; set; }

        public string? MensajeEstado { get; set; }
    }
}
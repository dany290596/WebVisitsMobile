using WebVisitsMobile.Models.Common;

namespace WebVisitsMobile.Models.HID.LicenciaHID
{
    public class LicenciaHIDRespDTO : BaseEntityDTO
    {
        public string? NumeroParte { get; set; }
        public string? Nombre { get; set; }
        public Guid? EmpresaClienteId { get; set; }
        public int? CantidadTotal { get; set; }
        public int? CantidadDisponible { get; set; }
        public int? CantidadConsumida { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? EstadoLicencia { get; set; }
        public string? EstadoPeriodo { get; set; }
        public string? MensajeEstado { get; set; }
    }
}
using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.HID
{
    public class LicenseHIDUpdateByTask : BaseEntity
    {
        public string NumeroParte { get; set; }
        public string Nombre { get; set; }
        public Guid? EmpresaClienteId { get; set; }
        public int? CantidadTotal { get; set; }
        public int CantidadDisponible { get; set; }
        public int CantidadConsumida { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? EstadoLicencia { get; set; }
        public string? EstadoPeriodo { get; set; }
        public string? MensajeEstado { get; set; }
        public int UserId { get; set; }
    }
}
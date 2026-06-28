using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.HID
{
    public class DipositivosHIDQueryFilter : BaseQueryFilter
    {
        public Guid? UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public string? SistemaOperativo { get; set; }
        public string? NombreDispositivo { get; set; }
        public string? CodigoInvitacion { get; set; }
        public string? EndpointId { get; set; }
        public string? SdkVersion { get; set; }
        public DateTime? DeviceInfoLastUpdated { get; set; }
        public int? DeviceDefault { get; set; }
        public int? Status { get; set; }
        public Guid? EmpresaClienteId { get; set; }
    }
}
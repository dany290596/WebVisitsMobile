using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.HID
{
    public class LicenciaUserHIDQueryFilter : BaseQueryFilter
    {
        public Guid? LicenciaId { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public int? UserId { get; set; }
        public string? Site { get; set; }
        public string? Alert { get; set; }
        public int? LicenseCount { get; set; }
        public string? Telefono { get; set; }
        public DateTime? InvitacionFecha { get; set; }
        public DateTime? InvitacionExpirationDate { get; set; }
        public string? InvitacionActividad { get; set; }
        public string? InvitacionDetalle { get; set; }
        public int? Status { get; set; }
        public Guid? EmpresaClienteId { get; set; }
    }
}
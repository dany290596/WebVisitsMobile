using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.HID
{
    public class CredencialHIDQueryFilter : BaseQueryFilter
    {
        public string? TipoCredencial { get; set; }
        public Guid? DispositivoId { get; set; }
        public Guid? Usuarioid { get; set; }
        public string? UsuarioNombre { get; set; }
        public string? CredencialValor { get; set; }
        public string? Validity { get; set; }
        public int? Status { get; set; }
        public Guid? EmpresaClienteId { get; set; }
        public Guid? ExternalId { get; set; }
    }
}
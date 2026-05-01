using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Organizacion.Tarea
{
    public class TareaQueryFilter : BaseQueryFilter
    {
        public Guid? TipoTareaId { get; set; }
        public DateTime? Fecha { get; set; }
        public int? Pendiente { get; set; }
        public byte? Status { get; set; }
        public string? ValorEnvio { get; set; }
        public string? ValorRetorno { get; set; }
        public Guid? ReferenciaId { get; set; }
        public Guid? EmpresaClienteId { get; set; }
    }
}
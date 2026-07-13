using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Empresa
{
    public class SucursalQueryFilter : BaseQueryFilter
    {
        public string? Nombre { get; set; }
        public string? RFC { get; set; }
        public Guid? EmpresaClienteId { get; set; }
    }
}

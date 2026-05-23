using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Ubicacion
{
    public class PaisEstadoQueryFilter : BaseQueryFilter
    {
        public string? Nombre { get; set; }
        public Guid? PaisId { get; set; }
    }
}
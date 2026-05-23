using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Ubicacion
{
    public class CiudadQueryFilter : BaseQueryFilter
    {
        public string? Nombre { get; set; }
        public Guid? EstadoId { get; set; }
    }
}
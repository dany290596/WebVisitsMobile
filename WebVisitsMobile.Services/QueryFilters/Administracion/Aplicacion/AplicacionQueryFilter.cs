using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Administracion.Aplicacion
{
    public class AplicacionQueryFilter : BaseQueryFilter
    {
        public string? Nombre { get; set; } = null!;
        public string? Imagen { get; set; }
        public byte? Orden { get; set; }
    }
}
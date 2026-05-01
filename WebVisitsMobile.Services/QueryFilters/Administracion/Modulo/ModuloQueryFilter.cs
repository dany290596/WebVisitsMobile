using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Administracion.Modulo
{
    public class ModuloQueryFilter : BaseQueryFilter
    {
        public string? Nombre { get; set; } = null!;
        public Guid? AplicacionId { get; set; }
        public byte? Orden { get; set; }
        public string? Imagen { get; set; }
    }
}
using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Administracion.Seccion
{
    public class SeccionQueryFilter : BaseQueryFilter
    {
        public string? Nombre { get; set; } = null!;
        public Guid? ModuloId { get; set; }
        public byte? Orden { get; set; }
        public string? Path { get; set; }
    }
}
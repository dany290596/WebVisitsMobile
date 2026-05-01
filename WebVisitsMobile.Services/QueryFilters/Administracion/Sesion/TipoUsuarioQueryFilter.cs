using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Administracion.Sesion
{
    public class TipoUsuarioQueryFilter : BaseQueryFilter
    {
        public string? Nombre { get; set; } = null!;
        public byte? TieneSesion { get; set; }
    }
}
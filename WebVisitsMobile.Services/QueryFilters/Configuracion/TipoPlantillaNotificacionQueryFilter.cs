using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Configuracion
{
    public class TipoPlantillaNotificacionQueryFilter : BaseQueryFilter
    {
        public string? Nombre { get; set; } = null!;
        public Guid? EmpresaClienteId { get; set; }
    }
}
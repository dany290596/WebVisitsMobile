using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Configuracion
{
    public class PlantillaNotificacionQueryFilter : BaseQueryFilter
    {
        public string? Nombre { get; set; } = null!;
        public string? CuerpoPlantilla { get; set; }
        public byte? NotificarEmail { get; set; }
        public byte? NotificarTeams { get; set; }
        public Guid? Identificador { get; set; }
        public Guid? TipoPlantillaNotificacionId { get; set; }
        public Guid? EmpresaClienteId { get; set; }
    }
}
using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Models.Configuracion.TipoPlantillaNotificacion;

namespace WebVisitsMobile.Models.Configuracion.PlantillaNotificacion
{
    public class PlantillaNotificacionRespDTO : BaseEntityDTO
    {
        public string Nombre { get; set; } = null!;
        public string? CuerpoPlantilla { get; set; }
        public byte? NotificarEmail { get; set; }
        public byte? NotificarTeams { get; set; }
        public Guid? Identificador { get; set; }
        public Guid TipoPlantillaNotificacionId { get; set; }
        public Guid EmpresaClienteId { get; set; }

        public virtual TipoPlantillaNotificacionRespDTO TipoPlantillaNotificacion { get; set; } = null!;
    }
}
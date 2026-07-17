using WebVisitsMobile.Models.Common;

namespace WebVisitsMobile.Models.Configuracion.TipoPlantillaNotificacion
{
    public class TipoPlantillaNotificacionRespDTO : BaseEntityDTO
    {
        public string Nombre { get; set; } = null!;
        public Guid EmpresaClienteId { get; set; }
    }
}
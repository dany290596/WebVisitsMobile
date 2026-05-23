using WebVisitsMobile.Models.Common;

namespace WebVisitsMobile.Models.Ubicacion.Ciudad
{
    public class CiudadRespDTO : BaseEntityDTO
    {
        public string Nombre { get; set; }
        public Guid? EstadoId { get; set; }
    }
}
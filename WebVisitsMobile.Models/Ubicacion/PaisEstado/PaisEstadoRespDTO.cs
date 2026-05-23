using WebVisitsMobile.Models.Common;

namespace WebVisitsMobile.Models.Ubicacion.PaisEstado
{
    public class PaisEstadoRespDTO : BaseEntityDTO
    {
        public string Nombre { get; set; }
        public Guid PaisId { get; set; }
    }
}
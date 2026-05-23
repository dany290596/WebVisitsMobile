using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Ubicacion
{
    public class Ciudad : BaseEntity
    {
        public string Nombre { get; set; }
        public Guid? EstadoId { get; set; }

  
    }
}
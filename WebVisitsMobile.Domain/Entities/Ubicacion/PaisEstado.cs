using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Ubicacion
{
    public class PaisEstado : BaseEntity
    {
        public string Nombre { get; set; }
        public Guid PaisId { get; set; }

        public virtual Pais? Pais { get; set; }
    }
}
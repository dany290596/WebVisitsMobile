using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Administracion.Modulo
{
    public class Modulo : BaseEntity
    {
        public Modulo()
        {
            Seccions = new HashSet<Seccion.Seccion>();
        }

        public string Nombre { get; set; } = null!;
        public Guid AplicacionId { get; set; }
        public byte Orden { get; set; }
        public string? Imagen { get; set; }


        public virtual Aplicacion.Aplicacion Aplicacion { get; set; } = null!;
        public virtual ICollection<Seccion.Seccion> Seccions { get; set; }
    }
}
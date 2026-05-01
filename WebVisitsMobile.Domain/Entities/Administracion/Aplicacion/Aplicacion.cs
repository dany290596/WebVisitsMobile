using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Administracion.Aplicacion
{
    public class Aplicacion : BaseEntity
    {
        public Aplicacion()
        {
            Modulos = new HashSet<Modulo.Modulo>();
        }

        public string Nombre { get; set; } = null!;
        public string? Imagen { get; set; }
        public byte Orden { get; set; }


        public virtual ICollection<Modulo.Modulo> Modulos { get; set; }
    }
}
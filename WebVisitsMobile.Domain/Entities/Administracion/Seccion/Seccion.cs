using WebVisitsMobile.Domain.Entities.Administracion.Perfil;
using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Administracion.Seccion
{
    public class Seccion : BaseEntity
    {
        public Seccion()
        {
            PerfilPermisoSecciones = new HashSet<PerfilPermisoSeccion>();
        }

        public string Nombre { get; set; } = null!;
        public Guid ModuloId { get; set; }
        public byte Orden { get; set; }
        public string Path { get; set; }


        public virtual Modulo.Modulo Modulo { get; set; } = null!;
        public virtual ICollection<PerfilPermisoSeccion> PerfilPermisoSecciones { get; set; }
    }
}
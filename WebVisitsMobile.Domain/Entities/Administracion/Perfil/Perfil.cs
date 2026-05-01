using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Administracion.Perfil
{
    public class Perfil : BaseEntity
    {
        public Perfil()
        {
            PerfilPermisoSecciones = new HashSet<PerfilPermisoSeccion>();
        }

        public string Nombre { get; set; } = null!;

        public virtual ICollection<PerfilPermisoSeccion> PerfilPermisoSecciones { get; set; }
    }
}
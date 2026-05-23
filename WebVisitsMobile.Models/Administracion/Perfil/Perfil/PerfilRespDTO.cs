using WebVisitsMobile.Models.Administracion.Perfil.PerfilPermisoSeccion;
using WebVisitsMobile.Models.Common;

namespace WebVisitsMobile.Models.Administracion.Perfil.Perfil
{
    public class PerfilRespDTO : BaseEntityDTO
    {
        public string Nombre { get; set; } = null!;

        public virtual ICollection<PerfilPermisoSeccionRespDTO> PerfilPermisoSecciones { get; set; }
    }
}
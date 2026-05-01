using System.ComponentModel.DataAnnotations;
using WebVisitsMobile.Models.Administracion.Perfil.PerfilPermisoSeccion;

namespace WebVisitsMobile.Models.Administracion.Perfil.Perfil
{
    public class PerfilReqDTO
    {
        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        [MaxLength(50, ErrorMessage = "Maximo 50 digitos")]
        public string Nombre { get; set; }

        public virtual ICollection<PerfilPermisoSeccionReqDTO>? PerfilPermisoSecciones { get; set; } = null!;
    }
}
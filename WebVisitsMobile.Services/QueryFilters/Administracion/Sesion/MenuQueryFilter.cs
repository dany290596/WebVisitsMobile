using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Services.QueryFilters.Administracion.Sesion
{
    public class MenuQueryFilter
    {
        [Required(ErrorMessage = "El parámetro perfilId es obligatorio.")]
        public Guid PerfilId { get; set; }
        public Guid TipoUsuarioId { get; set; }
    }
}
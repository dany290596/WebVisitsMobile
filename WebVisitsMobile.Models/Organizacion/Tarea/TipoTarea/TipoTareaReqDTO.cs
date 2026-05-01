using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Models.Organizacion.Tarea.TipoTarea
{
    public class TipoTareaReqDTO
    {
        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        [MaxLength(250, ErrorMessage = "Maximo 250 digitos")]
        public string Nombre { get; set; }
        public Guid UsuarioCreadorId { get; set; }
    }
}
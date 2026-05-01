using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Models.Organizacion.Tarea.Tarea
{
    public class TareaReqDTO
    {
        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public Guid TipoTareaId { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public byte Pendiente { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public byte Status { get; set; }

        public string? ValorEnvio { get; set; }

        public string? ValorRetorno { get; set; }

        public Guid? ReferenciaId { get; set; }

        public int? Marca { get; set; }
        public Guid? EmpresaClienteId { get; set; }
        public Guid UsuarioCreadorId { get; set; }
    }
}
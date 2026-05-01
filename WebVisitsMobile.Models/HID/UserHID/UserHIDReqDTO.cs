using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class UserHIDReqDTO
    {
        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public Guid LicenciaId { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string Nombre { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        [EmailAddress(ErrorMessage = "El Correo electrónico no es válido.")]
        public string Email { get; set; }

        public int? UserId { get; set; }
        public string? Site { get; set; }
        public string? Alert { get; set; }
        public int? LicenseCount { get; set; }

        public string? Telefono { get; set; }

        public DateTime? InvitacionFecha { get; set; }
        public DateTime? InvitacionExpirationDate { get; set; }
        public string? InvitacionActividad { get; set; }
        public int? InvitacionId { get; set; }
        public string? InvitacionDetalle { get; set; }
        public int? Status { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        public string? Apellidos { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public Guid UsuarioCreadorId { get; set; }
    }
}
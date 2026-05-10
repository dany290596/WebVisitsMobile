using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class UserHIDReqDTO
    {
        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        [Display(Name = "LicenciaId")]
        public Guid LicenciaId { get; set; }

        [Required(ErrorMessageResourceName = "MESSAGE_REQUIRED")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "El Correo electrónico no es válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Site")]
        public string? Site { get; set; }

        [Display(Name = "Alert")]
        public string? Alert { get; set; }

        [Display(Name = "LicenseCount")]
        public int? LicenseCount { get; set; }

        [Display(Name = "Telefono")]
        public string? Telefono { get; set; }

        [Required]
        [Display(Name = "Apellidos")]
        public string? Apellidos { get; set; }

        [Required]
        [Display(Name = "FechaInicio")]
        public DateTime? FechaInicio { get; set; }

        [Required]
        [Display(Name = "FechaFin")]
        public DateTime? FechaFin { get; set; }

        [Display(Name = "ExternalId")]
        public Guid? ExternalId { get; set; }

        public int? TipoCredencial { get; set; }

        public Guid UsuarioCreadorId { get; set; }
    }
}
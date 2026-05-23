namespace WebVisitsMobile.Models.Organizacion.Tarea.Tarea
{
    public class TareaUsuarioHIDDTO
    {
        public Guid Id { get; set; }
        public Guid LicenciaId { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public int? UserId { get; set; }
        public string? Site { get; set; }
        public string? Alert { get; set; }
        public int? LicenseCount { get; set; }
        public string? Telefono { get; set; }
        public DateTime? InvitacionFecha { get; set; }
        public DateTime? InvitacionExpirationDate { get; set; }
        public string? InvitacionActividad { get; set; }
        public string? InvitacionDetalle { get; set; }
        public int? InvitacionId { get; set; }
        public string? Apellidos { get; set; }
    }
}
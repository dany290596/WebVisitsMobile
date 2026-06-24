namespace WebVisitsMobile.Models.Organizacion.Tarea.Tarea
{
    public class TareaUsuarioHIDWalletDTO
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string ExternalId { get; set; }
        public Guid? UsuarioWalletId { get; set; }
        public string Emails { get; set; }
        public string Telefono { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Plataforma { get; set; }
        public TareaPlantillaCredencialDTO? Plantilla { get; set; }
    }
}
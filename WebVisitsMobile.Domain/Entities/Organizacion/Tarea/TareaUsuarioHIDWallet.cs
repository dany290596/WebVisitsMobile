namespace WebVisitsMobile.Domain.Entities.Organizacion.Tarea
{
    public class TareaUsuarioHIDWallet
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string ExternalId { get; set; }
        public string Emails { get; set; }
        public string Telefono { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Plataforma { get; set; }

        public TareaPlantillaCredencial? Plantilla { get; set; }
    }
}
namespace WebVisitsMobile.Domain.Entities.Organizacion.Tarea
{
    public class TareaPlantillaCredencial
    {
        public Guid Id { get; set; }
        public Guid? BackgroundExternalId { get; set; }
        public Guid? LogoExternalId { get; set; }
        public Guid? ExternalId { get; set; }
        public Guid? AppleId { get; set; }
    }
}
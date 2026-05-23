namespace WebVisitsMobile.Models.HID.PlantillaCredencial
{
    public class PlantillaCredencialReqDTO
    {
        public string Nombre { get; set; }
        public string? ImagenFondo { get; set; }
        public string? ExtensionImagenFondo { get; set; }
        public string? ImagenLogo { get; set; }
        public string? ExtensionImagenLogo { get; set; }

        public Guid? BackgroundExternalId { get; set; }
        public Guid? LogoExternalId { get; set; }
        public Guid? ExternalId { get; set; }
        public Guid? AppleId { get; set; }

        public Guid UsuarioCreadorId { get; set; }
    }
}
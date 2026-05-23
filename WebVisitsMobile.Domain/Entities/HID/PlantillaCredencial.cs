using System.ComponentModel.DataAnnotations.Schema;
using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.HID
{
    public class PlantillaCredencial : BaseEntity
    {
        public string Nombre { get; set; }
        public string? ImagenFondo { get; set; }
        public string? ExtensionImagenFondo { get; set; }
        public string? ImagenLogo { get; set; }
        public string? ExtensionImagenLogo { get; set; }
        public Guid? EmpresaClienteId { get; set; }

        public Guid? BackgroundExternalId { get; set; }
        public Guid? LogoExternalId { get; set; }
        public Guid? ExternalId { get; set; }
        public Guid? AppleId { get; set; }

        [NotMapped]
        public string? ImagenFondoBase64 { get; set; }

        [NotMapped]
        public string? ImagenLogoBase64 { get; set; }
    }
}
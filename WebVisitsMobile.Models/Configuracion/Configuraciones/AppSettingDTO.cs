namespace WebVisitsMobile.Models.Configuracion.Configuraciones
{
    public class AppSettingDTO
    {
        // CN01
        public string CustomerId { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string ClientSecretOrCertificate { get; set; } = default!;

        // CN02
        public string IdpAuthenticationUrl { get; set; } = default!;
        public string ApiUrl { get; set; } = default!;
        public string CallbackAndEventUrl { get; set; } = default!;
        public string? PremiumReportUrl { get; set; } // Opcional

        // CN03
        public string ContentType { get; set; } = default!;
        public string? AcceptType { get; set; } // Opcional
        public string ApplicationId { get; set; } = default!;
        public string ApplicationVersion { get; set; } = default!;

        // CN04
        public string PartNumberField { get; set; } = default!;

        // CN05
        public string? AutoDetectPartNumber { get; set; } // Opcional
        public string? SelectPartNumber { get; set; }     // Opcional
        public string? ManualEntryPartNumber { get; set; } // Opcional
    }
}
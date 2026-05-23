namespace WebVisitsMobile.Domain.Entities.Configuracion
{
    public class AppSetting
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
        public string? CredentialManagementURL { get; set; } = default!;
        public string? UsersURL { get; set; } = default!;
        public string? EventsURL { get; set; } = default!;
        public string? TransactionURL { get; set; } = default!;

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
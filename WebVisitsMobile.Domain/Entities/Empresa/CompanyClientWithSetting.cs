using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.Entities.Ubicacion;

namespace WebVisitsMobile.Domain.Entities.Empresa
{
    public class CompanyClientWithSetting
    {
        // Datos de la empresa
        public Guid Id { get; set; }
        public string RazonSocial { get; set; }
        public string RFC { get; set; }
        public string TelefonoEmpresa { get; set; }
        public string? TelefonoMovil { get; set; }
        public string CorreoElectronico { get; set; }
        public byte UsaCredencialesHID { get; set; }
        public Pais? Pais { get; set; }
        public PaisEstado? PaisEstado { get; set; }
        public Ciudad? Ciudad { get; set; }

        // Configuraciones HID (null si UsaCredencialesHID != 1)
        // @CN01
        public Setting? CustomerId { get; set; }                // 742CE98B-684B-4A76-BA0D-CF62621FC3E7
        public Setting? ClientId { get; set; }                  // BB617929-5F49-4FDC-8C28-62435505B600
        public Setting? ClientSecretOrCertificate { get; set; } // 29625587-4A45-495A-B728-203608694C44

        // @CN02
        public Setting? IdpAuthenticationUrl { get; set; }      // 60ADEBFE-01B5-497A-828B-CF3801F37495
        public Setting? ApiUrl { get; set; }                    // 9B02E35B-A069-4BF5-B9CA-337A59455347
        public Setting? CallbackAndEventUrl { get; set; }       // 82481E61-4BF5-44CE-B222-3283F7BC02F9
        public Setting? PremiumReportUrl { get; set; }          // 84BA81E1-56C0-4BEE-A57F-D05C13BB544A
        public Setting? CredentialManagementURL { get; set; }   // 5006A3E3-1E78-4341-9253-C2189A7C8974
        public Setting? UsersURL { get; set; }                  // 5F9327BE-42D6-46B9-BF0E-DB7176371A20
        public Setting? EventsURL { get; set; }                 // 9914DCB1-B370-4FC5-8CA3-D5ADD1605AF9
        public Setting? TransactionURL { get; set; }            // A90006CA-A3E8-4576-A8B0-25B1C5438D55

        // @CN03
        public Setting? ContentType { get; set; }               // 40E1A0B9-9144-490E-BF75-7663F3447118
        public Setting? AcceptType { get; set; }                // 4B6BCEFA-20CA-48B9-92FA-5396C7C94202
        public Setting? ApplicationId { get; set; }             // 788F90F3-0CE3-4E96-B4BA-38DA1CFE105B
        public Setting? ApplicationVersion { get; set; }        // FF5E7D45-FCED-4169-B4EB-BA70B43F7BB6

        // @CN04
        public Setting? PartNumberField { get; set; }           // C98EE139-92FB-4E71-94B7-AE258DD1929A

        // @CN05
        public Setting? AutoDetectPartNumber { get; set; }      // D539FF01-17F0-4C29-9E17-668A5591ACE5
        public Setting? SelectPartNumber { get; set; }          // 18A0E41D-960E-4F52-9604-D0C773A87F9C
        public Setting? ManualEntryPartNumber { get; set; }     // 32DC2E87-E6A4-48D7-AF0E-B967ED2BBF49
    }
}
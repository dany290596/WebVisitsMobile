using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.Entities.Ubicacion;

namespace WebVisitsMobile.Domain.Entities.Empresa
{
    public class CompanyWithSettingDecrypt
    {
        // Datos de la empresa
        public Guid Id { get; set; }
        public string RazonSocial { get; set; }
        public string RFC { get; set; }
        public string TelefonoEmpresa { get; set; }
        public string? TelefonoMovil { get; set; }
        public string CorreoElectronico { get; set; }

        public byte UsaCredencialesHID { get; set; }
        public byte UsaCredencialesWallet { get; set; }

        public Pais? Pais { get; set; }
        public PaisEstado? PaisEstado { get; set; }
        public Ciudad? Ciudad { get; set; }


        public List<SettingsGroupTap>? CredencialesHID { get; set; }
        public List<SettingsGroupTap>? CredencialesWallet { get; set; }
    }
}
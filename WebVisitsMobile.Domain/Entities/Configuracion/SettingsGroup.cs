namespace WebVisitsMobile.Domain.Entities.Configuracion
{
    public class SettingsGroup
    {
        public Guid EmpresaClienteId { get; set; }
        public string EmpresaClienteNombre { get; set; }
        public byte UsaCredencialesHID { get; set; }
        public byte UsaCredencialesWallet { get; set; }

        public AppSetting? CredencialesHID { get; set; }
        public AppSetting? CredencialesWallet { get; set; }
    }
}
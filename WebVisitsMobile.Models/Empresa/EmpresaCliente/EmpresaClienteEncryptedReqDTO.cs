namespace WebVisitsMobile.Models.Empresa.EmpresaCliente
{
    public class EmpresaClienteEncryptedReqDTO
    {
        public Guid? id { get; set; }
        public EmpresaClienteReqDTO Empresa { get; set; }
        public List<SettingsGroupTapDTO>? SettingEncryptedHID { get; set; }
        public List<SettingsGroupTapDTO>? SettingEncryptedWallet { get; set; }
    }
}
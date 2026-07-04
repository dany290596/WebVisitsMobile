namespace WebVisitsMobile.Models.Empresa.EmpresaCliente
{
    public class EmpresaClienteEncryptedReqDTO
    {
        public Guid? id { get; set; }
        public EmpresaClienteReqDTO Empresa { get; set; }
        public string? SettingEncryptedHID { get; set; }
        public string? SettingEncryptedWallet { get; set; }
    }
}
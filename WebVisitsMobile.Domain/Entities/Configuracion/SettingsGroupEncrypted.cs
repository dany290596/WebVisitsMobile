using WebVisitsMobile.Domain.Entities.Encriptacion;

namespace WebVisitsMobile.Domain.Entities.Configuracion
{
    public class SettingsGroupEncrypted
    {
        public Guid EmpresaClienteId { get; set; }
        public string EmpresaClienteNombre { get; set; }
        public byte UsaCredencialesHID { get; set; }
        public byte UsaCredencialesWallet { get; set; }

        public key? CredencialesHID { get; set; }
        public key? CredencialesWallet { get; set; }
    }
}
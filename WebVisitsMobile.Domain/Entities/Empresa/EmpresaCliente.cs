using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Empresa
{
    public class EmpresaCliente : BaseEntity
    {
        public string RazonSocial { get; set; }
        public string RFC { get; set; }
        public string TelefonoEmpresa { get; set; }
        public string? TelefonoMovil { get; set; }
        public string CorreoElectronico { get; set; }
        public byte UsaCredencialesHID { get; set; }
    }
}
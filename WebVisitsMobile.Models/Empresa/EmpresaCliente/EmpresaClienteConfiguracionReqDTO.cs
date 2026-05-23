using WebVisitsMobile.Models.Configuracion.Configuraciones;

namespace WebVisitsMobile.Models.Empresa.EmpresaCliente
{
    public class EmpresaClienteConfiguracionReqDTO
    {
        public Guid? id { get; set; }
        public EmpresaClienteReqDTO Empresa { get; set; }
        public List<ConfiguracionesReqDTO>? Configuraciones { get; set; }
    }
}
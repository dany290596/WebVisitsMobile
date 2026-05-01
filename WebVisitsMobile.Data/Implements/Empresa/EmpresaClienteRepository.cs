using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Empresa;
using WebVisitsMobile.Domain.Entities.Empresa;

namespace WebVisitsMobile.Data.Implements.Empresa
{
    public class EmpresaClienteRepository : Repository<EmpresaCliente>, IEmpresaClienteRepository
    {
        public EmpresaClienteRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
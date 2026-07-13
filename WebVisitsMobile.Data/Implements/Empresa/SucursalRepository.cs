using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Empresa;
using WebVisitsMobile.Domain.Entities.Empresa;

namespace WebVisitsMobile.Data.Implements.Empresa
{
    public class SucursalRepository : Repository<Sucursal>, ISucursalRepository
    {
        public SucursalRepository(WebVisitsMobileContext context) : base(context) { }
    }
}

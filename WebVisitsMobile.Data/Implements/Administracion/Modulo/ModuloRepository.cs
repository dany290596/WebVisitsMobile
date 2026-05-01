using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Administracion.Modulo;

namespace WebVisitsMobile.Data.Implements.Administracion.Modulo
{
    public class ModuloRepository : Repository<Domain.Entities.Administracion.Modulo.Modulo>, IModuloRepository
    {
        public ModuloRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
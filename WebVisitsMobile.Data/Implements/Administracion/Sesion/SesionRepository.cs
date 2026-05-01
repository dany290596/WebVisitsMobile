using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Administracion.Sesion;

namespace WebVisitsMobile.Data.Implements.Administracion.Sesion
{
    public class SesionRepository : Repository<Domain.Entities.Administracion.Sesion.Sesion>, ISesionRepository
    {
        public SesionRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
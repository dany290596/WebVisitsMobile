using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Administracion.Aplicacion;

namespace WebVisitsMobile.Data.Implements.Administracion.Aplicacion
{
    public class AplicacionRepository : Repository<Domain.Entities.Administracion.Aplicacion.Aplicacion>, IAplicacionRepository
    {
        public AplicacionRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Ubicacion;
using WebVisitsMobile.Domain.Entities.Ubicacion;

namespace WebVisitsMobile.Data.Implements.Ubicacion
{
    public class CiudadRepository : Repository<Ciudad>, ICiudadRepository
    {
        public CiudadRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Ubicacion;
using WebVisitsMobile.Domain.Entities.Ubicacion;

namespace WebVisitsMobile.Data.Implements.Ubicacion
{
    public class PaisEstadoRepository : Repository<PaisEstado>, IPaisEstadoRepository
    {
        public PaisEstadoRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
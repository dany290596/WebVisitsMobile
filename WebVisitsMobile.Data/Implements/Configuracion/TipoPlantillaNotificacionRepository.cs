using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Configuracion;
using WebVisitsMobile.Domain.Entities.Configuracion;

namespace WebVisitsMobile.Data.Implements.Configuracion
{
    public class TipoPlantillaNotificacionRepository : Repository<TipoPlantillaNotificacion>, ITipoPlantillaNotificacionRepository
    {
        public TipoPlantillaNotificacionRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
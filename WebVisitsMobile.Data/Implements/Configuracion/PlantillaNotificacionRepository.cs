using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Configuracion;
using WebVisitsMobile.Domain.Entities.Configuracion;

namespace WebVisitsMobile.Data.Implements.Configuracion
{
    public class PlantillaNotificacionRepository : Repository<PlantillaNotificacion>, IPlantillaNotificacionRepository
    {
        public PlantillaNotificacionRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<IEnumerable<PlantillaNotificacion>> GetAllNotificationTemplate()
        {
            return await _context.PlantillaNotificacion
                .Include(x => x.TipoPlantillaNotificacion)
                .OrderByDescending(u => u.FechaCreacion)
                .ThenBy(u => u.Estado)
                .ToListAsync();
        }

        public async Task<PlantillaNotificacion> GetNotificationTemplate(Expression<Func<PlantillaNotificacion, bool>> predicate)
        {
            return await _context.PlantillaNotificacion
                .Include(x => x.TipoPlantillaNotificacion)
                .FirstOrDefaultAsync(predicate);
        }
    }
}
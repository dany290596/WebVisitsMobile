using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Configuracion;

namespace WebVisitsMobile.Data.Interfaces.Configuracion
{
    public interface IPlantillaNotificacionRepository : IRepository<PlantillaNotificacion>
    {
        Task<IEnumerable<PlantillaNotificacion>> GetAllNotificationTemplate();
        Task<PlantillaNotificacion> GetNotificationTemplate(Expression<Func<PlantillaNotificacion, bool>> predicate);
    }
}
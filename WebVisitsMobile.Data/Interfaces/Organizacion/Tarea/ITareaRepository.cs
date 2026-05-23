using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;

namespace WebVisitsMobile.Data.Interfaces.Organizacion.Tarea
{
    public interface ITareaRepository : IRepository<Domain.Entities.Organizacion.Tarea.Tarea>
    {
        Task<IEnumerable<Domain.Entities.Organizacion.Tarea.Tarea>> GetAllTask();
        Task<Domain.Entities.Organizacion.Tarea.Tarea> GetTask(Expression<Func<Domain.Entities.Organizacion.Tarea.Tarea, bool>> predicate);
        Task<IEnumerable<TareaHID<T>>> GetAllByUserWallet<T>(Guid typeTaskId);
        Task<IEnumerable<TareaHID<TareaPlantilla>>> GetAllByTemplate();
    }
}
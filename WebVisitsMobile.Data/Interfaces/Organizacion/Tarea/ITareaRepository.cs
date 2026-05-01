using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;

namespace WebVisitsMobile.Data.Interfaces.Organizacion.Tarea
{
    public interface ITareaRepository : IRepository<Domain.Entities.Organizacion.Tarea.Tarea>
    {
        Task<IEnumerable<Domain.Entities.Organizacion.Tarea.Tarea>> GetAllTask();
        Task<Domain.Entities.Organizacion.Tarea.Tarea> GetTask(Expression<Func<Domain.Entities.Organizacion.Tarea.Tarea, bool>> predicate);
    }
}
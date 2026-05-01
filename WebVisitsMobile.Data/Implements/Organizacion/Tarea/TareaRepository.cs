using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Organizacion.Tarea;

namespace WebVisitsMobile.Data.Implements.Organizacion.Tarea
{
    public class TareaRepository : Repository<Domain.Entities.Organizacion.Tarea.Tarea>, ITareaRepository
    {
        public TareaRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<IEnumerable<Domain.Entities.Organizacion.Tarea.Tarea>> GetAllTask()
        {
            return _context.Tarea
                .Include(i => i.TipoTarea)
                .AsEnumerable();
        }

        public async Task<Domain.Entities.Organizacion.Tarea.Tarea> GetTask(Expression<Func<Domain.Entities.Organizacion.Tarea.Tarea, bool>> predicate)
        {
            return _context.Tarea
                .Include(l => l.TipoTarea)
                .AsNoTracking()
                .FirstOrDefault(predicate);
        }
    }
}
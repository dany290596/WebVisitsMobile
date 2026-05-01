using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Organizacion.Tarea;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;

namespace WebVisitsMobile.Data.Implements.Organizacion.Tarea
{
    public class TipoTareaRepository : Repository<TipoTarea>, ITipoTareaRepository
    {
        public TipoTareaRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Organizacion.Tarea;

namespace WebVisitsMobile.Services.Interfaces.Organizacion.Tarea
{
    public interface ITipoTareaService
    {
        Task<bool> Create(TipoTarea taskType, Guid currentUserId);
        Task<bool> Update(TipoTarea taskType, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<TipoTarea>> GetAll(TipoTareaQueryFilter filters);
        Task<TipoTarea> GetById(Guid taskType);
    }
}
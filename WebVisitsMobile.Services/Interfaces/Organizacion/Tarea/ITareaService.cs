using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Models.Organizacion.Tarea.Tarea;
using WebVisitsMobile.Services.QueryFilters.Organizacion.Tarea;

namespace WebVisitsMobile.Services.Interfaces.Organizacion.Tarea
{
    public interface ITareaService
    {
        Task<Domain.Entities.Organizacion.Tarea.Tarea?> Create(Domain.Entities.Organizacion.Tarea.Tarea task, Guid currentUserId);
        Task<bool> Update(Domain.Entities.Organizacion.Tarea.Tarea task, Guid currentUserId);
        Task<bool> UpdatePending(Guid taskId, TareaPendingDTO task);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<Domain.Entities.Organizacion.Tarea.Tarea>> GetAll(TareaQueryFilter filters);
        Task<Domain.Entities.Organizacion.Tarea.Tarea> GetById(Guid task);
    }
}
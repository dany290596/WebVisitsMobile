using WebVisitsMobile.Domain.Entities.Ubicacion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Ubicacion;

namespace WebVisitsMobile.Services.Interfaces.Ubicacion
{
    public interface IPaisEstadoService
    {
        Task<PaisEstado?> Create(PaisEstado data, Guid currentUserId);
        Task<bool> Update(PaisEstado data, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<PaisEstado>> GetAll(PaisEstadoQueryFilter filters, Guid clientCompanyId);
        Task<PaisEstado> GetById(Guid data, Guid clientCompanyId);
    }
}
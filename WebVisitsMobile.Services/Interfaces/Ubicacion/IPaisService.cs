using WebVisitsMobile.Domain.Entities.Ubicacion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Ubicacion;

namespace WebVisitsMobile.Services.Interfaces.Ubicacion
{
    public interface IPaisService
    {
        Task<Pais?> Create(Pais data, Guid currentUserId);
        Task<bool> Update(Pais data, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<Pais>> GetAll(PaisQueryFilter filters, Guid clientCompanyId);
        Task<Pais> GetById(Guid data, Guid clientCompanyId);
    }
}
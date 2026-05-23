using WebVisitsMobile.Domain.Entities.Ubicacion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Ubicacion;

namespace WebVisitsMobile.Services.Interfaces.Ubicacion
{
    public interface ICiudadService
    {
        Task<Ciudad?> Create(Ciudad data, Guid currentUserId);
        Task<bool> Update(Ciudad data, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<Ciudad>> GetAll(CiudadQueryFilter filters, Guid clientCompanyId);
        Task<Ciudad> GetById(Guid data, Guid clientCompanyId);
    }
}
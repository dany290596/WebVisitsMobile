using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Configuracion;

namespace WebVisitsMobile.Services.Interfaces.Configuracion
{
    public interface ITipoPlantillaNotificacionService
    {
        Task<TipoPlantillaNotificacion> GetById(Guid id, Guid clientCompanyId);
        PagedList<TipoPlantillaNotificacion> GetAll(TipoPlantillaNotificacionQueryFilter filters, Guid clientCompanyId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<bool> Create(TipoPlantillaNotificacion data, Guid currentUserId, Guid clientCompanyId);
        Task<bool> Update(TipoPlantillaNotificacion data, Guid currentUserId, Guid clientCompanyId);
    }
}
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Configuracion;

namespace WebVisitsMobile.Services.Interfaces.Configuracion
{
    public interface IPlantillaNotificacionService
    {
        Task<PlantillaNotificacion> GetById(Guid id, Guid clientCompanyId);
        Task<PlantillaNotificacion> GetByNotificationTemplate(Guid notificationTemplateTypeId, Guid clientCompanyId);
        Task<PagedList<PlantillaNotificacion>> GetAll(PlantillaNotificacionQueryFilter filters, Guid clientCompanyId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<bool> Create(PlantillaNotificacion data, Guid currentUserId, Guid clientCompanyId);
        Task<bool> Update(PlantillaNotificacion data, Guid currentUserId, Guid clientCompanyId);
        Task<bool> ExistsName(string name);
        Task<bool> ExistsNameForUpdate(Guid id, string name);
        Task<bool> ExistsTipoPlantilla(Guid notificationTemplateTypeId, Guid clientCompanyId);
        Task<bool> ExistsTipoPlantillaForUpdate(Guid id, Guid notificationTemplateTypeId, Guid clientCompanyId);
    }
}
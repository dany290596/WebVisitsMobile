using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Models.Configuracion.Configuraciones;
using WebVisitsMobile.Services.QueryFilters.Configuracion;

namespace WebVisitsMobile.Services.Interfaces.Configuracion
{
    public interface IConfiguracionService
    {
        Task<bool> Create(Configuraciones setting, Guid currentUserId);
        Task<bool> Update(Configuraciones setting, Guid currentUserId, Guid clientCompanyId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<Configuraciones>> GetAll(ConfiguracionesQueryFilter filters);
        Task<Configuraciones> GetById(Guid settingId, Guid clientCompanyId);
        Task<Configuraciones> GetById(Guid settingId);
        Task<Configuraciones> GetByTypeSettingAndCompanyId(Guid typeSetting, Guid clientCompanyId);
        Task<bool> Update(List<ConfiguracionesListReqDTO> data);
        Task<Dictionary<Guid, ConfigSettingDTO>> GetFullSetting(Guid clientCompanyId);
        Task<Models.Common.ResultDTO<AppSettingDTO>> GetAppSettings(Guid clientCompanyId);
        Task<EmailSettingDTO> GetEmailSetting(Guid clientCompanyId);
        Task<bool> CreateInitialSettingsForCompany(Guid clientCompanyId, Guid currentUserId);
        Task<bool> CreateSettingsForCompany(List<ConfiguracionesReqDTO> settings, Guid empresaClienteId, Guid currentUserId);
        Task<bool> ReactivateAllSettingsByCompany(Guid clientCompanyId, Guid currentUserId);
        Task<List<Configuraciones>> GetConfigurationTemplates();
        Task<List<SettingsGroup>> GetSettingsGroupByCompany();
    }
}
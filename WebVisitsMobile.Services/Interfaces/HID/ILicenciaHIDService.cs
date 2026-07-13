using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface ILicenciaHIDService
    {
        Task<bool> Create(LicenciaHID licenciaHID, Guid currentUserId);
        Task<bool> CreateByTask(LicenciaHID licenciaHID, Guid currentUserId);
        Task<bool> CreateByTask(Guid clientCompanyId, Guid currentUserId);
        Task<bool> SynchronizeByTask(LicenciaHID licenseHID, Guid currentUserId);
        Task<bool> Update(LicenciaHID licenseHID, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<LicenciaHID>> GetAll(LicenciaHIDQueryFilter filters, Guid clientCompanyId);
        Task<LicenciaHID> GetById(Guid licenseHIDId, Guid clientCompanyId);
        Task<LicenciaHID?> GetByEmpresaClienteId(Guid empresaClienteId);
        IEnumerable<LicenciaHID> GetAllList(LicenciaHIDQueryFilter filters);
    }
}
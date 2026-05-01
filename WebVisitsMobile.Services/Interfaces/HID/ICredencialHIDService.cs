using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface ICredencialHIDService
    {
        Task<bool> Create(CredencialHid credentialHID, Guid currentUserId);
        Task<bool> Update(CredencialHid credentialHID, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<CredencialHid>> GetAll(CredencialHIDQueryFilter filters, Guid clientCompanyId);
        Task<CredencialHid> GetById(Guid credentialHIDId, Guid clientCompanyId);
    }
}
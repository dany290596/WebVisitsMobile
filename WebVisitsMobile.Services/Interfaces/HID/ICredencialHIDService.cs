using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface ICredencialHIDService
    {
        Task<bool> Create(CredencialHid credentialHID, Guid currentUserId);
        Task<bool> CreateForWallet(CredencialHid credentialHID, Guid clientCompanyId);
        Task<bool> Update(CredencialHid credentialHID, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid clientCompanyId);
        Task<bool> Inactivate(Guid id, Guid clientCompanyId);
        Task<bool> Suspend(Guid id, Guid clientCompanyId);
        Task<PagedList<CredencialHid>> GetAll(CredencialHIDQueryFilter filters, Guid clientCompanyId);
        Task<CredencialHid> GetById(Guid credentialHIDId, Guid clientCompanyId);
        Task<bool> UpdateStatus(Guid userId, int status);
    }
}
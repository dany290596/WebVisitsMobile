using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface IDipositivosHIDService
    {
        Task<DipositivosHid?> Create(DipositivosHid deviceHID, Guid currentUserId);
        Task<bool> Update(DipositivosHid deviceHID, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<DipositivosHid>> GetAll(DipositivosHIDQueryFilter filters, Guid clientCompanyId);
        Task<PagedList<CommonDTO>> GetAllQuery(DipositivosHIDQueryFilter filters);
        Task<DipositivosHid> GetById(Guid deviceHID, Guid clientCompanyId);
    }
}
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Models.HID.PlantillaCredencial;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface IPlantillaCredencialService
    {
        Task<PlantillaCredencial?> GetById(Guid id, Guid clientCompanyId);
        Task<PagedList<PlantillaCredencial>> GetAll(PlantillaCredencialQueryFilter filters, Guid clientCompanyId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id);
        Task<bool> Update(Guid id, PlantillaCredencialExternalReqDTO credencial);
        Task<bool> Create(PlantillaCredencial data, Guid currentUserId, Guid clientCompanyId);
        Task<bool> Update(PlantillaCredencial data, Guid currentUserId, Guid clientCompanyId);
    }
}
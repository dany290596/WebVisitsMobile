using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface ITipoCredencialService
    {
        Task<TipoCredencial?> Create(TipoCredencial data, Guid currentUserId);
        Task<bool> Update(TipoCredencial data, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<TipoCredencial>> GetAll(TipoCredencialQueryFilter filters);
        Task<TipoCredencial> GetById(Guid dataId);
    }
}
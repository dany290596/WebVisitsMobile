using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface IUsuarioHidTipoCredencialService
    {
        Task<UsuarioHidTipoCredencial?> Create(UsuarioHidTipoCredencial data, Guid currentUserId);
        Task<bool> Update(UsuarioHidTipoCredencial data, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<UsuarioHidTipoCredencial>> GetAll(UsuarioHidTipoCredencialQueryFilter filters);
        Task<UsuarioHidTipoCredencial> GetById(Guid dataId);
        Task<UsuarioHidTipoCredencial> GetUserHidTypeCredential(Guid dataId);
    }
}
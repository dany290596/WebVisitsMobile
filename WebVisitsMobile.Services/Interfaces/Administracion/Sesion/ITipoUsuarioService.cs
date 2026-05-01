using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Administracion.Sesion;

namespace WebVisitsMobile.Services.Interfaces.Administracion.Sesion
{
    public interface ITipoUsuarioService
    {
        Task<TipoUsuario> GetById(Guid id, Guid clientCompanyId);
        Task<PagedList<TipoUsuario>> GetAll(TipoUsuarioQueryFilter filters, Guid clientCompanyId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Create(TipoUsuario data, Guid currentUserId);
        Task<bool> Update(TipoUsuario data, Guid currentUserId);
    }
}
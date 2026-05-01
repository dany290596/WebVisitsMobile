using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Administracion.Sesion;

namespace WebVisitsMobile.Services.Interfaces.Administracion.Sesion
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUserForCredentials(Login login);
        Task<PagedList<Usuario>> GetAll(UsuarioQueryFilter filters, Guid clientCompanyId);
        Task<bool> Create(Usuario user, string password, Guid currentUserId, Guid clientCompanyId);
        Task<bool> Update(Usuario user, Guid currentUserId, Guid clientCompanyId);
        Task<Usuario> GetById(Guid id);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<Usuario> GetUserValid(Guid id);
        Task<Usuario> GetUserById(Guid id);
        Task<Usuario> GetUserByEmail(string email);
    }
}
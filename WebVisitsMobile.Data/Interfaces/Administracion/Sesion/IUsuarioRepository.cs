using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;

namespace WebVisitsMobile.Data.Interfaces.Administracion.Sesion
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario> GetUserForCredentials(Login usuarioLogin);
        IEnumerable<Usuario> GetAllUser();
        Task<Usuario> GetFirstOrDefaultUser(Expression<Func<Usuario, bool>> predicate);
        Guid GetSelectUserByUserTypeNameAndUserMail(string typeUserName, string userMail);
        Task<bool> EmailExistsAsync(string email, Guid excludedUserId);
        Task<Usuario> GetUser(Expression<Func<Usuario, bool>> predicate);
    }
}
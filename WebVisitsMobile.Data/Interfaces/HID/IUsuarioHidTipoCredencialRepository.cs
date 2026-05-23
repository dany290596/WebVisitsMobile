using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Interfaces.HID
{
    public interface IUsuarioHidTipoCredencialRepository : IRepository<UsuarioHidTipoCredencial>
    {
        Task<UsuarioHidTipoCredencial> GetUserHidTypeCredential(Expression<Func<UsuarioHidTipoCredencial, bool>> predicate);
        Task<IEnumerable<UsuarioHidTipoCredencial>> GetAllUserHidTypeCredential();
    }
}
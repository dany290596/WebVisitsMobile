using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;

namespace WebVisitsMobile.Data.Interfaces.Administracion.Perfil
{
    public interface IPerfilRepository : IRepository<Domain.Entities.Administracion.Perfil.Perfil>
    {
        Task<Domain.Entities.Administracion.Perfil.Perfil> GetByIdConPermisos(Guid id);
        Task<Domain.Entities.Administracion.Perfil.Perfil> GetProfile(Expression<Func<Domain.Entities.Administracion.Perfil.Perfil, bool>> predicate);
    }
}
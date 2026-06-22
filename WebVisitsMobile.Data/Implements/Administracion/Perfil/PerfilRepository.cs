using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Administracion.Perfil;

namespace WebVisitsMobile.Data.Implements.Administracion.Perfil
{
    public class PerfilRepository : Repository<Domain.Entities.Administracion.Perfil.Perfil>, IPerfilRepository
    {
        public PerfilRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<Domain.Entities.Administracion.Perfil.Perfil> GetByIdConPermisos(Guid id)
        {
            return await _context.Perfil.Include(x => x.PerfilPermisoSecciones).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Domain.Entities.Administracion.Perfil.Perfil> GetProfile(Expression<Func<Domain.Entities.Administracion.Perfil.Perfil, bool>> predicate)
        {
            return await _context.Perfil
                .FirstOrDefaultAsync(predicate);
        }
    }
}
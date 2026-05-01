using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Administracion.Perfil;
using WebVisitsMobile.Domain.Entities.Administracion.Perfil;

namespace WebVisitsMobile.Data.Implements.Administracion.Perfil
{
    public class PerfilPermisoSeccionRepository : Repository<PerfilPermisoSeccion>, IPerfilPermisoSeccionRepository
    {
        public PerfilPermisoSeccionRepository(WebVisitsMobileContext context) : base(context) { }

        public void DeleteByProfile(Guid profileId)
        {
            _context.PerfilPermisoSeccion.RemoveRange(_context.PerfilPermisoSeccion.Where(x => x.PerfilId == profileId));
        }
    }
}
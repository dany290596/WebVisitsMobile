using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;

namespace WebVisitsMobile.Data.Implements.Administracion.Sesion
{
    public class TipoUsuarioRepository : Repository<TipoUsuario>, ITipoUsuarioRepository
    {
        public TipoUsuarioRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
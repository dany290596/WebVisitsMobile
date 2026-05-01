using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Administracion.Perfil;

namespace WebVisitsMobile.Data.Implements.Administracion.Perfil
{
    public class PerfilRepository : Repository<Domain.Entities.Administracion.Perfil.Perfil>, IPerfilRepository
    {
        public PerfilRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
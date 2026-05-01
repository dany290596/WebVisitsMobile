using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Administracion.Perfil;

namespace WebVisitsMobile.Data.Interfaces.Administracion.Perfil
{
    public interface IPerfilPermisoSeccionRepository : IRepository<PerfilPermisoSeccion>
    {
        void DeleteByProfile(Guid profileId);
    }
}
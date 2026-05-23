using WebVisitsMobile.Data.Interfaces.Common;

namespace WebVisitsMobile.Data.Interfaces.Administracion.Perfil
{
    public interface IPerfilRepository : IRepository<Domain.Entities.Administracion.Perfil.Perfil>
    {
        Task<Domain.Entities.Administracion.Perfil.Perfil> GetByIdConPermisos(Guid id);
    }
}
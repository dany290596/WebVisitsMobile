using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Administracion.Perfil;

namespace WebVisitsMobile.Services.Interfaces.Administracion.Perfil
{
    public interface IPerfilService
    {
        Task<Domain.Entities.Administracion.Perfil.Perfil?> GetById(Guid id, Guid clientCompanyId);
        PagedList<Domain.Entities.Administracion.Perfil.Perfil> GetAll(PerfilQueryFilter filters, Guid clientCompanyId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<bool> Create(Domain.Entities.Administracion.Perfil.Perfil data, Guid currentUserId, Guid clientCompanyId);
        Task<bool> Update(Domain.Entities.Administracion.Perfil.Perfil data, Guid currentUserId, Guid clientCompanyId);
        Task<Domain.Entities.Administracion.Perfil.Perfil> GetPerfilConPermisos(Guid id);
    }
}
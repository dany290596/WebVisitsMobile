using WebVisitsMobile.Domain.Entities.Administracion.Perfil;

namespace WebVisitsMobile.Services.Interfaces.Administracion.Perfil
{
    public interface IPerfilPermisoSeccionService
    {
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<bool> DeleteByProfile(Guid profileIdId);
        Task<bool> PostProfilePermissionSectionMultiple(ICollection<PerfilPermisoSeccion> permissions, Guid currentUserId);
    }
}
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Administracion.Aplicacion;

namespace WebVisitsMobile.Services.Interfaces.Administracion.Aplicacion
{
    public interface IAplicacionService
    {
        Task<Domain.Entities.Administracion.Aplicacion.Aplicacion?> GetById(Guid id, Guid empresaId);
        PagedList<Domain.Entities.Administracion.Aplicacion.Aplicacion> GetAll(AplicacionQueryFilter filters, Guid empresaId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
    }
}
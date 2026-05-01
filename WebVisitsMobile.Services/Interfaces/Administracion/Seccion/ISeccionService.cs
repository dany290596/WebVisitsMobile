using WebVisitsMobile.Domain.Entities.Administracion.Modulo;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Administracion.Seccion;
using WebVisitsMobile.Services.QueryFilters.Administracion.Sesion;

namespace WebVisitsMobile.Services.Interfaces.Administracion.Seccion
{
    public interface ISeccionService
    {
        Task<IEnumerable<SeccionesPorModulo>> GetSectionsGroupedByModule(MenuQueryFilter filters);
        Task<Domain.Entities.Administracion.Seccion.Seccion?> GetById(Guid id, Guid empresaId);
        Task<PagedList<Domain.Entities.Administracion.Seccion.Seccion>> GetAll(SeccionQueryFilter filters, Guid empresaId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
    }
}
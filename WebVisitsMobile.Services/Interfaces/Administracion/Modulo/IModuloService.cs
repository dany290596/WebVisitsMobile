using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Administracion.Modulo;

namespace WebVisitsMobile.Services.Interfaces.Administracion.Modulo
{
    public interface IModuloService
    {
        Task<Domain.Entities.Administracion.Modulo.Modulo?> GetById(Guid id, Guid empresaId);
        PagedList<Domain.Entities.Administracion.Modulo.Modulo> GetAll(ModuloQueryFilter filters, Guid empresaId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
    }
}
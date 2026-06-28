using WebVisitsMobile.Data.Interfaces.Common;

namespace WebVisitsMobile.Data.Interfaces.Administracion.Sesion
{
    public interface ISesionRepository : IRepository<Domain.Entities.Administracion.Sesion.Sesion>
    {
        Task<int> NumberOfSessions(Guid userId);
    }
}
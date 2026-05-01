using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Administracion.Modulo;

namespace WebVisitsMobile.Data.Interfaces.Administracion.Seccion
{
    public interface ISeccionRepository : IRepository<Domain.Entities.Administracion.Seccion.Seccion>
    {
        Task<IEnumerable<SeccionesPorModulo>> GetSectionsGroupedByModule(Guid? perfilId);
        Task<IEnumerable<Domain.Entities.Administracion.Seccion.Seccion>> GetAllSection();
    }
}
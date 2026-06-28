using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Parametrizacion;

namespace WebVisitsMobile.Data.Interfaces.Parametrizacion
{
    public interface ICorreoEnviarRepository : IRepository<CorreoEnviar>
    {
        public Task<List<CorreoEnviarConfiguracion>> GetPendingEmailsWithConfig();
    }
}
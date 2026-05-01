using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Empresa;

namespace WebVisitsMobile.Infrastructure.Interfaces
{
    public interface IPlataformaService
    {
        Task<EmpresaCliente?> ExistsCompany(Guid id);
        Task<Sesion?> SessionValidate(Guid id);
    }
}
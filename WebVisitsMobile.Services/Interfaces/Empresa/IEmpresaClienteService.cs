using WebVisitsMobile.Domain.Entities.Empresa;

namespace WebVisitsMobile.Services.Interfaces.Empresa
{
    public interface IEmpresaClienteService
    {
        Task<EmpresaCliente> GetById(Guid id);
    }
}
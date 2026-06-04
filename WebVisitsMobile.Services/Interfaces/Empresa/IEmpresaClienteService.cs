using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Models.Configuracion.Configuraciones;
using WebVisitsMobile.Services.QueryFilters.Empresa;

namespace WebVisitsMobile.Services.Interfaces.Empresa
{
    public interface IEmpresaClienteService
    {
        Task<PagedList<EmpresaCliente>> GetAll(EmpresaClienteQueryFilter filters);
        Task<EmpresaCliente> GetById(Guid id);
        Task<EmpresaCliente> GetCompanyClient(Guid id);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> CreateWithHID(EmpresaCliente clientCompany, List<ConfiguracionesReqDTO>? settings, Guid usuarioActualId);
        Task<bool> Update(EmpresaCliente clientCompany, Guid usuarioActualId);
        Task<bool> UpdateWithHID(EmpresaCliente clientCompany, List<ConfiguracionesReqDTO>? settings, Guid usuarioActualId);
        Task<EmpresaCliente?> GetByRFC(string rfc);
        Task<EmpresaCliente?> GetByRazonSocial(string socialReason);
        Task<CompanyClientWithSetting> GetWithSetting(Guid companyClientId);
    }
}
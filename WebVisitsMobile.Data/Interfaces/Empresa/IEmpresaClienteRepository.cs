using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Empresa;

namespace WebVisitsMobile.Data.Interfaces.Empresa
{
    public interface IEmpresaClienteRepository : IRepository<EmpresaCliente>
    {
        Task<EmpresaCliente> GetCompanyClient(Expression<Func<EmpresaCliente, bool>> predicate);
        Task<CompanyClientWithSetting> GetCompanyClientWithSetting(Guid companyClientId);
        Task<CompanyWithSettingEncrypted> GetCompanyWithSettingEncrypted(Guid companyClientId);
    }
}
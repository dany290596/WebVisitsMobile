using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Empresa;

namespace WebVisitsMobile.Services.Services.Empresa
{
    public class EmpresaClienteService : IEmpresaClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        public EmpresaClienteService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<EmpresaCliente> GetById(Guid id)
        {
            try
            {
                EmpresaCliente data = await _unitOfWork.EmpresaClienteRepository.GetById(id);
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Services.Interfaces.Empresa;

namespace WebVisitsMobile.Infrastructure.Services
{
    public class PlataformService : IPlataformaService
    {
        private readonly IEmpresaClienteService empresaClienteService;
        private readonly IUnitOfWork unitOfWork;

        public PlataformService(
            IEmpresaClienteService empresaClienteService,
            IUnitOfWork unitOfWork
            )
        {
            this.empresaClienteService = empresaClienteService;
            this.unitOfWork = unitOfWork;
        }

        public async Task<EmpresaCliente?> ExistsCompany(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            try
            {
                var empresa = await empresaClienteService.GetById(id);
                return empresa is not null ? empresa : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Sesion?> SessionValidate(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            try
            {
                Sesion sesion = await unitOfWork.SesionRepository.GetById(id);
                if (sesion == null) { return null; }

                if (sesion.Estado == 1)
                {
                    return sesion;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
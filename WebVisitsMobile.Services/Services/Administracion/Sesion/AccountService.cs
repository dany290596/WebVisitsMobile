using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Administracion.Sesion;

namespace WebVisitsMobile.Services.Services.Administracion.Sesion
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOption;
        public AccountService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOption = options.Value;
        }

        public async Task<AccountInfo?> GetAccountInfo(Guid userId)
        {
            return await _unitOfWork.UsuarioRepository.GetAccountInfo(userId);
        }
    }
}
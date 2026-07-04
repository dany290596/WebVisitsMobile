using WebVisitsMobile.Domain.Entities.Administracion.Sesion;

namespace WebVisitsMobile.Services.Interfaces.Administracion.Sesion
{
    public interface IAccountService
    {
        Task<AccountInfo?> GetAccountInfo(Guid userId);
    }
}
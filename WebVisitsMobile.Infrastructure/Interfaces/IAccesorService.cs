using WebVisitsMobile.Domain.Entities.Administracion.Sesion;

namespace WebVisitsMobile.Infrastructure.Interfaces
{
    public interface IAccesorService
    {
        Guid GetClaimUsuarioId();
        Token GetTokenData();
    }
}
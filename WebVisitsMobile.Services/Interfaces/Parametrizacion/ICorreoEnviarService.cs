using WebVisitsMobile.Domain.Entities.Parametrizacion;

namespace WebVisitsMobile.Services.Interfaces.Parametrizacion
{
    public interface ICorreoEnviarService
    {
        Task<List<CorreoEnviarConfiguracion>> GetPendingEmailsWithConfig();
        Task<bool> InsertEmailSend(CorreoEnviar data, Guid currentUserId, Guid companyId);
        Task<bool> MarkAsSent(Guid id);
        Task<bool> IncreaseAttemptCount(Guid id);
        Task<bool> SendUserEmail(CorreoEnviarUsuario data, Guid currentUserId, Guid companyId);
    }
}
using WebVisitsMobile.Models.Configuracion.Email;

namespace WebVisitsMobile.Services.Interfaces.Configuracion
{
    public interface IEmailTemplateService
    {
        Task<EmailResponseDTO?> GetTemplate(Guid companyId);
        Task<string?> GetRenderedPreview(Guid companyId);
        Task<bool> SaveTemplate(Guid companyId, Guid userId, string templateHtml);
    }
}
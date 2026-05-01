using WebVisitsMobile.Models.HID.UserHID;

namespace WebVisitsMobile.Services.Interfaces.Organizacion.Email
{
    public interface IEmailService
    {
        Task<bool> SendEmailUserHID(UserHIDReqDTO userHID);
        Task<bool> SendEmailInvitationCodeHID(string destinatario, string codigo, DateTime expiracion, Guid clientCompanyId);
        Task<bool> SendContractReportByEmail(byte[] pdfBytes, string destinatario, string asunto);
    }
}
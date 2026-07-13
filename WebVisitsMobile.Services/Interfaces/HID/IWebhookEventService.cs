using WebVisitsMobile.Models.HID.CredencialHID;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface IWebhookEventService
    {
        Task<bool> PostWebhookEvents(int userHIDId, string invitationCode, Guid companyId, Guid userCreatorId);
        Task<bool> CreateCredentialHID(int userHIDId, CredencialHIDSeriDTO credential, Guid companyId, Guid userCreatorId);
        Task<bool> CreateOrUpdateDeviceFromEndpoint(int userHIDId, EndpointSeriDTO endpoint, Guid companyId, Guid userCreatorId);
    }
}
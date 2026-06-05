using WebVisitsMobile.Models.HID.HIDOrigoCallback;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface IHIDOrigoEventService
    {
        // ─── USUARIOS ────────────────────────────────────────────────────────
        Task<bool> OnUserCreated(UserEventDTO data);
        Task<bool> OnUserUpdated(UserEventDTO data);
        Task<bool> OnUserDeleteInitiated(UserEventDTO data);
        Task<bool> OnUserDeleted(UserEventDTO data);

        // ─── CREDENCIALES ─────────────────────────────────────────────────────
        Task<bool> OnCredentialReserved(CredentialEventDTO data);
        Task<bool> OnCredentialIssued(CredentialEventDTO data);
        Task<bool> OnCredentialRevoking(CredentialEventDTO data);
        Task<bool> OnCredentialRevoked(CredentialEventDTO data);
        Task<bool> OnCredentialUnbound(CredentialEventDTO data);
        Task<bool> OnCredentialCreationFailure(CredentialEventDTO data);

        // ─── DISPOSITIVOS ─────────────────────────────────────────────────────
        Task<bool> OnDevicePersonalized(EndpointEventDTO data);
        Task<bool> OnDeviceInactive(EndpointEventDTO data);
    }
}
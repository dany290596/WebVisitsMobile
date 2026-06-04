using WebVisitsMobile.Models.HID.HIDOrigoCallback;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface IHIDOrigoEventService
    {
        // ─── USUARIOS ────────────────────────────────────────────────────────
        Task<bool> OnUserCreatedAsync(UserEventDTO data);
        Task<bool> OnUserUpdatedAsync(UserEventDTO data);
        Task<bool> OnUserDeleteInitiatedAsync(UserEventDTO data);
        Task<bool> OnUserDeletedAsync(UserEventDTO data);

        // ─── CREDENCIALES ─────────────────────────────────────────────────────
        Task<bool> OnCredentialReservedAsync(CredentialEventDTO data);
        Task<bool> OnCredentialIssuedAsync(CredentialEventDTO data);
        Task<bool> OnCredentialRevokingAsync(CredentialEventDTO data);
        Task<bool> OnCredentialRevokedAsync(CredentialEventDTO data);
        Task<bool> OnCredentialUnboundAsync(CredentialEventDTO data);
        Task<bool> OnCredentialCreationFailureAsync(CredentialEventDTO data);

        // ─── DISPOSITIVOS ─────────────────────────────────────────────────────
        Task<bool> OnDevicePersonalizedAsync(EndpointEventDTO data);
        Task<bool> OnDeviceInactiveAsync(EndpointEventDTO data);
    }
}
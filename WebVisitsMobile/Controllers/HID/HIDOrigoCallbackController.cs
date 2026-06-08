using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebVisitsMobile.Models.HID.HIDOrigoCallback;
using WebVisitsMobile.Services.Interfaces.HID;
using Microsoft.AspNetCore.Http.Features;

namespace WebVisitsMobile.Controllers.HID
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "HID Origo")]
    public class HIDOrigoCallbackController : ControllerBase
    {
        private readonly IHIDOrigoEventService _hidOrigoEventService;

        public HIDOrigoCallbackController(IHIDOrigoEventService hidOrigoEventService)
        {
            _hidOrigoEventService = hidOrigoEventService;
        }

        /// <summary>
        /// Recibe todos los eventos de HID Origo via Callback.
        /// URL registrada en HID: POST /api/HIDOrigoCallback
        /// </summary>
        /// [DisableRequestSizeLimit]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        [HttpPost]
        public async Task<IActionResult> ReceiveCallback()
        {
            // ✅ Leer el body ANTES de responder
            Request.EnableBuffering();
            using var reader = new StreamReader(Request.Body, leaveOpen: true);
            string body = await reader.ReadToEndAsync();

            Console.WriteLine($"[HIDOrigo] ▶️  RECIBIDO | {DateTime.UtcNow:HH:mm:ss.fff}");
            Console.WriteLine($"[HIDOrigo] 📦 BODY: {body}");

            // ✅ Procesar en background — HID recibe 200 OK de inmediato
            _ = Task.Run(async () =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(body)) return;

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var eventos = JsonSerializer.Deserialize<List<HIDWebhookEventDTO>>(body, options);

                    if (eventos == null || eventos.Count == 0) return;

                    foreach (var evt in eventos)
                    {
                        if (evt == null) continue;
                        Console.WriteLine($"[HIDOrigo] 🔁 Procesando | Type: {evt.Type} | Id: {evt.Id}");
                        await ProcessEventAsync(evt, options);
                        Console.WriteLine($"[HIDOrigo] ✅ Procesado OK | Type: {evt.Type}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[HIDOrigo] ❌ Error en background: {ex.Message}");
                }
            });

            // ✅ HID recibe 200 OK inmediatamente sin esperar el procesamiento
            return Ok();
        }

        // ─────────────────────────────────────────────────────────────────────
        // PROCESADOR — Rutea por el campo "type" del envelope + "status" en data
        // ─────────────────────────────────────────────────────────────────────
        private async Task ProcessEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options)
        {
            if (evt.Type.Contains("user"))
                await RouteUserEventAsync(evt, options);

            else if (evt.Type.Contains("invitation"))
                await RouteInvitationEventAsync(evt, options);

            else if (evt.Type.Contains("credentialcontainer"))
                await RouteDeviceEventAsync(evt, options);     // dispositivos

            else if (evt.Type.Contains("credential"))
                await RouteCredentialEventAsync(evt, options); // credenciales

            else if (evt.Type.Contains("pass"))
                await RoutePassEventAsync(evt, options);
        }

        // ─── USUARIOS ─────────────────────────────────────────────────────────
        private async Task RouteUserEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options)
        {
            var data = evt.Data.Deserialize<UserEventDTO>(options);
            if (data == null) return;

            Console.WriteLine($"[HIDOrigo] 👤 USUARIO | Status: {data.Status} | UserId: {data.UserId} | Nombre: {data.Firstname} {data.Lastname} | Email: {data.Email}");

            switch (data.Status)
            {
                case "USER_CREATED":
                    await _hidOrigoEventService.OnUserCreated(data);
                    break;
                case "USER_UPDATED":
                    await _hidOrigoEventService.OnUserUpdated(data);
                    break;
                case "USER_DELETE_INITIATED":
                    await _hidOrigoEventService.OnUserDeleteInitiated(data);
                    break;
                case "USER_DELETED":
                    await _hidOrigoEventService.OnUserDeleted(data);
                    break;
            }
        }

        // ─── INVITACIONES ─────────────────────────────────────────────────────
        private async Task RouteInvitationEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options)
        {
            var data = evt.Data.Deserialize<InvitationEventDTO>(options);
            if (data == null) return;

            Console.WriteLine($"[HIDOrigo] ✉️  INVITACIÓN | Status: {data.Status} | UserId: {data.UserId} | Código: {data.InvitationCode}");

            // TODO: agregar métodos de invitación al servicio si se necesitan
            switch (data.Status)
            {
                case "INVITATION_PENDING":
                    break;
                case "INVITATION_ACKNOWLEDGED":
                    break;
                case "INVITATION_CANCEL_INITIATED":
                case "INVITATION_CANCELLED":
                    break;
                case "INVITATION_EXPIRED":
                    break;
            }
        }

        // ─── DISPOSITIVOS ─────────────────────────────────────────────────────
        private async Task RouteDeviceEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options)
        {
            var data = evt.Data.Deserialize<EndpointEventDTO>(options);
            if (data == null) return;

            Console.WriteLine($"[HIDOrigo] 📱 DISPOSITIVO | Status: {data.Status} | ContainerId: {data.Id} | UserId: {data.UserId} | Modelo: {data.Model} | Tipo: {data.DeviceType} | Platform: {data.Platform?.Type}");

            switch (data.Status)
            {
                case "CREDENTIALCONTAINER_PERSONALIZED":
                    await _hidOrigoEventService.OnDevicePersonalized(data);
                    break;
                case "CREDENTIALCONTAINER_INACTIVE":
                    await _hidOrigoEventService.OnDeviceInactive(data);
                    break;
            }
        }

        // ─── CREDENCIALES ─────────────────────────────────────────────────────
        private async Task RouteCredentialEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options)
        {
            var data = evt.Data.Deserialize<CredentialEventDTO>(options);
            if (data == null) return;

            Console.WriteLine($"[HIDOrigo] 🔑 CREDENCIAL | Status: {data.Status} | CredentialId: {data.Id} | UserId: {data.UserId} | CardNumber: {data.CardNumber}");

            switch (data.Status)
            {
                case "CREDENTIAL_RESERVED":
                    await _hidOrigoEventService.OnCredentialReserved(data);
                    break;
                case "CREDENTIAL_ISSUED":
                    await _hidOrigoEventService.OnCredentialIssued(data);
                    break;
                case "CREDENTIAL_REVOKING":
                    await _hidOrigoEventService.OnCredentialRevoking(data);
                    break;
                case "CREDENTIAL_REVOKED":
                    await _hidOrigoEventService.OnCredentialRevoked(data);
                    break;
                case "CREDENTIAL_UNBOUND":
                    await _hidOrigoEventService.OnCredentialUnbound(data);
                    break;
                case "CREDENTIAL_CREATION_FAILURE":
                    await _hidOrigoEventService.OnCredentialCreationFailure(data);
                    break;
            }
        }

        // ─── PASSES ───────────────────────────────────────────────────────────
        private async Task RoutePassEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options)
        {
            var data = evt.Data.Deserialize<PassEventDTO>(options);
            if (data == null) return;

            Console.WriteLine($"[HIDOrigo] 🎫 PASS | Status: {data.Status} | PassId: {data.Id} | UserId: {data.UserId} | Platform: {data.PlatformType} | Device: {data.DeviceType}");

            // TODO: agregar métodos de pass al servicio cuando se necesiten
            switch (data.Status)
            {
                case "PASS_CREATED": break;
                case "PASS_ISSUE_INITIATED": break;
                case "PASS_ISSUING": break;
                case "PASS_ACTIVE": break;
                case "PASS_ISSUE_FAILED": break;
                case "PASS_SUSPENDING": break;
                case "PASS_SUSPENDED": break;
                case "PASS_USER_SUSPENDED": break;
                case "PASS_RESUMING": break;
                case "PASS_REVOKING": break;
                case "PASS_REVOKED": break;
                case "PASS_USER_REVOKED": break;
                case "PASS_REVOKE_FAILED": break;
                case "PASS_UPDATED": break;
                case "PASS_CANCELLED": break;
            }
        }
    }
}
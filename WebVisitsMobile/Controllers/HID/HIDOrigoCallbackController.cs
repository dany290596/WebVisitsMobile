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
        private readonly IServiceScopeFactory _scopeFactory;

        // Máximo 2 callbacks procesándose en paralelo — evita N DbContexts simultáneos
        private static readonly SemaphoreSlim _processingSlim = new(2, 2);

        // Reutilizado en todos los requests — JsonSerializerOptions es costoso de crear (~100-500 KB)
        private static readonly JsonSerializerOptions _jsonOptions =
            new() { PropertyNameCaseInsensitive = true };

        private static readonly object _logLock = new();
        private static bool _logDirReady;

        public HIDOrigoCallbackController(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private static void Log(string message)
        {
            var timestamp = DateTime.Now;
            var line = $"[{timestamp:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            Console.WriteLine(line);
            try
            {
                if (!_logDirReady)
                {
                    Directory.CreateDirectory(@"C:\logs");
                    _logDirReady = true;
                }
                var file = Path.Combine(@"C:\logs", $"webvisitsMobilecallbacks_{timestamp:yyyy-MM-dd}.txt");
                lock (_logLock)
                    System.IO.File.AppendAllText(file, line + Environment.NewLine);
            }
            catch { }
        }

        /// <summary>
        /// Recibe todos los eventos de HID Origo via Callback.
        /// URL registrada en HID: POST /api/HIDOrigoCallback
        /// </summary>
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        [HttpPost]
        public async Task<IActionResult> ReceiveCallback()
        {
            // ✅ Leer el body ANTES de responder
            Request.EnableBuffering();
            using var reader = new StreamReader(Request.Body, leaveOpen: true);
            string body = await reader.ReadToEndAsync();

            Log($"[HIDOrigo] ▶️  RECIBIDO | {DateTime.UtcNow:HH:mm:ss.fff}");
            Log($"[HIDOrigo] 📦 BODY: {body}");

            // ✅ Scope propio para el background task — evita "disposed context" al terminar el request
            // SemaphoreSlim limita a 2 tareas simultáneas para no saturar RAM con DbContexts concurrentes
            _ = Task.Run(async () =>
            {
                await _processingSlim.WaitAsync();
                using var scope = _scopeFactory.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<IHIDOrigoEventService>();
                try
                {
                    if (string.IsNullOrWhiteSpace(body)) return;

                    var eventos = JsonSerializer.Deserialize<List<HIDWebhookEventDTO>>(body, _jsonOptions);

                    if (eventos == null || eventos.Count == 0) return;

                    foreach (var evt in eventos)
                    {
                        if (evt == null) continue;
                        Log($"[HIDOrigo] 🔁 Procesando | Type: {evt.Type} | Id: {evt.Id}");
                        await ProcessEventAsync(evt, _jsonOptions, svc);
                        Log($"[HIDOrigo] ✅ Procesado OK | Type: {evt.Type}");
                    }
                }
                catch (Exception ex)
                {
                    Log($"[HIDOrigo] ❌ Error en background: {ex.Message}");
                }
                finally
                {
                    _processingSlim.Release();
                }
            });

            // ✅ HID recibe 200 OK inmediatamente sin esperar el procesamiento
            return Ok();
        }

        // ─────────────────────────────────────────────────────────────────────
        // PROCESADOR — Rutea por el campo "type" del envelope + "status" en data
        // ─────────────────────────────────────────────────────────────────────
        private static async Task ProcessEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options, IHIDOrigoEventService svc)
        {
            // EndsWith evita que "credential-management.events.pass" caiga en la rama ".credential"
            if (evt.Type.Contains("user"))
                await RouteUserEventAsync(evt, options, svc);

            else if (evt.Type.Contains("invitation"))
                await RouteInvitationEventAsync(evt, options);

            else if (evt.Type.EndsWith(".credentialcontainer"))
                await RouteDeviceEventAsync(evt, options, svc);

            else if (evt.Type.EndsWith(".pass"))
                await RoutePassEventAsync(evt, options, svc);

            else if (evt.Type.EndsWith(".credential"))
                await RouteCredentialEventAsync(evt, options, svc);
        }

        // ─── USUARIOS ─────────────────────────────────────────────────────────
        private static async Task RouteUserEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options, IHIDOrigoEventService svc)
        {
            var data = evt.Data.Deserialize<UserEventDTO>(options);
            if (data == null) return;

            Log($"[HIDOrigo] 👤 USUARIO | Status: {data.Status} | UserId: {data.UserId} | Nombre: {data.Firstname} {data.Lastname} | Email: {data.Email}");

            switch (data.Status)
            {
                case "USER_CREATED":
                    await svc.OnUserCreated(data);
                    break;
                case "USER_UPDATED":
                    await svc.OnUserUpdated(data);
                    break;
                case "USER_DELETE_INITIATED":
                    await svc.OnUserDeleteInitiated(data);
                    break;
                case "USER_DELETED":
                    await svc.OnUserDeleted(data);
                    break;
            }
        }

        // ─── INVITACIONES ─────────────────────────────────────────────────────
        private static async Task RouteInvitationEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options)
        {
            var data = evt.Data.Deserialize<InvitationEventDTO>(options);
            if (data == null) return;

            Log($"[HIDOrigo] ✉️  INVITACIÓN | Status: {data.Status} | UserId: {data.UserId} | Código: {data.InvitationCode}");

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
        private static async Task RouteDeviceEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options, IHIDOrigoEventService svc)
        {
            var data = evt.Data.Deserialize<EndpointEventDTO>(options);
            if (data == null) return;

            Log($"[HIDOrigo] 📱 DISPOSITIVO | Status: {data.Status} | ContainerId: {data.Id} | UserId: {data.UserId} | Modelo: {data.Model} | Tipo: {data.DeviceType} | Platform: {data.Platform?.Type}");

            switch (data.Status)
            {
                case "CREDENTIALCONTAINER_PERSONALIZED":
                    await svc.OnDeviceCreate(data);
                    break;
                case "CREDENTIALCONTAINER_INACTIVE":
                    await svc.OnDeviceInactive(data);
                    break;
            }
        }

        // ─── CREDENCIALES ─────────────────────────────────────────────────────
        private static async Task RouteCredentialEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options, IHIDOrigoEventService svc)
        {
            var data = evt.Data.Deserialize<CredentialEventDTO>(options);
            if (data == null) return;

            Log($"[HIDOrigo] 🔑 CREDENCIAL | Status: {data.Status} | CredentialId: {data.Id} | UserId: {data.UserId} | CardNumber: {data.CardNumber}");

            switch (data.Status)
            {
                case "CREDENTIAL_RESERVED":
                    await svc.OnCredentialReserved(data);
                    break;
                case "CREDENTIAL_ISSUED":
                    await svc.OnCredentialIssued(data);
                    break;
                case "CREDENTIAL_REVOKING":
                    await svc.OnCredentialRevoking(data);
                    break;
                case "CREDENTIAL_REVOKED":
                    await svc.OnCredentialRevoked(data);
                    break;
                case "CREDENTIAL_UNBOUND":
                    await svc.OnCredentialUnbound(data);
                    break;
                case "CREDENTIAL_CREATION_FAILURE":
                    await svc.OnCredentialCreationFailure(data);
                    break;
            }
        }

        // ─── PASSES ───────────────────────────────────────────────────────────
        private static async Task RoutePassEventAsync(HIDWebhookEventDTO evt, JsonSerializerOptions options, IHIDOrigoEventService svc)
        {
            var data = evt.Data.Deserialize<PassEventDTO>(options);
            if (data == null) return;

            Log($"[HIDOrigo] 🎫 PASS | Status: {data.Status} | PassId: {data.Id} | UserId: {data.UserId} | Platform: {data.PlatformType} | Device: {data.DeviceType}");

            // TODO: agregar métodos de pass al servicio cuando se necesiten
            switch (data.Status)
            {
                case "PASS_CREATED": break;
                case "PASS_ISSUE_INITIATED": break;
                case "PASS_ISSUING":
                    if (data.DeviceType == "WATCH")
                    {
                        await svc.OnUserUpdatedPass(data);
                    }

                    break;
                case "PASS_ACTIVE":

                    if (data.DeviceType == "WATCH" && data.PlatformType == "GOOGLE_WALLET")
                    {
                        await svc.OnUserUpdatedPass(data);  // primero crea la credencial
                        await svc.ActualizarStatusPass(data, 3); // luego actualiza el status

                    }
                    else {
                        await svc.ActualizarStatusPass(data, 3); // luego actualiza el status

                    }
                    break;
                case "PASS_ISSUE_FAILED": break;
                case "PASS_SUSPENDING": break;
                case "PASS_SUSPENDED":
                    await svc.ActualizarStatusPass(data,4);                    
                    break;
                case "PASS_USER_SUSPENDED":
                    await svc.ActualizarStatusPass(data, 4);
                    break;
                case "PASS_RESUMING":
                    await svc.ActualizarStatusPass(data, 3);
                    break;
                case "PASS_REVOKING": break;
                case "PASS_REVOKED":
                    await svc.ActualizarStatusPass(data, 7);
                    break;
                case "PASS_USER_REVOKED":
                    await svc.ActualizarStatusPass(data, 4);
                    break;
                case "PASS_REVOKE_FAILED": break;
                case "PASS_UPDATED": break;
                case "PASS_CANCELLED": break;
            }
        }
    }
}

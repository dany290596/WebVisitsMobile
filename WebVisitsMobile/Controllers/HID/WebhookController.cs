using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebVisitsMobile.Models.HID.CredencialHID;
using WebVisitsMobile.Services.Interfaces.HID;

namespace WebVisitsMobile.Controllers.HID
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "HID Origo")]
    public class WebhookController : ControllerBase
    {
        private readonly IWebhookEventService _webhookEventService;

        private static readonly object _logLock = new();
        private static bool _logDirReady;

        public WebhookController(
            IWebhookEventService webhookEventService
            )
        {
            _webhookEventService = webhookEventService;
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
                var file = Path.Combine(@"C:\logs", $"webvisitsMobilewebhooks_{timestamp:yyyy-MM-dd}.txt");
                lock (_logLock)
                    System.IO.File.AppendAllText(file, line + Environment.NewLine);
            }
            catch { }
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveWebhookEvent()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            Log($"[Webhook] ▶️  RECIBIDO | {DateTime.UtcNow:HH:mm:ss.fff}");
            Log($"[Webhook] 📦 BODY: {body}");

            try
            {
                if (string.IsNullOrWhiteSpace(body))
                {
                    Log("[Webhook] ⚠️ Cuerpo del webhook está vacío.");
                    return BadRequest("El cuerpo de la solicitud no puede estar vacío.");
                }

                var eventos = JsonSerializer.Deserialize<List<WebhookSeriDTO>>(body);
                if (eventos == null || eventos.Count == 0)
                {
                    Log("[Webhook] ⚠️ No se encontraron eventos en el cuerpo.");
                    return BadRequest("No se encontraron eventos válidos en el cuerpo.");
                }
                foreach (var evt in eventos)
                {
                    if (evt == null)
                    {
                        Log("[Webhook] ⚠️ Evento o datos nulos. Se omite.");
                        continue;
                    }

                    Log($"[Webhook] 🔁 Procesando | Type: {evt.Type}");

                    switch (evt.Type)
                    {
                        case "com.origo.mi.invitation":
                            var invitation = evt.Data.Deserialize<InvitationSeriDTO>();
                            if (invitation == null)
                            {
                                Log("[Webhook] ❌ No se pudo deserializar invitation.");
                                break;
                            }
                            Log($"[Webhook] ✉️  Invitación recibida: {invitation.InvitationCode} | Status: {invitation.Status}");
                            if (invitation.Status == "INVITATION_ACKNOWLEDGED")
                            {
                                bool isUserIdValid = !string.IsNullOrWhiteSpace(invitation.UserId) && invitation.UserId != "undefined" && int.TryParse(invitation.UserId, out int userId);
                                bool isInvitationCodeValid = !string.IsNullOrWhiteSpace(invitation.InvitationCode) && invitation.InvitationCode != "undefined";

                                if (isUserIdValid && isInvitationCodeValid)
                                {
                                    Log($"[Webhook] ✅ Invitación ACKNOWLEDGED recibida: {invitation.InvitationCode}");
                                    await _webhookEventService.PostWebhookEvents(Convert.ToInt32(invitation.UserId), invitation.InvitationCode, new Guid("E096DCEF-B118-4596-9FA0-676855A3FB53"), new Guid("739B4C8F-4DB1-4475-84D4-7644DCE00620"));
                                }
                                else
                                {
                                    Log("[Webhook] ❌ Datos incompletos o inválidos en la invitación:");
                                    Log($"[Webhook] UserId: '{invitation.UserId}'");
                                    Log($"[Webhook] InvitationCode: '{invitation.InvitationCode}'");
                                }
                            }
                            else
                            {
                                Log($"[Webhook] ⏭️ Evento invitation descartado, status: {invitation?.Status}");
                            }
                            break;

                        case "com.origo.mi.endpoint":
                            var endpoint = evt.Data.Deserialize<EndpointSeriDTO>();
                            if (endpoint == null)
                            {
                                Log("[Webhook] ❌ No se pudo deserializar endpoint.");
                                break;
                            }
                            Log($"[Webhook] 📱 Endpoint | EndpointId: {endpoint.EndpointId} - Modelo: {endpoint.EndpointModel} | Status: {endpoint.Status}");

                            if (endpoint.Status == "ENDPOINT_ACTIVE" && int.TryParse(endpoint.UserId, out int endpointUserId))
                            {
                                var dispositivoSincronizado = await _webhookEventService.CreateOrUpdateDeviceFromEndpoint(
                                    endpointUserId,
                                    endpoint,
                                    new Guid("E096DCEF-B118-4596-9FA0-676855A3FB53"),
                                    new Guid("739B4C8F-4DB1-4475-84D4-7644DCE00620"));

                                Log(dispositivoSincronizado
                                    ? $"[Webhook] ✅ Dispositivo sincronizado: {endpoint.EndpointId}"
                                    : $"[Webhook] ❌ No se pudo sincronizar el dispositivo {endpoint.EndpointId} (usuario HID '{endpoint.UserId}' no encontrado o error al guardar).");
                            }
                            break;

                        case "com.origo.mi.credential":
                            var credential = evt.Data.Deserialize<CredencialHIDSeriDTO>();
                            if (credential == null)
                            {
                                Log("[Webhook] ❌ No se pudo deserializar credential.");
                                break;
                            }
                            Log($"[Webhook] 🔑 Credencial emitida: MID #{credential.Mid} | Status: {credential.Status}");
                            if (credential.Status == "CREDENTIAL_ISSUED")
                            {
                                await _webhookEventService.CreateCredentialHID(Convert.ToInt32(credential.UserId), credential, new Guid("E096DCEF-B118-4596-9FA0-676855A3FB53"), new Guid("739B4C8F-4DB1-4475-84D4-7644DCE00620"));
                            }

                            break;

                        default:
                            Log($"[Webhook] ❓ Tipo de evento desconocido: {evt.Type}");
                            break;
                    }

                    Log($"[Webhook] ✅ Procesado OK | Type: {evt.Type}");
                }

                return StatusCode(200, "Webhook recibido y procesado exitosamente. El evento fue persistido en la base de datos.");
            }
            catch (Exception ex)
            {
                Log($"[Webhook] ❌ Error al procesar el webhook: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al intentar procesar el evento recibido desde HID Origo. Consulte los logs para más detalles.");
            }
        }
    }
}
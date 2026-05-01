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
        public WebhookController(
            IWebhookEventService webhookEventService
            )
        {
            _webhookEventService = webhookEventService;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveWebhookEvent()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(body))
                {
                    Console.WriteLine("[WebhookController] Cuerpo del webhook está vacío.");
                    return BadRequest("El cuerpo de la solicitud no puede estar vacío.");
                }

                var eventos = JsonSerializer.Deserialize<List<WebhookSeriDTO>>(body);
                if (eventos == null || eventos.Count == 0)
                {
                    Console.WriteLine("No se encontraron eventos en el cuerpo.");
                    return BadRequest("No se encontraron eventos válidos en el cuerpo.");
                }
                foreach (var evt in eventos)
                {
                    if (evt == null)
                    {
                        Console.WriteLine("[WebhookController] ⚠️ Evento o datos nulos. Se omite.");
                        continue;
                    }
                    switch (evt.Type)
                    {
                        case "com.origo.mi.invitation":
                            var invitation = evt.Data.Deserialize<InvitationSeriDTO>();
                            if (invitation == null)
                            {
                                Console.WriteLine("[WebhookController] ❌ No se pudo deserializar invitation.");
                                break;
                            }
                            Console.WriteLine($"Invitación recibida: {invitation.InvitationCode}");
                            if (invitation.Status == "INVITATION_ACKNOWLEDGED")
                            {
                                bool isUserIdValid = !string.IsNullOrWhiteSpace(invitation.UserId) && invitation.UserId != "undefined" && int.TryParse(invitation.UserId, out int userId);
                                bool isInvitationCodeValid = !string.IsNullOrWhiteSpace(invitation.InvitationCode) && invitation.InvitationCode != "undefined";

                                if (isUserIdValid && isInvitationCodeValid)
                                {
                                    Console.WriteLine($"✅ Invitación ACKNOWLEDGED recibida: {invitation.InvitationCode}");
                                    await _webhookEventService.PostWebhookEvents(Convert.ToInt32(invitation.UserId), invitation.InvitationCode, new Guid("E096DCEF-B118-4596-9FA0-676855A3FB53"), new Guid("739B4C8F-4DB1-4475-84D4-7644DCE00620"));
                                }
                                else
                                {
                                    Console.WriteLine("❌ Datos incompletos o inválidos en la invitación:");
                                    Console.WriteLine($"UserId: '{invitation.UserId}'");
                                    Console.WriteLine($"InvitationCode: '{invitation.InvitationCode}'");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"⏭️ Evento invitation descartado, status: {invitation?.Status}");
                            }
                            break;

                        case "com.origo.mi.endpoint":
                            var endpoint = evt.Data.Deserialize<EndpointSeriDTO>();
                            Console.WriteLine($"Endpoint activo: {endpoint.EndpointId} - Modelo: {endpoint.EndpointModel}");
                            break;

                        case "com.origo.mi.credential":
                            var credential = evt.Data.Deserialize<CredencialHIDSeriDTO>();
                            Console.WriteLine($"Credencial emitida: MID #{credential.Mid}");
                            if (credential == null)
                            {
                                Console.WriteLine("[WebhookController] ❌ No se pudo deserializar invitation.");
                                break;
                            }
                            if (credential.Status == "CREDENTIAL_ISSUED")
                            {
                                await _webhookEventService.CreateCredentialHID(Convert.ToInt32(credential.UserId), credential, new Guid("E096DCEF-B118-4596-9FA0-676855A3FB53"), new Guid("739B4C8F-4DB1-4475-84D4-7644DCE00620"));
                            }

                            break;

                        default:
                            Console.WriteLine($"Tipo de evento desconocido: {evt.Type}");
                            break;
                    }
                }

                Console.WriteLine("Evento recibido:");
                Console.WriteLine(body);

                return StatusCode(200, "Webhook recibido y procesado exitosamente. El evento fue persistido en la base de datos.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al intentar procesar el evento recibido desde HID Origo. Consulte los logs para más detalles.");
            }
        }
    }
}
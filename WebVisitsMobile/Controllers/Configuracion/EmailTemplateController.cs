using Microsoft.AspNetCore.Mvc;
using WebVisitsMobile.Models.Configuracion.Email;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Configuracion
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Configuración")]
    public class EmailTemplateController : ControllerBase
    {
        private readonly IEmailTemplateService _emailService;

        public EmailTemplateController(IEmailTemplateService emailService)
        {
            _emailService = emailService;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET api/EmailTemplate/{companyId}
        // Retorna la plantilla HTML con los placeholders {{...}} SIN reemplazar.
        // El frontend la usa para mostrar el editor o el preview estructurado.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetTemplate(Guid companyId)
        {
            var result = await _emailService.GetTemplate(companyId);

            if (result == null)
            {
                return StatusCode(404, new ApiResponse<string>(
                    false,
                    "No se encontró plantilla para la empresa especificada.",
                    404, null));
            }

            return StatusCode(200, new ApiResponse<EmailResponseDTO>(
                true,
                "Plantilla obtenida correctamente.",
                200, result));
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET api/EmailTemplate/{companyId}/preview
        // Retorna la plantilla HTML con los placeholders YA REEMPLAZADOS
        // por datos de muestra (estáticos). Útil para "Probar plantilla".
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{companyId}/Preview")]
        public async Task<IActionResult> GetPreview(Guid companyId)
        {
            var html = await _emailService.GetRenderedPreview(companyId);

            if (html == null)
            {
                return StatusCode(404, new ApiResponse<string>(
                    false,
                    "No se encontró plantilla para generar la vista previa.",
                    404, null));
            }

            // Devolvemos el HTML plano para que el frontend abra window.open()
            return Content(html, "text/html");
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST api/EmailTemplate/{companyId}
        // Persiste (crea o actualiza) la plantilla HTML completa en BD.
        // El cuerpo contiene el HTML como string en "templateHtml".
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost("{companyId}")]
        public async Task<IActionResult> SaveTemplate(Guid companyId, [FromBody] EmailReqDTO dto)
        {
            if (!Guid.TryParse(Request.Headers["Usuario"], out var usuarioId))
            {
                return BadRequest(new ApiResponse<string>(false,
                    "El header 'Usuario' es requerido y debe ser un GUID válido.", 400, null));
            }

            if (string.IsNullOrWhiteSpace(dto?.TemplateHtml))
            {
                return BadRequest(new ApiResponse<string>(false,
                    "El cuerpo de la solicitud debe contener 'templateHtml' con contenido.", 400, null));
            }

            var ok = await _emailService.SaveTemplate(companyId, usuarioId, dto.TemplateHtml);

            if (!ok)
            {
                return StatusCode(500, new ApiResponse<string>(false,
                    "No se pudo guardar la plantilla. Intenta de nuevo.", 500, null));
            }

            return StatusCode(200, new ApiResponse<string>(
                true, "La plantilla se guardó correctamente.", 200, null));
        }
    }
}
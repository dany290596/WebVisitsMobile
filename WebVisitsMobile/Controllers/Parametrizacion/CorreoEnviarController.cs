using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebVisitsMobile.Domain.Entities.Parametrizacion;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Services.Interfaces.Parametrizacion;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Parametrizacion
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Parametrización")]
    public class CorreoEnviarController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly ICorreoEnviarService _correoEnviarService;

        public CorreoEnviarController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            ICorreoEnviarService correoEnviarService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _correoEnviarService = correoEnviarService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingEmailWithConfiguration()
        {
            try
            {
                var data = await _correoEnviarService.GetPendingEmailsWithConfig();
                if (data == null || !data.Any())
                {
                    return StatusCode(200, new ApiResponse<IEnumerable<CorreoEnviarConfiguracion>>(
                        true,
                        "No se encontraron registros que coincidan con tu búsqueda",
                        200,
                        Enumerable.Empty<CorreoEnviarConfiguracion>()
                    ));
                }
                var response = new ApiResponse<IEnumerable<CorreoEnviarConfiguracion>>(true, "Consulta exitosa", 200, data);
                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Se produjo un error interno al procesar la solicitud.");
            }
        }

        [HttpPatch("{id}/mark-sent")]
        public async Task<IActionResult> MarkAsSent(Guid id)
        {
            var result = await _correoEnviarService.MarkAsSent(id);

            if (!result)
            {
                return StatusCode(400,
                    new ApiResponse<bool>(
                        false,
                        "No fue posible marcar como enviado.",
                        400,
                        false));
            }

            return StatusCode(200,
                new ApiResponse<bool>(
                    true,
                    "Se marcó como enviado.",
                    200,
                    true));
        }

        [HttpPatch("{id}/increment-attempt")]
        public async Task<IActionResult> IncreaseAttemptCount(Guid id)
        {
            var result = await _correoEnviarService.IncreaseAttemptCount(id);

            if (!result)
            {
                return StatusCode(400,
                    new ApiResponse<bool>(
                        false,
                        "No fue posible aumentar la marca del registro.",
                        400,
                        false));
            }

            return StatusCode(200,
                new ApiResponse<bool>(
                    true,
                    "Se aumentó la marca del registro correctamente.",
                    200,
                    true));
        }
    }
}
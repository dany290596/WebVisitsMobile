using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Services.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Administracion.Sesion
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Sesión")]
    public class SesionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly ISesionService _sesionService;

        public SesionController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            ISesionService sesionService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _sesionService = sesionService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> VerificarPrimeraConexion(Guid userId)
        {
            bool primeraConexion = await _sesionService.VerifyFirstConnection(userId);
            var response = new ApiResponse<bool>(true, "Consulta ejecutada", 200, primeraConexion);

            return StatusCode(200, response);
        }
    }
}
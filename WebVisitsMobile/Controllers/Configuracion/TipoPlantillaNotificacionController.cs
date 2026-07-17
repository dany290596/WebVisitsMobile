using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.Configuracion.TipoPlantillaNotificacion;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.QueryFilters.Configuracion;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Configuracion
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Configuración")]
    public class TipoPlantillaNotificacionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly ITipoPlantillaNotificacionService _plantillaNotificacionService;

        public TipoPlantillaNotificacionController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            ITipoPlantillaNotificacionService plantillaNotificacionService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _plantillaNotificacionService = plantillaNotificacionService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TipoPlantillaNotificacionRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] TipoPlantillaNotificacionQueryFilter filters)
        {
            try
            {
                if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
                {
                    return BadRequest("El header de la empresa es inválido.");
                }
                var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
                if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

                Token token = _accesorService.GetTokenData();
                if (token == null)
                {
                    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
                }

                var data = _plantillaNotificacionService.GetAll(filters, empresaId);
                var dataDTO = _mapper.Map<IEnumerable<TipoPlantillaNotificacionRespDTO>>(data);

                string strUriPreviousPage = _uriService.GetNotificationTemplateTypeUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetNotificationTemplateTypeUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<IEnumerable<TipoPlantillaNotificacionRespDTO>>(true, "Consulta exitosa.", 200, dataDTO);
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
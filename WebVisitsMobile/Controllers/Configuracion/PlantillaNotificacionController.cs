using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.Configuracion.PlantillaNotificacion;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.QueryFilters.Configuracion;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Configuracion
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Configuración")]
    public class PlantillaNotificacionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly IPlantillaNotificacionService _plantillaNotificacionService;

        public PlantillaNotificacionController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            IPlantillaNotificacionService plantillaNotificacionService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _plantillaNotificacionService = plantillaNotificacionService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<PlantillaNotificacionRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] PlantillaNotificacionQueryFilter filters)
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

                var data = await _plantillaNotificacionService.GetAll(filters, empresaId);
                var dataDTO = _mapper.Map<IEnumerable<PlantillaNotificacionRespDTO>>(data);

                string strUriPreviousPage = _uriService.GetNotificationTemplateUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetNotificationTemplateUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<IEnumerable<PlantillaNotificacionRespDTO>>(true, "Consulta exitosa.", 200, dataDTO);
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
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

                var data = await _plantillaNotificacionService.GetById(id, empresaId);
                var dataDTO = _mapper.Map<PlantillaNotificacionRespDTO>(data);
                var response = new ApiResponse<PlantillaNotificacionRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPatch("Inactivate")]
        public async Task<IActionResult> Inactivate([Required] Guid id)
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

            var result = await _plantillaNotificacionService.Inactivate(id, token.UsuarioId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible inactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se inactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPatch("Reactivate")]
        public async Task<IActionResult> Reactivate([Required] Guid id)
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

            var result = await _plantillaNotificacionService.Reactivate(id, token.UsuarioId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible reactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se reactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PlantillaNotificacionReqDTO data)
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

            var mapper = _mapper.Map<PlantillaNotificacion>(data);

            var name = await _plantillaNotificacionService.ExistsName(mapper.Nombre);
            if (name == true)
            {
                return StatusCode(409, new ApiResponse<bool>(false, "Ya existe una plantilla activa registrada con este nombre.", 409, false));
            }

            var tipoExiste = await _plantillaNotificacionService.ExistsTipoPlantilla(mapper.TipoPlantillaNotificacionId, empresaId);
            if (tipoExiste)
            {
                return StatusCode(409, new ApiResponse<bool>(false, "Ya existe una plantilla activa para este tipo de notificación.", 409, false));
            }

            bool book = await _plantillaNotificacionService.Create(mapper, token.UsuarioId, empresaId);
            if (!book)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));
            }

            PlantillaNotificacionRespDTO dto = _mapper.Map<PlantillaNotificacionRespDTO>(mapper);

            var response = new ApiResponse<PlantillaNotificacionRespDTO>(book, "El registro se creó correctamente.", 200, dto);

            return StatusCode(200, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, PlantillaNotificacionReqDTO data)
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

            var mapper = _mapper.Map<PlantillaNotificacion>(data);
            mapper.Id = id;

            var existeNombre = await _plantillaNotificacionService.ExistsNameForUpdate(id, data.Nombre);
            if (existeNombre)
            {
                return StatusCode(409, new ApiResponse<bool>(
                    false,
                    "Ya existe una plantilla activa con ese nombre.",
                    409,
                    false
                ));
            }

            var tipoExiste = await _plantillaNotificacionService.ExistsTipoPlantillaForUpdate(id, mapper.TipoPlantillaNotificacionId, empresaId);
            if (tipoExiste)
            {
                return StatusCode(409, new ApiResponse<bool>(false, "Ya existe otra plantilla activa para este tipo de notificación.", 409, false));
            }

            var result = await _plantillaNotificacionService.Update(mapper, token.UsuarioId, empresaId);
            if (!result)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));

            }
            var response = new ApiResponse<bool>(true, "Se actualizó correctamente.", 200, result);

            return StatusCode(200, response);
        }
    }
}
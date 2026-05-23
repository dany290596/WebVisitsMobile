using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Ubicacion;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.Ubicacion.Pais;
using WebVisitsMobile.Services.Interfaces.Ubicacion;
using WebVisitsMobile.Services.QueryFilters.Ubicacion;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Ubicacion
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Ubicación")]
    public class PaisController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly IPaisService _paisService;
        public PaisController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            IPaisService paisService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _paisService = paisService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<PaisRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] PaisQueryFilter filters)
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

                var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
                if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

                var data = await _paisService.GetAll(filters, empresaId);
                var dataDTO = _mapper.Map<IEnumerable<PaisRespDTO>>(data);

                string strUriPreviousPage = _uriService.GetCountryUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetCountryUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<IEnumerable<PaisRespDTO>>(true, "Consulta exitosa.", 200, dataDTO);
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

                var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
                if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

                var data = await _paisService.GetById(id, empresaId);
                var dataDTO = _mapper.Map<PaisRespDTO>(data);
                var response = new ApiResponse<PaisRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

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

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null)
            {
                return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 });
            }

            var result = await _paisService.Inactivate(id, token.UsuarioId);
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

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

            var result = await _paisService.Reactivate(id, token.UsuarioId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible reactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se reactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaisReqDTO data)
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

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

            var mapper = _mapper.Map<Pais>(data);

            var book = await _paisService.Create(mapper, token.UsuarioId);
            if (book == null)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));
            }

            PaisRespDTO dto = _mapper.Map<PaisRespDTO>(mapper);

            var response = new ApiResponse<PaisRespDTO>(true, "El registro se creó correctamente.", 200, dto);

            return StatusCode(200, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, PaisReqDTO data)
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

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

            var mapper = _mapper.Map<Pais>(data);
            mapper.Id = id;
            var result = await _paisService.Update(mapper, token.UsuarioId);
            if (!result)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));

            }
            var response = new ApiResponse<bool>(true, "Se actualizó correctamente.", 200, result);

            return StatusCode(200, response);
        }
    }
}
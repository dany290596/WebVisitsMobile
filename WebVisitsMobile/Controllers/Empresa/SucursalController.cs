using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.Empresa.Sucursal;
using WebVisitsMobile.Services.Interfaces.Empresa;
using WebVisitsMobile.Services.QueryFilters.Empresa;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Empresa
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Cliente")]
    public class SucursalController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly ISucursalService _sucursalService;

        public SucursalController(
            IMapper mapper,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            ISucursalService sucursalService
            )
        {
            _mapper = mapper;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _sucursalService = sucursalService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<SucursalRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] SucursalQueryFilter filters)
        {
            try
            {


                Token token = _accesorService.GetTokenData();
                if (token == null)
                {
                    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
                }

                var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
                if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

                var data = await _sucursalService.GetAll(filters);
                if (data == null)
                {
                    return StatusCode(500, new ApiResponse<string>(false, "Ocurrió un error al consultar la información.", 500, null));
                }

                var dataDTO = _mapper.Map<IEnumerable<SucursalRespDTO>>(data);

                var response = new ApiResponse<IEnumerable<SucursalRespDTO>>(true, "Consulta exitosa.", 200, dataDTO);
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, string.Empty, string.Empty);

                return StatusCode(200, response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {

                Token token = _accesorService.GetTokenData();
                if (token == null)
                {
                    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
                }

                var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
                if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

                var data = await _sucursalService.GetById(id);
                if (data == null)
                {
                    return StatusCode(404, new ApiResponse<string>(false, "No se encontró la sucursal solicitada.", 404, null));
                }

                var dataDTO = _mapper.Map<SucursalRespDTO>(data);
                var response = new ApiResponse<SucursalRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

                return StatusCode(200, response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(SucursalReqDTO data)
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

            var mapper = _mapper.Map<Sucursal>(data);

            var book = await _sucursalService.Create(mapper, token.UsuarioId);
            if (book == null)
            {
                return StatusCode(500, new ApiResponse<string>(false, "Ocurrió un error.", 500, null));
            }

            SucursalRespDTO dto = _mapper.Map<SucursalRespDTO>(book);

            var response = new ApiResponse<SucursalRespDTO>(true, "El registro se creó correctamente.", 200, dto);

            return StatusCode(200, response);
        }

        [HttpPost("Bulk")]
        public async Task<IActionResult> CreateBulk(List<SucursalBulkReqDTO> data)
        {
            if (data == null || data.Count == 0)
            {
                return BadRequest(new ApiResponse<string>(false, "El cuerpo de la petición es requerido.", 400, null));
            }

            Token token = _accesorService.GetTokenData();
            if (token == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
            }

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

            var entidades = _mapper.Map<List<Sucursal>>(data);

            var (creados, idsOmitidos) = await _sucursalService.CreateBulk(entidades, token.UsuarioId);

            var respuesta = new SucursalBulkRespDTO
            {
                TotalRecibidos = data.Count,
                TotalCreados = creados.Count,
                TotalOmitidos = idsOmitidos.Count,
                IdsOmitidos = idsOmitidos,
                Creados = _mapper.Map<List<SucursalRespDTO>>(creados)
            };

            var response = new ApiResponse<SucursalBulkRespDTO>(true, "El proceso de carga masiva finalizó.", 200, respuesta);

            return StatusCode(200, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, SucursalReqDTO data)
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

            var mapper = _mapper.Map<Sucursal>(data);
            mapper.Id = id;
            var result = await _sucursalService.Update(mapper, token.UsuarioId);
            if (!result)
            {
                return StatusCode(500, new ApiResponse<string>(false, "Ocurrió un error.", 500, null));
            }

            var response = new ApiResponse<bool>(true, "Se actualizó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPut("VincularEmpresaCliente/{id}")]
        public async Task<IActionResult> VincularEmpresaCliente(Guid id, SucursalVincularEmpresaClienteReqDTO data)
        {
            if (data == null || data.EmpresaClienteId == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(false, "El EmpresaClienteId es requerido.", 400, null));
            }

            var empresaExiste = await _plataformaService.ExistsCompany(data.EmpresaClienteId);
            if (empresaExiste == null)
            {
                return StatusCode(404, new ApiResponse<string>(false, $"La empresa con el ID {data.EmpresaClienteId} no existe.", 404, null));
            }

            Token token = _accesorService.GetTokenData();
            if (token == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
            }

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

            var result = await _sucursalService.VincularEmpresaCliente(id, data.EmpresaClienteId, token.UsuarioId);
            if (result == null)
            {
                return StatusCode(404, new ApiResponse<string>(false, "No se encontró la sucursal solicitada.", 404, null));
            }

            var dto = _mapper.Map<SucursalRespDTO>(result);
            var response = new ApiResponse<SucursalRespDTO>(true, "La empresa cliente se vinculó correctamente.", 200, dto);

            return StatusCode(200, response);
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

            var result = await _sucursalService.Inactivate(id, token.UsuarioId);
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

            var result = await _sucursalService.Reactivate(id, token.UsuarioId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible reactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se reactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }
    }
}

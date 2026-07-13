using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.Administracion.Sesion.Usuario;
using WebVisitsMobile.Services.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Services.QueryFilters.Administracion.Sesion;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Administracion.Sesion
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Autenticación")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;
        private readonly IHttpContextAccessor _accesor;
        private readonly IAccesorService _accesorService;
        private readonly IUriService _uriService;
        private readonly IPlataformaService _plataformaService;

        public UsuarioController(
            IUsuarioService usuarioService,
            IMapper mapper,
            IPasswordService passwordService,
            IHttpContextAccessor accesor,
            IAccesorService accesorService,
            IUriService uriService,
            IPlataformaService plataformaService
            )
        {
            _usuarioService = usuarioService;
            _mapper = mapper;
            _passwordService = passwordService;
            _accesorService = accesorService;
            _accesor = accesor;
            _uriService = uriService;
            _plataformaService = plataformaService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<UsuarioRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] UsuarioQueryFilter filters)
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
                    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso", 401, null));
                }

                var data = await _usuarioService.GetAll(filters, empresaId);
                var dataDTO = _mapper.Map<IEnumerable<UsuarioRespDTO>>(data);

                string strUriPreviousPage = _uriService.GetUserPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetUserPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<IEnumerable<UsuarioRespDTO>>(true, "Consulta exitosa.", 200, dataDTO);
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
            Token token = _accesorService.GetTokenData();
            if (token == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
            }

            var usuario = await _usuarioService.GetById(id);
            var cusuarioDTO = _mapper.Map<UsuarioRespDTO>(usuario);
            var response = new ApiResponse<UsuarioRespDTO>(true, "Consulta ejecutada.", 200, cusuarioDTO);

            return StatusCode(200, response);
        }

        [HttpPatch("{id}")]
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
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso", 401, null));
            }

            var result = await _usuarioService.Inactivate(id, token.UsuarioId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible inactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se inactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPatch("{id}")]
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
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso", 401, null));
            }

            var result = await _usuarioService.Reactivate(id, token.UsuarioId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible reactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se reactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserData()
        {
            Token token = _accesorService.GetTokenData();
            if (token == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
            }

            var dataDTO = _mapper.Map<TokenRespDTO>(token);
            var response = new ApiResponse<TokenRespDTO>(true, "Consulta ejecutada.", 200, dataDTO);

            return StatusCode(200, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioReqDTO data)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }
            data.EmpresaClienteId = empresaId;
            Token token = _accesorService.GetTokenData();
            if (token == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso", 401, null));
            }

            var mapper = _mapper.Map<Usuario>(data);
            string password = mapper.Contrasena;
            mapper.Contrasena = _passwordService.Hash(mapper.Contrasena);

            var email = await _usuarioService.GetUserByEmail(mapper.Correo);
            if (email != null)
            {
                return StatusCode(409, new ApiResponse<bool>(false, "Ya hay una cuenta registrada con este correo electrónico. Usa otro correo o intenta recuperar tu contraseña.", 409, false));
            }

            bool book = await _usuarioService.Create(mapper, password, token.UsuarioId, empresaId);
            if (!book)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));
            }

            UsuarioRespDTO dto = _mapper.Map<UsuarioRespDTO>(mapper);

            var response = new ApiResponse<UsuarioRespDTO>(book, "El usuario se creó correctamente.", 200, dto);

            return StatusCode(200, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, UsuarioReqDTO data)
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

            var mapper = _mapper.Map<Usuario>(data);
            mapper.Id = id;
            if (!string.IsNullOrWhiteSpace(data.Contrasena))
            {
                mapper.Contrasena = _passwordService.Hash(data.Contrasena);
            }
            var result = await _usuarioService.Update(mapper, token.UsuarioId, empresaId);
            if (!result)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));
            }
            var response = new ApiResponse<bool>(true, "Se actualizó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [Route("ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(CambiarContrasena nuevaContasena)
        {
            Token tokenData = _accesorService.GetTokenData();

            if (tokenData.UsuarioId == Guid.Empty)
            {
                return StatusCode(401, new ApiResponse<string>(false, "no tiene permiso sobre este recurso", 401, null));
            }

            var contrasena = _passwordService.Hash(nuevaContasena.Contrasena);

            bool cambiarContrasena = await _usuarioService.ChangePassword(contrasena, tokenData.Email);

            return StatusCode(200, cambiarContrasena);
        }
    }
}
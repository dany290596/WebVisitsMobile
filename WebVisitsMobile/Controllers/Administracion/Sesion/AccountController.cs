using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Services.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Administracion.Sesion
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Mi cuenta")]
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly IAccountService _accountService;
        public AccountController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            IAccountService accountService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _accountService = accountService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<AccountInfo>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAccountInfo()
        {
            try
            {
                // Validar header de empresa
                if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
                {
                    return BadRequest(new ApiResponse<string>(false, "El header de la empresa es inválido.", 400, null));
                }

                var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
                if (empresaExiste == null)
                {
                    return BadRequest(new ApiResponse<string>(false, $"La empresa con el ID {empresaId} no existe.", 400, null));
                }

                // Validar token
                Token token = _accesorService.GetTokenData();
                if (token == null)
                {
                    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
                }

                // Obtener información de la cuenta (usando el userId del token)
                var accountInfo = await _accountService.GetAccountInfo(token.UsuarioId);
                if (accountInfo == null)
                {
                    return NotFound(new ApiResponse<string>(false, "Usuario no encontrado o inactivo.", 404, null));
                }

                var response = new ApiResponse<AccountInfo>(true, "Consulta exitosa.", 200, accountInfo);
                return StatusCode(200, response);
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>(false, "Error interno del servidor.", 500, null));
            }
        }
    }
}
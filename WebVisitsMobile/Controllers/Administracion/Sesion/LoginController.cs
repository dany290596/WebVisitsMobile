using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Services.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Administracion.Sesion
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Autenticación")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioService _usuarioService;
        private readonly IPasswordService _passwordService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISesionService _sesionService;

        public LoginController(
            IConfiguration configuration,
            IUsuarioService usuarioService,
            IPasswordService passwordService,
            IHttpContextAccessor httpContextAccessor,
            ISesionService sesionService
            )
        {
            _configuration = configuration;
            _usuarioService = usuarioService;
            _passwordService = passwordService;
            _httpContextAccessor = httpContextAccessor;
            _sesionService = sesionService;
        }

        [HttpPost]
        public async Task<IActionResult> Autenticacion(Login login)
        {
            var validation = await IsValidUser(login);

            if (validation.Item1)
            {
                if (UserExpired(validation.Item2.FechaVencimiento))
                {
                    var respuestaVencido = new ApiResponse<string>(
                        false,
                        "El usuario ha vencido. Por favor contacte al administrador.",
                        407,
                        ""
                    );
                    return StatusCode(403, respuestaVencido);
                }
                if (validation.Item2.Estado == 2)
                {
                    var respuestaVencido = new ApiResponse<string>(
                        false,
                        "No puedes iniciar sesión porque tu cuenta se encuentra deshabilitada. Contacta al administrador del sistema.",
                        408,
                        ""
                    );
                    return StatusCode(403, respuestaVencido);
                }
                string ipAddress = _httpContextAccessor.HttpContext!.Connection.RemoteIpAddress!.ToString();
                Domain.Entities.Administracion.Sesion.Sesion sesion = new Domain.Entities.Administracion.Sesion.Sesion();
                sesion.PerfilId = validation.Item2.PerfilId;
                sesion.UsuarioId = validation.Item2.Id;
                sesion.FechaInicio = DateTime.Now;
                sesion.DireccionIp = ipAddress;
                sesion.Id = new Guid();
                await _sesionService.Insert(sesion, validation.Item2.Id, validation.Item2.Id);

                if (validation.Item2.TipoUsuario.Nombre == "Api" && validation.Item2.Correo == "WebVisits")
                {
                    var token = GenerateToken(validation.Item2, sesion.Id);
                    var repuesta = new ApiResponse<string>(true, "Token generado correctamente.", 200, token);
                    return StatusCode(200, repuesta);
                }
                if (validation.Item2.TipoUsuario.Nombre == "Partner HID")
                {
                    var token = GenerateToken(validation.Item2, sesion.Id);
                    var repuesta = new ApiResponse<string>(true, "Token generado correctamente.", 200, token);
                    return StatusCode(200, repuesta);
                }
                if (validation.Item2.TipoUsuario.Nombre == "Desarrollador")
                {
                    var token = GenerateToken(validation.Item2, sesion.Id);
                    var repuesta = new ApiResponse<string>(true, "Token generado correctamente.", 200, token);
                    return StatusCode(200, repuesta);
                }
                if (validation.Item2.TipoUsuario.Nombre == "Equipo CRC")
                {
                    var token = GenerateToken(validation.Item2, sesion.Id);
                    var repuesta = new ApiResponse<string>(true, "Token generado correctamente.", 200, token);
                    return StatusCode(200, repuesta);
                }

                return StatusCode(401, new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, ""));
            }

            var response = new ApiResponse<string>(false, "Usuario no válido.", 404, "");

            return StatusCode(404, response);
        }

        private async Task<(bool, Usuario)> IsValidUser(Login login)
        {
            var userName = await _usuarioService.GetUserForCredentials(login);

            bool isValid = false;

            if (userName != null)
            {
                isValid = _passwordService.Check(userName.Contrasena, login.Contrasena);
            }

            return (isValid, userName);
        }

        private string GenerateToken(Usuario usuario, Guid sesionId)
        {
            // Header
            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));

            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            // Claims
            var claims = new[]
            {
                new Claim("Correo", usuario.Correo),
                new Claim("PerfilId", usuario.PerfilId.ToString()),
                new Claim("UsuarioId", usuario.Id.ToString()),
                new Claim("EmpresaId", usuario.EmpresaClienteId.ToString()!),
                new Claim("AsociadoId", usuario.IdAsociado.ToString()),
                new Claim("TipoUsuarioId", usuario.TipoUsuarioId.ToString()),
                new Claim("SesionId", sesionId.ToString())
            };

            // Payload
            var payload = new JwtPayload
            (
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claims,
                DateTime.Now,
                DateTime.UtcNow.AddMinutes(60)
            );

            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool UserExpired(DateTime? fechaVencimiento)
        {
            if (fechaVencimiento == null)
                return false;

            return DateTime.Now.Date > fechaVencimiento.Value.Date;
        }
    }
}
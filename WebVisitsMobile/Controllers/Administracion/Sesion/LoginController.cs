using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Infrastructure.Options;
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
        private readonly AesManagedOption _aesManagedOption;

        public LoginController(
            IConfiguration configuration,
            IUsuarioService usuarioService,
            IPasswordService passwordService,
            IHttpContextAccessor httpContextAccessor,
            ISesionService sesionService,
            IOptions<AesManagedOption> aesManagedOption
            )
        {
            _configuration = configuration;
            _usuarioService = usuarioService;
            _passwordService = passwordService;
            _httpContextAccessor = httpContextAccessor;
            _sesionService = sesionService;
            _aesManagedOption = aesManagedOption.Value;
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
                if (validation.Item2.TipoUsuario.Id == new Guid("2228D6FB-CBDD-4672-9A06-A6E054157E6D"))
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

        [HttpPost("LoginWebVisitsMeeting")]
        public async Task<IActionResult> LoginWebVisitsMeeting(LoginEncriptado loginEncriptado)
        {
            string email;
            string contrasena;

            try
            {
                email = Decrypt(loginEncriptado.Email);
                contrasena = Decrypt(loginEncriptado.Contrasena);
            }
            catch
            {
                var respuestaError = new ApiResponse<string>(false, "Las credenciales no pudieron ser desencriptadas.", 400, "");
                return StatusCode(400, respuestaError);
            }

            var login = new Login { Email = email, Contrasena = contrasena };
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
                    var respuestaDeshabilitado = new ApiResponse<string>(
                        false,
                        "No puedes iniciar sesión porque tu cuenta se encuentra deshabilitada. Contacta al administrador del sistema.",
                        408,
                        ""
                    );
                    return StatusCode(403, respuestaDeshabilitado);
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

            return StatusCode(404, new ApiResponse<string>(false, "Usuario no válido.", 404, ""));
        }

        private string Decrypt(string encryptedText)
        {
            // Key: SHA-256 de StartupVector → 32 bytes (AES-256)
            // IV:  MD5 de StartupVector → 16 bytes (tamaño de bloque AES)
            var vectorBytes = Encoding.UTF8.GetBytes(_aesManagedOption.StartupVector);

            byte[] keyBytes;
            byte[] ivBytes;

            using (var sha256 = SHA256.Create())
                keyBytes = sha256.ComputeHash(vectorBytes);

            using (var md5 = MD5.Create())
                ivBytes = md5.ComputeHash(vectorBytes);

            var cipherBytes = Convert.FromBase64String(encryptedText);

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(cipherBytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs, Encoding.UTF8);

            return reader.ReadToEnd();
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
                new Claim("PerfilName", usuario.Perfil.Nombre.ToString()),
                new Claim("UsuarioId", usuario.Id.ToString()),
                new Claim("EmpresaId", usuario.EmpresaClienteId.ToString()!),
                new Claim("AsociadoId", usuario.IdAsociado.ToString()),
                new Claim("TipoUsuarioId", usuario.TipoUsuarioId.ToString()),
                new Claim("TipoUsuarioName", usuario.TipoUsuario.Nombre.ToString()),
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

        [Route("GetRecoveryCode")]
        [HttpGet]
        public async Task<IActionResult> GetRecoveryCode(string correo)
        {

            bool validarCorreo = await _usuarioService.ValidateUserEmail(correo);
            if (validarCorreo == false) { return BadRequest("El usuario que desea recuperar no existe"); }
            Random random = new Random();
            int numero = random.Next(10000000, 99999999);
            string clave = numero.ToString();

            bool enviarCorreo = await _usuarioService.SendRecoveryCode(correo, numero.ToString(), clave);

            return StatusCode(200, enviarCorreo);
        }

        [Route("GetValidateCode")]
        [HttpGet]
        public async Task<IActionResult> GetValidateCode(string codigo, string correo)
        {

            bool validarFechaIntento = await _usuarioService.ValidateAttemptDate(codigo, correo);
            if (validarFechaIntento == false) { return BadRequest("Numero de intentos superado espere 1 hora para volver a probar"); }

            bool validarCorreo = await _usuarioService.ValidateUserEmail(correo);
            if (validarCorreo == false) { return BadRequest("El usuario que desea recuperar no existe"); }


            bool validarCodigo = await _usuarioService.ValidateCode(codigo, correo);
            if (validarCodigo == false)
            {
                int ValidarIntentos = await _usuarioService.ValidateIntent(codigo, correo);
                if (ValidarIntentos == 7) { return BadRequest("Numero de intentos superado espere 1 hora para volver a probar"); }
            }

            return StatusCode(200, validarCodigo);
        }

        [Route("RecoverPassword")]
        [HttpPost]
        public async Task<IActionResult> RecoverPassword(NuevaContasena nuevaContasena)
        {
            bool validarFechaIntento = await _usuarioService.ValidateIntentDate(nuevaContasena.Codigo!, nuevaContasena.Correo!);
            if (validarFechaIntento == false) { return BadRequest("Numero de intentos superado espere 1 hora para volver a probar"); }

            bool validarCorreo = await _usuarioService.ValidateUserEmail(nuevaContasena.Correo!);
            if (validarCorreo == false) { return BadRequest("El usuario que desea recuperar no existe"); }

            bool validarCodigo = await _usuarioService.ValidateCode(nuevaContasena.Codigo!, nuevaContasena.Correo!);
            if (validarCodigo == false)
            {
                int ValidarIntentos = await _usuarioService.ValidateIntent(nuevaContasena.Codigo!, nuevaContasena.Correo!);
                if (ValidarIntentos == 7) { return BadRequest("Numero de intentos superado espere 1 hora para volver a probar"); }
                if (validarCodigo == false) { return BadRequest("Ocurrio un error, datos incorrectos"); }
            }

            var contrasena = _passwordService.Hash(nuevaContasena.Contrasena);

            bool cambiarContrasena = await _usuarioService.ChangePassword(contrasena, nuevaContasena.Correo);

            return StatusCode(200, cambiarContrasena);
        }
    }
}
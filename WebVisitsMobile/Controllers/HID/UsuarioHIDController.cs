using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Models.HID.UserHID;
using WebVisitsMobile.Models.Organizacion.Tarea.Tarea;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.QueryFilters.HID;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.HID
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "HID Origo")]
    public class UsuarioHIDController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly ILicenciaUserHIDService _licenciaUserHIDService;
        private readonly IUsuarioHidTipoCredencialService _usuarioHidTipoCredencialService;

        public UsuarioHIDController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            ILicenciaUserHIDService licenciaUserHIDService,
            IUsuarioHidTipoCredencialService usuarioHidTipoCredencial
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _licenciaUserHIDService = licenciaUserHIDService;
            _usuarioHidTipoCredencialService = usuarioHidTipoCredencial;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<UserHIDRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] LicenciaUserHIDQueryFilter filters)
        {
            try
            {
                //Token token = _accesorService.GetTokenData();
                //if (token == null)
                //{
                //    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
                //}

                var data = await _licenciaUserHIDService.GetAll(filters);
                if (data == null || !data.Any())
                {
                    return StatusCode(200, new ApiResponse<IEnumerable<UserHIDRespDTO>>(
                        true,
                        "No se encontraron registros que coincidan con tu búsqueda",
                        200,
                        Enumerable.Empty<UserHIDRespDTO>()
                    ));
                }
                var dataDTO = _mapper.Map<IEnumerable<UserHIDRespDTO>>(data);

                string strUriPreviousPage = _uriService.GetLicenseUserHIDPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetLicenseUserHIDPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<IEnumerable<UserHIDRespDTO>>(true, "Consulta exitosa", 200, dataDTO);
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("Query")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<UserHIDRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllQuery([FromQuery] LicenciaUserHIDQueryFilter filters)
        {
            try
            {
                //Token token = _accesorService.GetTokenData();
                //if (token == null)
                //{
                //    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
                //}

                var data = await _licenciaUserHIDService.GetAllQuery(filters);
                if (data == null || !data.Any())
                {
                    return StatusCode(200, new ApiResponse<IEnumerable<UserHIDRespDTO>>(
                        true,
                        "No se encontraron registros que coincidan con tu búsqueda",
                        200,
                        Enumerable.Empty<UserHIDRespDTO>()
                    ));
                }

                string strUriPreviousPage = _uriService.GetLicenseUserHIDPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetLicenseUserHIDPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<IEnumerable<CommonDTO>>(true, "Consulta exitosa", 200, data.ToList());
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
                //Token token = _accesorService.GetTokenData();
                //if (token == null)
                //{
                //    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
                //}

                var data = await _licenciaUserHIDService.GetById(id);
                if (data == null)
                {
                    return StatusCode(400, new ApiResponse<UserHIDRespDTO>(false, "El registro no existe.", 400, null));
                }
                var dataDTO = _mapper.Map<UserHIDRespDTO>(data);
                var response = new ApiResponse<UserHIDRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("Photo/{id}")]
        public async Task<IActionResult> GetByPhoto(Guid id)
        {
            try
            {
                //Token token = _accesorService.GetTokenData();
                //if (token == null)
                //{
                //    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
                //}

                var data = await _licenciaUserHIDService.GetByPhoto(id);
                if (data == null)
                {
                    return StatusCode(400, new ApiResponse<UserHIDRespDTO>(false, "El registro no existe.", 400, null));
                }
                var dataDTO = _mapper.Map<UserHIDRespDTO>(data);
                var response = new ApiResponse<UserHIDRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserHIDReqDTO data)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

            var mapper = _mapper.Map<LicenciaHidUser>(data);

            bool book = await _licenciaUserHIDService.Create(mapper, empresaId, data.UsuarioCreadorId);
            if (!book)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));
            }

            UserHIDRespDTO dto = _mapper.Map<UserHIDRespDTO>(mapper);

            var response = new ApiResponse<UserHIDRespDTO>(book, "El registro se creó correctamente.", 200, dto);

            return StatusCode(200, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, UserHIDEditDTO data)
        {
            //Token token = _accesorService.GetTokenData();
            //if (token == null)
            //{
            //    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
            //}

            var mapper = _mapper.Map<LicenciaHidUser>(data);
            mapper.Id = id;
            var result = await _licenciaUserHIDService.Update(mapper, data.UsuarioCreadorId);
            if (!result)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));

            }
            var response = new ApiResponse<bool>(true, "Se actualizó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPut("Partial/{id}")]
        public async Task<IActionResult> Update(Guid id, UserHIDInvitationReqDTO data)
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

            var tokenData = _accesorService.GetTokenData();
            var mapper = _mapper.Map<LicenciaHidUser>(data);
            mapper.Id = id;
            var result = await _licenciaUserHIDService.UpdatePartial(mapper, empresaId, tokenData.UsuarioId);
            if (result == null)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, string.Empty));
            }
            var response = new ApiResponse<LicenciaHidUser>(true, "Se actualizó correctamente.", 200, result);

            return StatusCode(200, response);
        }


        [HttpPut("ActualizarCredencial/{correo}")]
        public async Task<IActionResult> ActualizarCredencial(string correo)
        {
            Token token = _accesorService.GetTokenData();
            if (token == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
            }

            var result = await _licenciaUserHIDService.ActualizarCredencial(correo, token.EmpresaId, token.UsuarioId);
            if (result == null)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, string.Empty));
            }

            TareaRespDTO dto = _mapper.Map<TareaRespDTO>(result);

            var response = new ApiResponse<TareaRespDTO>(true, "Se actualizó correctamente.", 200, dto);

            return StatusCode(200, response);
        }

        [HttpGet("GetAllExpired")]
        public async Task<IActionResult> GetAllExpired()
        {
            var users = await _licenciaUserHIDService.GetAllExpired();
            if (!users.Any())
            {
                var emptyResponse = new ApiResponse<List<UserHIDExpired>>(
                            true,
                             "No se encontraron registros que coincidan con los criterios especificados.",
                            200,
                            new List<UserHIDExpired>());

                return StatusCode(200, emptyResponse);
            }

            var response = new ApiResponse<List<UserHIDExpired>>(
                true,
                "Consulta ejecutada",
                200,
                users
            );

            return StatusCode(200, response);
        }

        [HttpGet("GetByIdExpired/{id}")]
        public async Task<IActionResult> GetUserByIdExpired([FromRoute][Required] Guid id)
        {
            var user = await _licenciaUserHIDService.GetByIdExpired(id);
            if (user == null)
            {
                return StatusCode(404, new ApiResponse<string>(
                    false,
                    $"El usuario con el ID {id} no fue encontrado.",
                    404,
                    null
                ));
            }

            var licenciaEstado = await _licenciaUserHIDService.GetExpired(id);
            if (licenciaEstado == null)
            {
                return StatusCode(404, new ApiResponse<string>(
                    false,
                    $"No se encontró al usuario asociado al identificador '{id}'.",
                    404,
                    null
                ));
            }

            var response = new ApiResponse<UserHIDExpired>(
                true,
                "Consulta ejecutada",
                200,
                licenciaEstado
            );

            return StatusCode(200, response);
        }


        [HttpPatch("InactivateCredentialUser")]
        public async Task<IActionResult> InactivateCredentialUser([Required] Guid id, [Required] Guid usuarioBajaId)
        {
            //Token token = _accesorService.GetTokenData();
            //if (token == null)
            //{
            //    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
            //}

            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

            var userTipoCredencial = await _usuarioHidTipoCredencialService.GetUserHidTypeCredential(id);
            if (userTipoCredencial == null)
            {
                return StatusCode(400, new ApiResponse<UserHIDRespDTO>(false, "El registro no existe.", 400, null));
            }

            if (userTipoCredencial.TipoCredencialId == new Guid("2B3C4D5E-6F70-8901-BCDE-F12345678901"))
            {
                var result = await _licenciaUserHIDService.InactivateWithWalletAndTask(userTipoCredencial.LicenciaHidUserId, usuarioBajaId, empresaId);
                if (result == true)
                {
                    await _usuarioHidTipoCredencialService.Inactivate(id, usuarioBajaId);
                }
                var response = result
                ? new ApiResponse<bool>(true, "El usuario ha sido inactivado correctamente.", 200, true)
                : new ApiResponse<bool>(false, "Ocurrió un error al intentar inactivar al usuario o la tarea asociada.", 400, false);

                return StatusCode(response.Codigo, response);
            }

            if (userTipoCredencial.TipoCredencialId == new Guid("1A2B3C4D-5E6F-7890-ABCD-EF1234567890"))
            {
                if (userTipoCredencial.LicenciaHidUserId == Guid.Empty || userTipoCredencial.LicenciaHidUser.UserId <= 0 || userTipoCredencial.LicenciaHidUser.UserId == null)
                {
                    var result = await _licenciaUserHIDService.InactivateWithHIDAndTask(userTipoCredencial.LicenciaHidUserId, usuarioBajaId, empresaId);
                    if (result == true)
                    {
                        await _usuarioHidTipoCredencialService.Inactivate(id, usuarioBajaId);
                    }
                    var response = result
                    ? new ApiResponse<bool>(true, "El usuario ha sido inactivado correctamente.", 200, true)
                    : new ApiResponse<bool>(false, "Ocurrió un error al intentar inactivar al usuario o la tarea asociada.", 400, false);

                    return StatusCode(response.Codigo, response);
                }

                try
                {
                    var result = await _licenciaUserHIDService.InactivateWithHID(userTipoCredencial.LicenciaHidUserId, empresaId, usuarioBajaId);
                    if (!result)
                    {
                        var response = new ApiResponse<string>(false, "No se pudo inactivar el usuario. Verifica los datos o intenta nuevamente.", 400, null);
                        return StatusCode(response.Codigo, response);
                    }
                    else
                    {
                        await _usuarioHidTipoCredencialService.Inactivate(id, usuarioBajaId);
                        var response = new ApiResponse<bool>(true, "El usuario ha sido inactivado correctamente.", 200, result);
                        return StatusCode(response.Codigo, response);
                    }
                }
                catch (Exception ex)
                {
                    var response = new ApiResponse<string>(false, "Se produjo un error inesperado al procesar la solicitud.", 500, null);
                    return StatusCode(response.Codigo, response);
                }
            }

            return BadRequest("El tipo de credencial no es válido.");
        }


        [HttpPatch("InactivateCredentialExternalId")]
        public async Task<IActionResult> InactivateCredentialExternalId([Required] Guid externalId)
        {
            Token token = _accesorService.GetTokenData();
            if (token == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
            }

            var usuario = await _licenciaUserHIDService.GetByExternalId(externalId);
            if (usuario == null)
            {
                return StatusCode(404, new ApiResponse<string>(false, $"No se encontró un usuario con el ExternalId '{externalId}'.", 404, null));
            }

            var userTipoCredencial = await _usuarioHidTipoCredencialService.GetByLicenciaHidUserId(usuario.Id);
            if (userTipoCredencial == null)
            {
                return StatusCode(400, new ApiResponse<UserHIDRespDTO>(false, "El registro no existe.", 400, null));
            }

            var empresaId = token.EmpresaId;
            var usuarioBajaId = token.UsuarioId;

            if (userTipoCredencial.TipoCredencialId == new Guid("2B3C4D5E-6F70-8901-BCDE-F12345678901"))
            {
                var result = await _licenciaUserHIDService.InactivateWithWalletAndTask(userTipoCredencial.LicenciaHidUserId, usuarioBajaId, empresaId);
                if (result == true)
                {
                    await _usuarioHidTipoCredencialService.Inactivate(userTipoCredencial.Id, usuarioBajaId);
                }
                var response = result
                ? new ApiResponse<bool>(true, "El usuario ha sido inactivado correctamente.", 200, true)
                : new ApiResponse<bool>(false, "Ocurrió un error al intentar inactivar al usuario o la tarea asociada.", 400, false);

                return StatusCode(response.Codigo, response);
            }

            if (userTipoCredencial.TipoCredencialId == new Guid("1A2B3C4D-5E6F-7890-ABCD-EF1234567890"))
            {
                if (userTipoCredencial.LicenciaHidUserId == Guid.Empty || userTipoCredencial.LicenciaHidUser.UserId <= 0 || userTipoCredencial.LicenciaHidUser.UserId == null)
                {
                    var result = await _licenciaUserHIDService.InactivateWithHIDAndTask(userTipoCredencial.LicenciaHidUserId, usuarioBajaId, empresaId);
                    if (result == true)
                    {
                        await _usuarioHidTipoCredencialService.Inactivate(userTipoCredencial.Id, usuarioBajaId);
                    }
                    var response = result
                    ? new ApiResponse<bool>(true, "El usuario ha sido inactivado correctamente.", 200, true)
                    : new ApiResponse<bool>(false, "Ocurrió un error al intentar inactivar al usuario o la tarea asociada.", 400, false);

                    return StatusCode(response.Codigo, response);
                }

                try
                {
                    var result = await _licenciaUserHIDService.InactivateWithHID(userTipoCredencial.LicenciaHidUserId, empresaId, usuarioBajaId);
                    if (!result)
                    {
                        var response = new ApiResponse<string>(false, "No se pudo inactivar el usuario. Verifica los datos o intenta nuevamente.", 400, null);
                        return StatusCode(response.Codigo, response);
                    }
                    else
                    {
                        await _usuarioHidTipoCredencialService.Inactivate(userTipoCredencial.Id, usuarioBajaId);
                        var response = new ApiResponse<bool>(true, "El usuario ha sido inactivado correctamente.", 200, result);
                        return StatusCode(response.Codigo, response);
                    }
                }
                catch (Exception ex)
                {
                    var response = new ApiResponse<string>(false, "Se produjo un error inesperado al procesar la solicitud.", 500, null);
                    return StatusCode(response.Codigo, response);
                }
            }

            return BadRequest("El tipo de credencial no es válido.");
        }

        /// <summary>
        /// Obtiene las licencias HID cuya FechaFin ya expiró y tienen Plataforma asignada.
        /// </summary>
        [HttpGet("GetLicenciasExpiradas")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<LicenciaHidUserExpiradaRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetLicenciasExpiradas()
        {
            try
            {
                var data = await _licenciaUserHIDService.GetAllLicenciasExpiradas();
                if (data == null || !data.Any())
                {
                    return StatusCode(200, new ApiResponse<IEnumerable<LicenciaHidUserExpiradaRespDTO>>(
                        true,
                        "No se encontraron licencias expiradas con plataforma asignada.",
                        200,
                        Enumerable.Empty<LicenciaHidUserExpiradaRespDTO>()));
                }

                var dataDTO = data.Select(u => new LicenciaHidUserExpiradaRespDTO
                {
                    Id = u.Id,
                    EmpresaClienteId = u.EmpresaClienteId,
                    FechaFin = u.FechaFin,
                    Plataforma = u.Plataforma,
                    ExternalId = u.ExternalId,
                    UsuarioWalletId = u.UsuarioWalletId
                });

                var response = new ApiResponse<IEnumerable<LicenciaHidUserExpiradaRespDTO>>(
                    true, "Consulta exitosa", 200, dataDTO);

                return StatusCode(200, response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene el UsuarioInvitacionDetalle de la licencia HID vigente más reciente asociada al correo electrónico.
        /// </summary>
        [HttpGet("GetInvitacionDetalle")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<InvitacionDetalleRespDTO>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInvitacionDetalleByEmail([Required][FromQuery] string email)
        {
            try
            {

                //Token token = _accesorService.GetTokenData();
                //if (token.Email==string.Empty)
                //{
                //    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
                //}

                var emailExiste = await _licenciaUserHIDService.ExisteEmailEnLicenciaHidUser(email);
                if (!emailExiste)
                {
                    return StatusCode(404, new ApiResponse<InvitacionDetalleRespDTO>(
                        false,
                        "No se encontraron registros para el correo proporcionado.",
                        404,
                        null));
                }

                var invitacionDetalle = await _licenciaUserHIDService.GetInvitacionDetalleVigenteByEmail(email);
                if (invitacionDetalle == null)
                {
                    return StatusCode(404, new ApiResponse<InvitacionDetalleRespDTO>(
                        false,
                        "No existe una licencia HID válida para el correo proporcionado.",
                        404,
                        null));
                }

                var response = new ApiResponse<InvitacionDetalleRespDTO>(
                    true,
                    "Consulta exitosa",
                    200,
                    new InvitacionDetalleRespDTO { UsuarioInvitacionDetalle = invitacionDetalle });

                return StatusCode(200, response);
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<InvitacionDetalleRespDTO>(
                    false,
                    "Ocurrió un error interno al procesar la solicitud.",
                    500,
                    null));
            }
        }

        /// <summary>
        /// Obtiene el CredencialValor Wallet más reciente del usuario identificado por su ExternalId.
        /// </summary>
        [HttpGet("GetCredencialWalletByExternalId")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<CredencialWalletRespDTO>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCredencialWalletByExternalId([Required][FromQuery] Guid externalId)
        {
            try
            {
                var usuario = await _licenciaUserHIDService.GetByExternalId(externalId);
                if (usuario == null)
                {
                    return StatusCode(404, new ApiResponse<CredencialWalletRespDTO>(
                        false,
                        $"No se encontró un usuario con el ExternalId '{externalId}'.",
                        404,
                        null));
                }

                var tieneWallet = await _licenciaUserHIDService.TieneCredencialWallet(usuario.Id);
                if (!tieneWallet)
                {
                    return StatusCode(404, new ApiResponse<CredencialWalletRespDTO>(
                        false,
                        "El usuario no tiene una credencial de tipo Wallet asignada.",
                        404,
                        null));
                }

                var credencialValor = await _licenciaUserHIDService.GetCredencialWalletMasReciente(usuario.Id);
                if (credencialValor == null)
                {
                    return StatusCode(404, new ApiResponse<CredencialWalletRespDTO>(
                        false,
                        "El usuario no tiene registros en la tabla de credenciales.",
                        404,
                        null));
                }

                var response = new ApiResponse<CredencialWalletRespDTO>(
                    true,
                    "Consulta exitosa",
                    200,
                    new CredencialWalletRespDTO { CredencialValor = credencialValor });

                return StatusCode(200, response);
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<CredencialWalletRespDTO>(
                    false,
                    "Ocurrió un error interno al procesar la solicitud.",
                    500,
                    null));
            }
        }

        /// <summary>
        /// Obtiene el CredencialValor Wallet más reciente del usuario identificado por su ExternalId.
        /// </summary>
        [HttpGet("GetCredencialWalletByExternalIdWatch")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<CredencialWalletRespDTO>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCredencialWalletByExternalIdWatch([Required][FromQuery] Guid externalId)
        {
            try
            {
                var usuario = await _licenciaUserHIDService.GetByExternalId(externalId);
                if (usuario == null)
                {
                    return StatusCode(404, new ApiResponse<CredencialWalletRespDTO>(
                        false,
                        $"No se encontró un usuario con el ExternalId '{externalId}'.",
                        404,
                        null));
                }

                var tieneWallet = await _licenciaUserHIDService.TieneCredencialWallet(usuario.Id);
                if (!tieneWallet)
                {
                    return StatusCode(404, new ApiResponse<CredencialWalletRespDTO>(
                        false,
                        "El usuario no tiene una credencial de tipo Wallet asignada.",
                        404,
                        null));
                }

                var credencialValor = await _licenciaUserHIDService.GetCredencialWalletMasRecienteWatch(usuario.Id);
                if (credencialValor == null)
                {
                    return StatusCode(404, new ApiResponse<CredencialWalletRespDTO>(
                        false,
                        "El usuario no tiene registros en la tabla de credenciales.",
                        404,
                        null));
                }

                var response = new ApiResponse<CredencialWalletRespDTO>(
                    true,
                    "Consulta exitosa",
                    200,
                    new CredencialWalletRespDTO { CredencialValor = credencialValor });

                return StatusCode(200, response);
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<CredencialWalletRespDTO>(
                    false,
                    "Ocurrió un error interno al procesar la solicitud.",
                    500,
                    null));
            }
        }


        /// <summary>
        /// Obtiene el CredencialValor Origo más reciente del usuario identificado por su ExternalId.
        /// </summary>
        [HttpGet("GetCredencialOrigoByExternalId")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<CredencialOrigoRespDTO>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCredencialOrigoByExternalId([Required][FromQuery] Guid externalId)
        {
            try
            {
                var usuario = await _licenciaUserHIDService.GetByExternalId(externalId);
                if (usuario == null)
                {
                    return StatusCode(404, new ApiResponse<CredencialOrigoRespDTO>(
                        false,
                        $"No se encontró un usuario con el ExternalId '{externalId}'.",
                        404,
                        null));
                }

                var tieneOrigo = await _licenciaUserHIDService.TieneCredencialOrigo(usuario.Id);
                if (!tieneOrigo)
                {
                    return StatusCode(404, new ApiResponse<CredencialOrigoRespDTO>(
                        false,
                        "El usuario no tiene una credencial de tipo Origo asignada.",
                        404,
                        null));
                }

                var credencialValor = await _licenciaUserHIDService.GetCredencialOrigoMasReciente(usuario.Id);
                if (credencialValor == null)
                {
                    return StatusCode(404, new ApiResponse<CredencialOrigoRespDTO>(
                        false,
                        "El usuario no tiene registros en la tabla de credenciales.",
                        404,
                        null));
                }

                var response = new ApiResponse<CredencialOrigoRespDTO>(
                    true,
                    "Consulta exitosa",
                    200,
                    new CredencialOrigoRespDTO { CredencialValor = credencialValor });

                return StatusCode(200, response);
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<CredencialOrigoRespDTO>(
                    false,
                    "Ocurrió un error interno al procesar la solicitud.",
                    500,
                    null));
            }
        }

        [HttpPatch("ReactivateCredentialUser")]
        public async Task<IActionResult> ReactivateCredentialUser([Required] Guid id, [Required] Guid usuarioReactivadorId)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

            //Token token = _accesorService.GetTokenData();
            //if (token == null)
            //{
            //    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
            //}

            var userTipoCredencial = await _usuarioHidTipoCredencialService.GetUserHidTypeCredential(id);
            if (userTipoCredencial == null)
            {
                return StatusCode(400, new ApiResponse<UserHIDRespDTO>(false, "El registro no existe.", 400, null));
            }

            if (userTipoCredencial.TipoCredencialId == new Guid("2B3C4D5E-6F70-8901-BCDE-F12345678901"))
            {
                var result = await _licenciaUserHIDService.ReactivateWithWalletAndTask(userTipoCredencial.LicenciaHidUserId, usuarioReactivadorId, empresaId);
                if (result == true)
                {
                    await _usuarioHidTipoCredencialService.Reactivate(id, usuarioReactivadorId);
                }
                var response = result
                ? new ApiResponse<bool>(true, "El usuario ha sido reactivado correctamente.", 200, true)
                : new ApiResponse<bool>(false, "Ocurrió un error al intentar reactivar al usuario o la tarea asociada.", 400, false);

                return StatusCode(response.Codigo, response);
            }

            return BadRequest("El tipo de credencial no es válido.");
        }
    }
}
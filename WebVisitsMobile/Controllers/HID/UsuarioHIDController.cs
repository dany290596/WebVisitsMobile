using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.HID.UserHID;
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
                var data = await _licenciaUserHIDService.GetAll(filters);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
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

        /*
        [HttpPatch("Inactivate")]
        public async Task<IActionResult> Inactivate([Required] Guid id, [Required] Guid usuarioBajaId)
        {
            var result = await _licenciaUserHIDService.Inactivate(id, usuarioBajaId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible inactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se inactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPatch("Reactivate")]
        public async Task<IActionResult> Reactivate([Required] Guid id, [Required] Guid usuarioReactivadorId)
        {
            var result = await _licenciaUserHIDService.Reactivate(id, usuarioReactivadorId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible reactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se reactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }
        */

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
            var mapper = _mapper.Map<LicenciaHidUser>(data);
            mapper.Id = id;
            var result = await _licenciaUserHIDService.UpdatePartial(mapper);
            if (result == null)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));

            }
            var response = new ApiResponse<LicenciaHidUser>(true, "Se actualizó correctamente.", 200, result);

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


        [HttpPatch("ReactivateCredentialUser")]
        public async Task<IActionResult> ReactivateCredentialUser([Required] Guid id, [Required] Guid usuarioReactivadorId)
        {
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
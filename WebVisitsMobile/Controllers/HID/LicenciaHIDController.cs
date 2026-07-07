using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.HID.LicenciaHID;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.QueryFilters.HID;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.HID
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "HID Origo")]
    public class LicenciaHIDController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly ILicenciaHIDService _licenciaHIDService;
        public LicenciaHIDController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            ILicenciaHIDService licenciaHIDService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _licenciaHIDService = licenciaHIDService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<LicenciaHIDRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] LicenciaHIDQueryFilter filters)
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

                var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
                if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta", tipoError = 3 }); }

                var data = await _licenciaHIDService.GetAll(filters, empresaId);
                if (data == null || !data.Any())
                {
                    return StatusCode(200, new ApiResponse<IEnumerable<LicenciaHIDRespDTO>>(
                        true,
                        "No se encontraron registros que coincidan con tu búsqueda",
                        200,
                        Enumerable.Empty<LicenciaHIDRespDTO>()
                    ));
                }
                var dataDTO = _mapper.Map<IEnumerable<LicenciaHIDRespDTO>>(data);

                string strUriPreviousPage = _uriService.GetLicenseHIDPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetLicenseHIDPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<IEnumerable<LicenciaHIDRespDTO>>(true, "Consulta exitosa", 200, dataDTO);
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
                    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso", 401, null));
                }

                var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
                if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta", tipoError = 3 }); }

                var data = await _licenciaHIDService.GetById(id, empresaId);
                var dataDTO = _mapper.Map<LicenciaHIDRespDTO>(data);
                var response = new ApiResponse<LicenciaHIDRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPatch("Inactivate/{id}")]
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

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null)
            {
                return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta", tipoError = 3 });
            }

            var result = await _licenciaHIDService.Inactivate(id, token.UsuarioId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible inactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se inactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPatch("Reactivate/{id}")]
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

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta", tipoError = 3 }); }

            var result = await _licenciaHIDService.Reactivate(id, token.UsuarioId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible reactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se reactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LicenciaHIDReqDTO data)
        {
            var licenseHID = _mapper.Map<LicenciaHID>(data);
            bool insert = await _licenciaHIDService.Create(licenseHID, new Guid("739B4C8F-4DB1-4475-84D4-7644DCE00620"));
            if (insert)
            {
                LicenciaHIDRespDTO licenciaHIDDTO = _mapper.Map<LicenciaHIDRespDTO>(licenseHID);
                var response = new ApiResponse<LicenciaHIDRespDTO>(
                    true,
                    "La licencia HID fue creada exitosamente.",
                    200,
                    licenciaHIDDTO
                    );

                return StatusCode(200, response);
            }
            else
            {
                return StatusCode(500, new ApiResponse<string>(false, "Se produjo un error interno al procesar la solicitud.", 500, null));
            }
        }
    }
}
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
        public UsuarioHIDController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            ILicenciaUserHIDService licenciaUserHIDService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _licenciaUserHIDService = licenciaUserHIDService;
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
                var dataDTO = _mapper.Map<UserHIDRespDTO>(data);
                var response = new ApiResponse<UserHIDRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

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
        public async Task<IActionResult> Update(Guid id, UserHIDReqDTO data)
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
    }
}
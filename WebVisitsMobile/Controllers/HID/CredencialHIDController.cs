using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.HID.CredencialHID;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.QueryFilters.HID;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.HID
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "HID Origo")]
    public class CredencialHIDController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly ICredencialHIDService _credencialHIDService;
        public CredencialHIDController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            ICredencialHIDService credencialHIDService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _credencialHIDService = credencialHIDService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<CredencialHIDRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] CredencialHIDQueryFilter filters)
        {
            try
            {
                if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
                {
                    return BadRequest("El header de la empresa es inválido.");
                }
                var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
                if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

                var data = await _credencialHIDService.GetAll(filters, empresaId);
                var dataDTO = _mapper.Map<IEnumerable<CredencialHIDRespDTO>>(data);

                string strUriPreviousPage = _uriService.GetCredentialHIDPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetCredentialHIDPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<IEnumerable<CredencialHIDRespDTO>>(true, "Consulta exitosa", 200, dataDTO);
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

                var data = await _credencialHIDService.GetById(id, empresaId);
                var dataDTO = _mapper.Map<CredencialHIDRespDTO>(data);
                var response = new ApiResponse<CredencialHIDRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPatch("Inactivate/{id}")]
        public async Task<IActionResult> Inactivate(Guid id)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

            var result = await _credencialHIDService.Inactivate(id, empresaId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible inactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se inactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPatch("Reactivate/{id}")]
        public async Task<IActionResult> Reactivate(Guid id)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

            var result = await _credencialHIDService.Reactivate(id, empresaId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible reactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se reactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPatch("Suspend/{id}")]
        public async Task<IActionResult> Suspend(Guid id)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

            var result = await _credencialHIDService.Suspend(id, empresaId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible suspender el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se suspendio correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpPost("CreateWalletCredential")]
        public async Task<IActionResult> CreateForWallet(CredencialWalletReqDTO data)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

            var mapper = _mapper.Map<CredencialHid>(data);

            bool book = await _credencialHIDService.CreateForWallet(mapper, empresaId);
            if (!book)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));
            }

            CredencialHIDRespDTO dto = _mapper.Map<CredencialHIDRespDTO>(mapper);

            var response = new ApiResponse<CredencialHIDRespDTO>(book, "El registro se creó correctamente.", 200, dto);

            return StatusCode(200, response);
        }
    }
}
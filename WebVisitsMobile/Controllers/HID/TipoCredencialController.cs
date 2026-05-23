using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.HID.TipoCredencial;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.QueryFilters.HID;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.HID
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "HID Origo")]
    public class TipoCredencialController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly ITipoCredencialService _tipoCredencialService;
        public TipoCredencialController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            ITipoCredencialService tipoCredencialService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _tipoCredencialService = tipoCredencialService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TipoCredencialRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] TipoCredencialQueryFilter filters)
        {
            try
            {
                var data = await _tipoCredencialService.GetAll(filters);
                var dataDTO = _mapper.Map<IEnumerable<TipoCredencialRespDTO>>(data);

                string strUriPreviousPage = _uriService.GetTypeCredentialUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetTypeCredentialUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<IEnumerable<TipoCredencialRespDTO>>(true, "Consulta exitosa", 200, dataDTO);
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
                var data = await _tipoCredencialService.GetById(id);
                var dataDTO = _mapper.Map<TipoCredencialRespDTO>(data);
                var response = new ApiResponse<TipoCredencialRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

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
            var result = await _tipoCredencialService.Inactivate(id, usuarioBajaId);
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
            var result = await _tipoCredencialService.Reactivate(id, usuarioReactivadorId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible reactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se reactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }
    }
}
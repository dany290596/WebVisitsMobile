using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.Organizacion.Tarea.Tarea;
using WebVisitsMobile.Services.Interfaces.Organizacion.Tarea;
using WebVisitsMobile.Services.QueryFilters.Common;
using WebVisitsMobile.Services.QueryFilters.Organizacion.Tarea;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Organizacion.Tarea
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Organización")]
    public class TareaController : ControllerBase
    {
        private readonly ITareaService _tareaService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        public TareaController(
            ITareaService tareaService,
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService
            )
        {
            _tareaService = tareaService;
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
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
                    return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
                }

                var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
                if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

                var perfil = await _tareaService.GetById(id);
                var perfilDto = _mapper.Map<TareaRespDTO>(perfil);
                var response = new ApiResponse<TareaRespDTO>(true, "Consulta ejecutada.", 200, perfilDto);

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
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
            }

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null)
            {
                return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 });
            }

            var result = await _tareaService.Inactivate(id, token.UsuarioId);
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
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
            }

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

            var result = await _tareaService.Reactivate(id, token.UsuarioId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible reactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se reactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TareaRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] TareaQueryFilter filters)
        {
            try
            {
                var data = await _tareaService.GetAll(filters);
                var dataDTO = _mapper.Map<IEnumerable<TareaRespDTO>>(data);

                string strUriPreviousPage = _uriService.GetTaskPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetTaskPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<IEnumerable<TareaRespDTO>>(true, "Consulta exitosa.", 200, dataDTO);
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPut("UpdateTaskPending")]
        public async Task<IActionResult> UpdatePending([Required] Guid id, TareaPendingDTO data)
        {
            var result = await _tareaService.UpdatePending(id, data);
            if (result)
            {
                var response = new ApiResponse<bool>(result, $"La tarea {id.ToString()} se actualizó correctamente.", 200, result);
                return StatusCode(200, response);
            }

            var errorMessage = id == Guid.Empty
        ? "ID de tarea inválido. No se pudo actualizar la tarea."
        : $"No se pudo actualizar la tarea {id.ToString()}. Ocurrió un error inesperado.";

            return StatusCode(500, new ApiResponse<bool>(false, errorMessage, 500, false));
        }

        [HttpPost]
        public async Task<IActionResult> Create(TareaReqDTO data)
        {
            var mapper = _mapper.Map<Domain.Entities.Organizacion.Tarea.Tarea>(data);

            var book = await _tareaService.Create(mapper, data.UsuarioCreadorId);
            if (book == null)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));
            }

            TareaRespDTO dto = _mapper.Map<TareaRespDTO>(mapper);

            var response = new ApiResponse<TareaRespDTO>(true, "El registro se creó correctamente.", 200, dto);

            return StatusCode(200, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, TareaReqDTO data)
        {
            var mapper = _mapper.Map<Domain.Entities.Organizacion.Tarea.Tarea>(data);
            mapper.Id = id;
            var result = await _tareaService.Update(mapper, data.UsuarioCreadorId);
            if (!result)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));

            }
            var response = new ApiResponse<bool>(true, "Se actualizó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        [HttpGet("GetAllByUserWallet")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TareaRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllByUserWallet([FromQuery] BaseQueryFilter filters)
        {
            try
            {
                var data = await _tareaService.GetAllByUserWallet<TareaUsuarioHIDWallet>(filters, new Guid("FD82D317-F02C-4A26-86F4-23766E029BC0"));

                string strUriPreviousPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<List<TareaHID<TareaUsuarioHIDWallet>>>(true, "Consulta exitosa.", 200, data.ToList());
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetAllByUserWalletInactivate")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TareaRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllByUserWalletInactivate([FromQuery] BaseQueryFilter filters)
        {
            try
            {
                var data = await _tareaService.GetAllByUserWallet<TareaWalletInactivate>(filters, new Guid("8BB6A16A-E148-4952-97A7-76106F048E5D"));

                string strUriPreviousPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<List<TareaHID<TareaWalletInactivate>>>(true, "Consulta exitosa.", 200, data.ToList());
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetAllByUserWalletReactivate")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TareaRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllByUserWalletReactivate([FromQuery] BaseQueryFilter filters)
        {
            try
            {
                var data = await _tareaService.GetAllByUserWallet<TareaWalletReactivate>(filters, new Guid("1116C4E2-8B24-4EBD-A938-4123291956F5"));

                string strUriPreviousPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<List<TareaHID<TareaWalletReactivate>>>(true, "Consulta exitosa.", 200, data.ToList());
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetAllByCredentialInactivate")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TareaRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllByCredentialInactivate([FromQuery] BaseQueryFilter filters)
        {
            try
            {
                var data = await _tareaService.GetAllByUserWallet<TareaCommonRespDTO>(filters, new Guid("CCE218E1-99E1-4D96-AD85-4F7D71A57F4B"));

                string strUriPreviousPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<List<TareaHID<TareaCommonRespDTO>>>(true, "Consulta exitosa.", 200, data.ToList());
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetAllByCredentialReactivate")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TareaRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllByCredentialReactivate([FromQuery] BaseQueryFilter filters)
        {
            try
            {
                var data = await _tareaService.GetAllByUserWallet<TareaCommonRespDTO>(filters, new Guid("99EFE58C-5A7D-424A-AD03-54A9F5D6EA94"));

                string strUriPreviousPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<List<TareaHID<TareaCommonRespDTO>>>(true, "Consulta exitosa.", 200, data.ToList());
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetAllByCredentialSuspend")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TareaRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllByCredentialSuspend([FromQuery] BaseQueryFilter filters)
        {
            try
            {
                var data = await _tareaService.GetAllByUserWallet<TareaCommonRespDTO>(filters, new Guid("7006FE62-BE92-4976-B83C-45DD276CCDF0"));

                string strUriPreviousPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<List<TareaHID<TareaCommonRespDTO>>>(true, "Consulta exitosa.", 200, data.ToList());
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetAllByTemplate")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TareaRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllByTemplate([FromQuery] BaseQueryFilter filters)
        {
            try
            {
                var data = await _tareaService.GetAllByTemplate(filters);

                string strUriPreviousPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetCommonPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<List<TareaHID<TareaPlantilla>>>(true, "Consulta exitosa.", 200, data.ToList());
                response.CargarMetaData(data.TotalCount, data.PageSize, data.CurrentPage, data.TotalPages,
                                        data.HasNextPage, data.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
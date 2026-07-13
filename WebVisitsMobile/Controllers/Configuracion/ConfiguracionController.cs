using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.Configuracion.Configuraciones;
using WebVisitsMobile.Models.Configuracion.CorreoEmpresa;
using WebVisitsMobile.Models.HID.TipoCredencial;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.Interfaces.Empresa;
using WebVisitsMobile.Services.QueryFilters.Configuracion;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Configuracion
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Configuración")]
    public class ConfiguracionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly IConfiguracionService _configuracionService;
        private readonly IEmpresaClienteService _empresaClienteService;

        public ConfiguracionController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            IConfiguracionService configuracionService,
            IEmpresaClienteService empresaClienteService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _configuracionService = configuracionService;
            _empresaClienteService = empresaClienteService;
        }

        [HttpGet("Company/{companyId}")]
        public async Task<IActionResult> GetSettingByClientCompanyId(Guid companyId)
        {
            var existClientCompany = await _empresaClienteService.GetById(companyId);
            if (existClientCompany == null)
            {
                var responseNotFound = new ApiResponse<Dictionary<Guid, ConfigSettingDTO>>(
                    false,
                    "No se encontró una empresa registrada con el identificador proporcionado.",
                    404,
                    null
                );
                return StatusCode(404, responseNotFound);
            }

            var setting = await _configuracionService.GetFullSetting(companyId);
            if (setting == null || !setting.Any())
            {
                var emptyResponse = new ApiResponse<Dictionary<Guid, ConfigSettingDTO>>(
                    false,
                    "No se encontraron configuraciones asociadas a la empresa especificada.",
                    404,
                    null
                );
                return StatusCode(404, emptyResponse);
            }

            var response = new ApiResponse<Dictionary<Guid, ConfigSettingDTO>>(
                true,
                "La configuración fue obtenida correctamente.",
                200,
                setting
            );

            return StatusCode(200, response);
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

                var data = await _configuracionService.GetById(id, empresaId);
                var dataDTO = _mapper.Map<ConfiguracionesRespDTO>(data);
                var response = new ApiResponse<ConfiguracionesRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ConfiguracionesReqDTO data)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

            var mapper = _mapper.Map<Configuraciones>(data);

            bool book = await _configuracionService.Create(mapper, empresaId);
            if (!book)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));
            }

            ConfiguracionesRespDTO dto = _mapper.Map<ConfiguracionesRespDTO>(mapper);

            var response = new ApiResponse<ConfiguracionesRespDTO>(book, "El registro se creó correctamente.", 200, dto);

            return StatusCode(200, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, ConfiguracionesReqDTO data)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

            var mapper = _mapper.Map<Configuraciones>(data);
            mapper.Id = id;
            var result = await _configuracionService.Update(mapper, data.UsuarioCreadorId, empresaId);
            if (!result)
            {
                return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));

            }
            var response = new ApiResponse<bool>(true, "Se actualizó correctamente.", 200, result);

            return StatusCode(200, response);
        }


        [HttpGet("GroupByCompany", Name = "GetGroupByCompany")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<SettingsGroup>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetGroupByCompany([FromQuery] SettingsGroupEncryptedQueryFilter filters)
        {
            try
            {
                string strUriPreviousPage = _uriService.GetCompanyEncryptedUri(filters, Url.RouteUrl(nameof(GetGroupByCompanyEncrypted))).ToString();
                string strUriNextPage = _uriService.GetCompanyEncryptedUri(filters, Url.RouteUrl(nameof(GetGroupByCompanyEncrypted))).ToString();

                var setting = await _configuracionService.GetGroupByCompany(filters);
                if (setting == null)
                {
                    return StatusCode(503, new ApiResponse<string>(
                        false,
                        "No fue posible obtener la información solicitada desde la base de datos. El resultado obtenido fue nulo.",
                        503,
                        null
                    ));
                }
                if (!setting.Any())
                {
                    var emptyResponse = new ApiResponse<List<SettingsGroup>>(
                                true,
                                 "No se encontraron registros que coincidan con los criterios especificados.",
                                200,
                                new List<SettingsGroup>());
                    return StatusCode(200, emptyResponse);
                }

                var response = new ApiResponse<List<SettingsGroup>>(true, "La operación se completó exitosamente.", 200, setting);
                response.CargarMetaData(setting.TotalCount, setting.PageSize, setting.CurrentPage, setting.TotalPages,
                                        setting.HasNextPage, setting.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, "Se produjo un error interno al procesar la solicitud.", 500, null));
            }
        }

        [HttpGet("GroupByCompanyEncrypted", Name = "GetGroupByCompanyEncrypted")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<SettingsGroupEncrypted>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetGroupByCompanyEncrypted([FromQuery] SettingsGroupEncryptedQueryFilter filters)
        {
            try
            {
                string strUriPreviousPage = _uriService.GetCompanyEncryptedUri(filters, Url.RouteUrl(nameof(GetGroupByCompanyEncrypted))).ToString();
                string strUriNextPage = _uriService.GetCompanyEncryptedUri(filters, Url.RouteUrl(nameof(GetGroupByCompanyEncrypted))).ToString();

                var setting = await _configuracionService.GetGroupByCompanyEncrypted(filters);
                if (setting == null)
                {
                    return StatusCode(503, new ApiResponse<List<SettingsGroupEncrypted>>(
                        false,
                        "No fue posible obtener la información solicitada desde la base de datos. El resultado obtenido fue nulo.",
                        503,
                        new List<SettingsGroupEncrypted>()
                    ));
                }
                if (!setting.Any())
                {
                    var emptyResponse = new ApiResponse<List<SettingsGroupEncrypted>>(
                                true,
                                 "No se encontraron registros que coincidan con los criterios especificados.",
                                200,
                                new List<SettingsGroupEncrypted>());
                    return StatusCode(200, emptyResponse);
                }

                var response = new ApiResponse<List<SettingsGroupEncrypted>>(true, "La operación se completó exitosamente.", 200, setting);
                response.CargarMetaData(setting.TotalCount, setting.PageSize, setting.CurrentPage, setting.TotalPages,
                                        setting.HasNextPage, setting.HasPreviousPage, strUriNextPage, strUriPreviousPage);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, "Se produjo un error interno al procesar la solicitud.", 500, null));
            }
        }

        [HttpGet("AccountEmail", Name = "GetSettingOfAccountEmail")]
        public async Task<IActionResult> GetSettingOfAccountEmail()
        {
            try
            {
                var setting = await _configuracionService.GetSettingOfAccountEmail();
                var response = new ApiResponse<SettingAccountEmail>(true, "La operación se completó exitosamente.", 200, setting);
                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Se produjo un error interno al procesar la solicitud.");
            }
        }

        [HttpGet("SettingsGrouped")]
        public async Task<IActionResult> GetConfigurationTemplates()
        {
            var result = await _configuracionService.GetConfigurationTemplates();
            if (result == null || !result.Any())
                return StatusCode(404, new ApiResponse<Configuraciones>(false, "No se encontraron configuraciones.", 404, null));

            return StatusCode(200, new ApiResponse<List<Configuraciones>>(true, "Configuración obtenida correctamente.", 200, result));
        }

        [HttpGet("SettingsGroupedType")]
        public async Task<IActionResult> GetSettingsGroupedByType()
        {
            var result = await _configuracionService.GetSettingsGroupedByType();
            if (result == null || !result.Any())
                return StatusCode(404, new ApiResponse<SettingsGroupTap>(false, "No se encontraron configuraciones.", 404, null));

            return StatusCode(200, new ApiResponse<List<SettingsGroupTap>>(true, "Configuración obtenida correctamente.", 200, result));
        }

        [HttpGet("SettingsHIDGrouped")]
        public async Task<IActionResult> GetSettingsHIDGrouped()
        {
            var result = await _configuracionService.GetSettingsForHID();
            if (result == null || !result.Any())
                return StatusCode(404, new ApiResponse<SettingsGroupTap>(false, "No se encontraron configuraciones.", 404, null));

            return StatusCode(200, new ApiResponse<List<SettingsGroupTap>>(true, "Configuración obtenida correctamente.", 200, result));
        }

        [HttpGet("SettingsWalletGrouped")]
        public async Task<IActionResult> GetSettingsWalletGrouped()
        {
            var result = await _configuracionService.GetSettingsForWallet();
            if (result == null || !result.Any())
                return StatusCode(404, new ApiResponse<SettingsGroupTap>(false, "No se encontraron configuraciones.", 404, null));

            return StatusCode(200, new ApiResponse<List<SettingsGroupTap>>(true, "Configuración obtenida correctamente.", 200, result));
        }

        [HttpPost("Correo")]
        public async Task<IActionResult> CreateCorreoEmpresa(CorreoEmpresaReqDTO data)
        {
            if (data == null)
            {
                return BadRequest(new ApiResponse<string>(false, "El cuerpo de la petición es requerido.", 400, null));
            }

            var empresaExiste = await _plataformaService.ExistsCompany(data.EmpresaId);
            if (empresaExiste == null)
            {
                return StatusCode(404, new ApiResponse<string>(false, $"La empresa con el ID {data.EmpresaId} no existe.", 404, null));
            }

            Token token = _accesorService.GetTokenData();
            var currentUserId = token?.UsuarioId ?? Guid.Empty;

            var result = await _configuracionService.CreateCorreoEmpresa(data, currentUserId);
            if (!result.Success)
            {
                return StatusCode(409, new ApiResponse<string>(false, result.ErrorMessage, 409, null));
            }

            return StatusCode(200, new ApiResponse<CorreoEmpresaRespDTO>(true, "La configuración de correo se creó correctamente.", 200, result.Value));
        }

        [HttpGet("Correo")]
        public async Task<IActionResult> GetAllCorreoEmpresa()
        {
            var result = await _configuracionService.GetAllCorreoEmpresa();
            if (result == null || !result.Any())
            {
                return StatusCode(200, new ApiResponse<List<CorreoEmpresaRespDTO>>(true, "No se encontraron configuraciones de correo registradas.", 200, new List<CorreoEmpresaRespDTO>()));
            }

            return StatusCode(200, new ApiResponse<List<CorreoEmpresaRespDTO>>(true, "La operación se completó exitosamente.", 200, result));
        }

        [HttpGet("Correo/{empresaId}")]
        public async Task<IActionResult> GetCorreoEmpresa(Guid empresaId)
        {
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null)
            {
                return StatusCode(404, new ApiResponse<string>(false, $"La empresa con el ID {empresaId} no existe.", 404, null));
            }

            var result = await _configuracionService.GetCorreoEmpresa(empresaId);
            if (!result.Success)
            {
                return StatusCode(404, new ApiResponse<string>(false, result.ErrorMessage, 404, null));
            }

            return StatusCode(200, new ApiResponse<CorreoEmpresaRespDTO>(true, "La configuración fue obtenida correctamente.", 200, result.Value));
        }

        [HttpPut("Correo")]
        public async Task<IActionResult> UpdateCorreoEmpresa(CorreoEmpresaUpdateReqDTO data)
        {
            if (data == null)
            {
                return BadRequest(new ApiResponse<string>(false, "El cuerpo de la petición es requerido.", 400, null));
            }

            var empresaExiste = await _plataformaService.ExistsCompany(data.EmpresaId);
            if (empresaExiste == null)
            {
                return StatusCode(404, new ApiResponse<string>(false, $"La empresa con el ID {data.EmpresaId} no existe.", 404, null));
            }

            Token token = _accesorService.GetTokenData();
            var currentUserId = token?.UsuarioId ?? Guid.Empty;

            var result = await _configuracionService.UpdateCorreoEmpresa(data, currentUserId);
            if (!result.Success)
            {
                return StatusCode(400, new ApiResponse<string>(false, result.ErrorMessage, 400, null));
            }

            return StatusCode(200, new ApiResponse<CorreoEmpresaRespDTO>(true, "La configuración se actualizó correctamente.", 200, result.Value));
        }

        [HttpGet("SettingsDecrypt")]
        public async Task<IActionResult> GetSettingsDecrypt(Guid companyId, Guid typeSettingId)
        {
            var result = await _configuracionService.GetSettingsDecrypt(companyId, typeSettingId);
            if (result == null || !result.Any())
                return StatusCode(404, new ApiResponse<List<SettingsGroupTap>>(false, "No se encontraron configuraciones.", 404, new List<SettingsGroupTap>()));

            return StatusCode(200, new ApiResponse<List<SettingsGroupTap>>(true, "Configuración obtenida correctamente.", 200, result));
        }
    }
}
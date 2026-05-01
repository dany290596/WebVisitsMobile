using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Models.Configuracion.Configuraciones;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.Interfaces.Empresa;
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

        [HttpGet("company/{companyId}")]
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
    }
}
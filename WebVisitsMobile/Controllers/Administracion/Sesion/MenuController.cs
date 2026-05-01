using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebVisitsMobile.Domain.Entities.Administracion.Modulo;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Services.Interfaces.Administracion.Seccion;
using WebVisitsMobile.Services.QueryFilters.Administracion.Sesion;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Administracion.Sesion
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Autenticación")]
    public class MenuController : ControllerBase
    {
        private readonly ISeccionService _seccionService;
        private readonly IPlataformaService _plataformaService;
        private readonly IMapper _mapper;

        public MenuController(
            ISeccionService seccionService,
            IPlataformaService plataformaService,
            IMapper mapper
            )
        {
            _seccionService = seccionService;
            _plataformaService = plataformaService;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetMenuGroupedByModule))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<SeccionesPorModulo>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetMenuGroupedByModule([FromQuery] MenuQueryFilter filters)
        {
            //var empresaId = new Guid(Request.Headers["Empresa"]);
            //if (empresaId == Guid.Empty) return BadRequest("Falta el header Empresa");

            //var empresaExiste = await _plataformaService.EmpresaValidar(empresaId);
            //if (empresaExiste == false) { return BadRequest($"La empresa con el ID {empresaId} no existe"); }

            if (filters.PerfilId == Guid.Empty)
                return BadRequest("El parámetro perfilId es obligatorio.");

            var seccion = await _seccionService.GetSectionsGroupedByModule(filters);
            var response = new ApiResponse<IEnumerable<SeccionesPorModulo>>(true, "Consulta ejecutada", 200, seccion);

            return StatusCode(200, response);
        }
    }
}
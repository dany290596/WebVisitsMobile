using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Services.Interfaces.Empresa;

namespace WebVisitsMobile.Controllers.Empresa
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Cliente")]
    public class EmpresaClienteController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly IEmpresaClienteService _empresaClienteService;
        public EmpresaClienteController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            IEmpresaClienteService empresaClienteService
            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _empresaClienteService = empresaClienteService;
        }
    }
}
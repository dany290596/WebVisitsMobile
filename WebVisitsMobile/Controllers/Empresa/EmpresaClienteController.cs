using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Infrastructure.Services;
using WebVisitsMobile.Models.Empresa.EmpresaCliente;
using WebVisitsMobile.Services.Interfaces.Empresa;
using WebVisitsMobile.Services.Interfaces.Organizacion.Tarea;
using WebVisitsMobile.Services.QueryFilters.Empresa;
using WebVisitsMobile.Services.Responses;

namespace WebVisitsMobile.Controllers.Empresa
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Cliente")]
    public class EmpresaClienteController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IAccesorService _accesorService;
        private readonly IPlataformaService _plataformaService;
        private readonly IEmpresaClienteService _empresaClienteService;
        private readonly ITareaService _tareaService;
        private readonly ITipoTareaService _tipoTareaService;
        private readonly TaskEventService _taskEventService;
        private readonly IServiceProvider _serviceProvider;

        public EmpresaClienteController(
            IMapper mapper,
            IUriService uriService,
            IAccesorService accesorService,
            IPlataformaService plataformaService,
            IEmpresaClienteService empresaClienteService,
            ITareaService tareaService,
            ITipoTareaService tipoTareaService,
            TaskEventService taskEventService,
            IServiceProvider serviceProvider

            )
        {
            _mapper = mapper;
            _uriService = uriService;
            _accesorService = accesorService;
            _plataformaService = plataformaService;
            _empresaClienteService = empresaClienteService;
            _tareaService = tareaService;
            _tipoTareaService = tipoTareaService;
            _taskEventService = taskEventService;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<EmpresaClienteRespDTO>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] EmpresaClienteQueryFilter filters)
        {
            try
            {
                var data = await _empresaClienteService.GetAll(filters);
                var dataDTO = _mapper.Map<IEnumerable<EmpresaClienteRespDTO>>(data);

                string strUriPreviousPage = _uriService.GetClientCompanyPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();
                string strUriNextPage = _uriService.GetClientCompanyPaginationUri(filters, Url.RouteUrl(nameof(GetAll))).ToString();

                var response = new ApiResponse<IEnumerable<EmpresaClienteRespDTO>>(true, "Consulta exitosa", 200, dataDTO);
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

                var data = await _empresaClienteService.GetById(id);
                var dataDTO = _mapper.Map<EmpresaClienteRespDTO>(data);
                var response = new ApiResponse<EmpresaClienteRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("Complete/{id}")]
        public async Task<IActionResult> GetCompanyClient(Guid id)
        {
            try
            {
                if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
                {
                    return BadRequest("El header de la empresa es inválido.");
                }
                var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
                if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

                var data = await _empresaClienteService.GetCompanyClient(id);
                var dataDTO = _mapper.Map<EmpresaClienteRespDTO>(data);
                var response = new ApiResponse<EmpresaClienteRespDTO>(true, "Consulta ejecutada", 200, dataDTO);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("CredentialConfiguration/{id}")]
        public async Task<IActionResult> GetCredentialConfiguration(Guid id)
        {
            try
            {
                if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
                {
                    return BadRequest("El header de la empresa es inválido.");
                }
                var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
                if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

                var data = await _empresaClienteService.GetWithSetting(id);
                var response = new ApiResponse<CompanyClientWithSetting>(true, "Consulta ejecutada", 200, data);

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
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

            var result = await _empresaClienteService.Inactivate(id, usuarioBajaId);
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
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
            {
                return BadRequest("El header de la empresa es inválido.");
            }
            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

            var result = await _empresaClienteService.Reactivate(id, usuarioReactivadorId);
            if (!result)
            {
                return StatusCode(400, new ApiResponse<bool>(false, "No fue posible reactivar el registro.", 400, false));
            }
            var response = new ApiResponse<bool>(true, "El registro se reactivó correctamente.", 200, result);

            return StatusCode(200, response);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(EmpresaClienteReqDTO data)
        //{
        //    if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
        //    {
        //        return BadRequest("El header de la empresa es inválido.");
        //    }
        //    var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
        //    if (empresaExiste == null) { return BadRequest($"La empresa con el ID {empresaId} no existe."); }

        //    Token token = _accesorService.GetTokenData();
        //    if (token == null)
        //    {
        //        return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));
        //    }

        //    var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
        //    if (validarSesion == null) { return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesion activa con tu cuenta.", tipoError = 3 }); }

        //    var mapper = _mapper.Map<EmpresaCliente>(data);

        //    bool book = await _empresaClienteService.CreateWithHID(mapper, [], token.UsuarioId);
        //    if (!book)
        //    {
        //        return StatusCode(500, new ApiResponse<string>(false, "ocurrió un error.", 500, null));
        //    }

        //    EmpresaClienteRespDTO dto = _mapper.Map<EmpresaClienteRespDTO>(mapper);

        //    var response = new ApiResponse<EmpresaClienteRespDTO>(book, "El registro se creó correctamente.", 200, dto);

        //    return StatusCode(200, response);
        //}

        [HttpPost("TestConnection")]
        public async Task<IActionResult> TestConnection([FromBody] TestConnectionDTO data)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
                return BadRequest("El header de la empresa es inválido.");

            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null)
                return BadRequest($"La empresa con el ID {empresaId} no existe.");

            Token token = _accesorService.GetTokenData();
            if (token == null)
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null)
                return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesión activa con tu cuenta.", tipoError = 3 });

            try
            {
                // Verificar tipo de tarea (ID fijo como en el antiguo)
                var tipoTareaId = new Guid("617950AD-6DAE-4FE3-B31F-5D18D6315645");
                var tipoTarea = await _tipoTareaService.GetById(tipoTareaId);
                if (tipoTarea == null)
                    return BadRequest(new ApiResponse<bool>(false, "El tipo de tarea no existe.", 400, false));

                // Serializar datos de prueba
                var jsonOptions = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                var nuevaTarea = new Domain.Entities.Organizacion.Tarea.Tarea
                {
                    TipoTareaId = tipoTareaId,
                    Fecha = DateTime.Now,
                    Pendiente = 1,
                    Status = 1,
                    ValorEnvio = JsonSerializer.Serialize(data, jsonOptions),
                    ValorRetorno = "",
                    EmpresaClienteId = empresaId   // Asignamos la empresa del header
                };

                var tareaCreada = await _tareaService.Create(nuevaTarea, token.UsuarioId);
                if (tareaCreada == null)
                    return StatusCode(500, new ApiResponse<bool>(false, "No se pudo crear la tarea.", 500, false));

                // Registrar en el sistema SSE para que el frontend pueda suscribirse
                _taskEventService.RegisterTask(tareaCreada.Id);

                // Iniciar monitoreo en segundo plano (sin await)
                _ = MonitorTaskInBackground(tareaCreada.Id);

                var response = new ApiResponse<Domain.Entities.Organizacion.Tarea.Tarea>(
                    true,
                    "Tarea creada correctamente. Esperando resultado de la conexión HID.",
                    200,
                    tareaCreada
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, "Error interno al crear la tarea.", 500, null));
            }
        }

        /// <summary>
        /// Endpoint SSE para recibir actualizaciones de la tarea.
        /// </summary>
        [HttpGet("TaskUpdates")]
        [AllowAnonymous]
        public async Task TaskUpdates(Guid id)
        {
            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");
            Response.Headers.Add("X-Accel-Buffering", "no");

            try
            {
                if (!_taskEventService.TaskExists(id))
                {
                    await Response.WriteAsync($"event: error\ndata: {{\"message\":\"Tarea no encontrada\"}}\n\n");
                    await Response.Body.FlushAsync();
                    return;
                }

                await foreach (var message in _taskEventService.GetTaskStream(id, HttpContext.RequestAborted))
                {
                    if (HttpContext.RequestAborted.IsCancellationRequested)
                        break;

                    await Response.WriteAsync(message);
                    await Response.Body.FlushAsync();
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception) { }
        }

        /// <summary>
        /// Monitorea una tarea en segundo plano, actualizando el progreso y notificando al finalizar.
        /// </summary>
        private async Task MonitorTaskInBackground(Guid taskId)
        {
            using var scope = _serviceProvider.CreateScope();
            var tareaService = scope.ServiceProvider.GetRequiredService<ITareaService>();
            var taskEventService = scope.ServiceProvider.GetRequiredService<TaskEventService>();

            try
            {
                var maxWait = TimeSpan.FromMinutes(2);
                var start = DateTime.UtcNow;
                var interval = TimeSpan.FromSeconds(2);

                while (DateTime.UtcNow - start < maxWait)
                {
                    await Task.Delay(interval);

                    var tarea = await tareaService.GetById(taskId);
                    if (tarea == null)
                    {
                        await taskEventService.NotifyErrorAsync(taskId, "La tarea no existe.");
                        break;
                    }

                    int progress = Math.Min(90, (int)((DateTime.UtcNow - start).TotalSeconds / maxWait.TotalSeconds * 100));
                    await taskEventService.NotifyProgressAsync(taskId, progress, "Procesando con servicio externo...");

                    if (!string.IsNullOrEmpty(tarea.ValorRetorno))
                    {
                        // Intentar deserializar el resultado
                        if (JsonHelper.TryDeserialize<TestConnectionRespDTO>(tarea.ValorRetorno, out var result))
                        {
                            var payload = new
                            {
                                taskId = tarea.Id,
                                result = result,
                                message = result.Mensaje,
                                code = result.Codigo,
                                completedAt = DateTime.UtcNow
                            };

                            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                            {
                                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                            });

                            await taskEventService.NotifyCompletionAsync(taskId, json);
                            return;
                        }

                        await taskEventService.NotifyErrorAsync(taskId, "Formato de resultado no válido.");
                        return;
                    }
                }

                await taskEventService.NotifyErrorAsync(taskId, "Tiempo de espera agotado.");
            }
            catch (Exception ex)
            {
                // Obtener el servicio desde el scope interno para notificar error
                var _taskEventService = scope.ServiceProvider.GetRequiredService<TaskEventService>();
                await _taskEventService.NotifyErrorAsync(taskId, $"Error interno: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EmpresaClienteConfiguracionReqDTO model)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
                return BadRequest("El header de la empresa es inválido.");

            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null)
                return BadRequest($"La empresa con el ID {empresaId} no existe.");

            Token token = _accesorService.GetTokenData();
            if (token == null)
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));

            var validarSesion = await _plataformaService.SessionValidate(token.SesionId);
            if (validarSesion == null)
                return Unauthorized(new { Ok = false, Code = 401, msg = "Ya existe una sesión activa con tu cuenta.", tipoError = 3 });

            try
            {
                var empresa = _mapper.Map<EmpresaCliente>(model.Empresa);
                empresa.Id = id;

                bool resultado = await _empresaClienteService.UpdateWithHID(empresa, model.Configuraciones, token.UsuarioId);
                if (!resultado)
                    return StatusCode(500, new ApiResponse<bool>(false, "No se pudo actualizar el registro.", 500, false));

                var response = new ApiResponse<bool>(true, "La empresa se actualizó correctamente.", 200, true);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, "Error interno del servidor.", 500, null));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmpresaClienteConfiguracionReqDTO model)
        {
            if (!Guid.TryParse(Request.Headers["Empresa"], out var empresaId))
                return BadRequest("El header de la empresa es inválido.");

            var empresaExiste = await _plataformaService.ExistsCompany(empresaId);
            if (empresaExiste == null)
                return BadRequest($"La empresa con el ID {empresaId} no existe.");

            Token token = _accesorService.GetTokenData();
            if (token == null)
                return Unauthorized(new ApiResponse<string>(false, "No tiene permiso sobre este recurso.", 401, null));

            var empresa = model.Empresa;
            var configuraciones = model.Configuraciones;

            try
            {
                // ----- Validación de RFC y Razón Social duplicados -----
                var rfcExistente = await _empresaClienteService.GetByRFC(empresa.RFC);
                if (rfcExistente != null)
                {
                    return Ok(new ApiResponse<string>(
                        false,
                        "Ya existe una empresa registrada con el mismo RFC.",
                        200,
                        null
                    ));
                }

                var razonSocialExistente = await _empresaClienteService.GetByRazonSocial(empresa.RazonSocial);
                if (razonSocialExistente != null)
                {
                    return Ok(new ApiResponse<string>(
                        false,
                        "Ya existe una empresa registrada con la misma razón social.",
                        200,
                        null
                    ));
                }

                // ----- Validación de configuraciones HID -----
                if (empresa.UsaCredencialesHID == 1)
                {
                    if (configuraciones == null || !configuraciones.Any())
                    {
                        return BadRequest(new ApiResponse<string>(
                            false,
                            "Las configuraciones HID son obligatorias cuando la empresa utiliza credenciales HID.",
                            400,
                            null
                        ));
                    }
                }

                var clientCompany = _mapper.Map<EmpresaCliente>(empresa);

                bool resultado = await _empresaClienteService.CreateWithHID(clientCompany, configuraciones, token.UsuarioId);
                if (!resultado)
                {
                    return StatusCode(500, new ApiResponse<bool>(false, "No se pudo crear el registro.", 500, false));
                }

                EmpresaClienteRespDTO empresaRespDTO = _mapper.Map<EmpresaClienteRespDTO>(clientCompany);
                var response = new ApiResponse<EmpresaClienteRespDTO>(true, "La empresa se registró correctamente.", 200, empresaRespDTO);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, "Error interno del servidor.", 500, null));
            }
        }

        public static class JsonHelper
        {
            public static bool TryDeserialize<T>(string json, out T? result)
            {
                result = default;

                if (string.IsNullOrWhiteSpace(json))
                    return false;

                try
                {
                    result = JsonSerializer.Deserialize<T>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    return result != null;
                }
                catch (JsonException)
                {
                    return false;
                }
            }
        }
    }
}
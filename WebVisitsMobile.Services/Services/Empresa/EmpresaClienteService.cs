using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;
using WebVisitsMobile.Domain.Entities.Parametrizacion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Models.Configuracion.Configuraciones;
using WebVisitsMobile.Models.Empresa.EmpresaCliente;
using WebVisitsMobile.Services.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.Interfaces.Empresa;
using WebVisitsMobile.Services.Interfaces.Encriptacion;
using WebVisitsMobile.Services.Interfaces.Parametrizacion;
using WebVisitsMobile.Services.QueryFilters.Empresa;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebVisitsMobile.Services.Services.Empresa
{
    public class EmpresaClienteService : IEmpresaClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        private readonly IConfiguracionService _configuracionService;
        private readonly IUsuarioService _usuarioService;
        private readonly ICorreoEnviarService _correoEnviarService;
        private readonly IEncriptacionService _encriptacionService;
        private readonly IPlantillaNotificacionService _plantillaNotificacionService;

        public EmpresaClienteService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options,
            IConfiguracionService configuracionService,
            IUsuarioService usuarioService,
            ICorreoEnviarService correoEnviarService,
            IEncriptacionService encriptacionService,
            IPlantillaNotificacionService plantillaNotificacionService
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
            _configuracionService = configuracionService;
            _usuarioService = usuarioService;
            _correoEnviarService = correoEnviarService;
            _encriptacionService = encriptacionService;
            _plantillaNotificacionService = plantillaNotificacionService;
        }

        public async Task<PagedList<EmpresaCliente>> GetAll(EmpresaClienteQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

            IEnumerable<EmpresaCliente> data;

            if (filters.DatosCompletos == 0)
            {
                data = _unitOfWork.EmpresaClienteRepository.GetAll();
            }
            else
            {
                data = _unitOfWork.EmpresaClienteRepository.GetAll();
            }

            if (filters.Id != null && filters.Id != Guid.Empty) { data = data.Where(x => x.Id == filters.Id); }
            if (filters.RazonSocial != null) { data = data.Where(x => x.RazonSocial.ToLower().Contains(filters.RazonSocial.ToLower())); }
            if (filters.RFC != null) { data = data.Where(x => x.RFC.ToLower().Contains(filters.RFC.ToLower())); }
            if (filters.TelefonoEmpresa != null) { data = data.Where(x => x.TelefonoEmpresa.ToLower().Contains(filters.TelefonoEmpresa.ToLower())); }
            if (filters.TelefonoMovil != null) { data = data.Where(x => x.TelefonoMovil.ToLower().Contains(filters.TelefonoMovil.ToLower())); }
            if (filters.CorreoElectronico != null) { data = data.Where(x => x.CorreoElectronico.ToLower().Contains(filters.CorreoElectronico.ToLower())); }
            if (filters.UsaCredencialesHID != null && filters.UsaCredencialesHID > 0) { data = data.Where(x => x.UsaCredencialesHID == filters.UsaCredencialesHID); }

            if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { data = data.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
            if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { data = data.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
            if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { data = data.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
            if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { data = data.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
            if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
            if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
            if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
            if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
            if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
            if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
            if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
            if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
            if (filters.Estado != null && filters.Estado > 0) { data = data.Where(x => x.Estado == filters.Estado); }

            var pagedClientCompany = PagedList<EmpresaCliente>.Create(data, filters.PageNumber, filters.PageSize);

            return pagedClientCompany;
        }

        public async Task<EmpresaCliente> GetById(Guid id)
        {
            try
            {
                EmpresaCliente data = await _unitOfWork.EmpresaClienteRepository.GetById(id);
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<EmpresaCliente> GetCompanyClient(Guid id)
        {
            try
            {
                EmpresaCliente data = await _unitOfWork.EmpresaClienteRepository.GetCompanyClient(g => g.Id == id);
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Inactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                EmpresaCliente clientCompany = await GetById(id);
                if (clientCompany == null) { return false; }

                clientCompany.FechaBaja = DateTime.Now;
                clientCompany.UsuarioBajaId = currentUserId;
                clientCompany.Estado = 2;

                _unitOfWork.EmpresaClienteRepository.Update(clientCompany);

                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> Reactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                EmpresaCliente clientCompany = await GetById(id);
                if (clientCompany == null) { return false; }

                clientCompany.FechaReactivacion = DateTime.Now;
                clientCompany.UsuarioReactivadorId = currentUserId;
                clientCompany.Estado = 1;

                _unitOfWork.EmpresaClienteRepository.Update(clientCompany);

                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> CreateWithHID(EmpresaCliente clientCompany, List<ConfiguracionesReqDTO>? settings, string password, string passwordHash, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                if (currentUserId == Guid.Empty) { return false; }

                clientCompany.Id = Guid.NewGuid();
                clientCompany.UsuarioCreadorId = currentUserId;
                clientCompany.FechaCreacion = DateTime.Now;
                clientCompany.Estado = 1;

                await _unitOfWork.EmpresaClienteRepository.Add(clientCompany);
                await _unitOfWork.SaveChangesAsync();

                if (clientCompany.UsaCredencialesHID == 1)
                {
                    if (settings != null)
                    {
                        booOk = await _configuracionService.CreateSettingsForCompany(settings, clientCompany.Id, currentUserId);
                        var setting = await _configuracionService.GetByTypeSettingAndCompanyId(new Guid("742CE98B-684B-4A76-BA0D-CF62621FC3E7"), clientCompany.Id);
                        if (setting != null)
                        {
                            var taskById = await _unitOfWork.TipoTareaRepository.GetById(new Guid("90E155AB-EEE5-4B24-943C-9407A27F344D"));
                            if (taskById != null)
                            {
                                string valor = setting.Valor1!;
                                if (int.TryParse(valor, out int userId))
                                {
                                    var licenseHIDByTask = new LicenseHIDByTask
                                    {
                                        EmpresaClienteId = clientCompany.Id,
                                        UserId = userId
                                    };

                                    var jsonOptions = new System.Text.Json.JsonSerializerOptions
                                    {
                                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                        WriteIndented = false
                                    };

                                    Tarea task = new Tarea();
                                    task.TipoTareaId = new Guid("90E155AB-EEE5-4B24-943C-9407A27F344D");
                                    task.Fecha = DateTime.Now;
                                    task.Pendiente = 1;
                                    task.Status = 1;
                                    task.ValorEnvio = System.Text.Json.JsonSerializer.Serialize(licenseHIDByTask, jsonOptions);
                                    task.ValorRetorno = "";
                                    task.EmpresaClienteId = clientCompany.Id;
                                    task.Id = Guid.NewGuid();
                                    task.UsuarioCreadorId = currentUserId;
                                    task.FechaCreacion = DateTime.Now;
                                    task.Estado = 1;

                                    await _unitOfWork.TareaRepository.Add(task);
                                    await _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
                else
                {
                    booOk = true;
                }

                List<ConfiguracionesReqDTO> settingCommon = new List<ConfiguracionesReqDTO>()
                {
                    new ConfiguracionesReqDTO(){ NombreParametro = "Plantilla correo HID", ValorGuid = null, Valor1 = "<!DOCTYPE html>\r\n<html lang=\"es\">\r\n<head>\r\n  <meta charset=\"UTF-8\" />\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />\r\n  <title>Código de Acceso - Plataforma HID</title>\r\n  <style>\r\n    /* -- Reset básico compatible con clientes de correo -- */\r\n    body, table, td, a { -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }\r\n    table, td { mso-table-lspace: 0pt; mso-table-rspace: 0pt; }\r\n    img { -ms-interpolation-mode: bicubic; border: 0; }\r\n    body { margin: 0; padding: 0; background-color: #f4f6f9; font-family: Arial, sans-serif; }\r\n\r\n    /* -- Layout -- */\r\n    .email-outer  { width: 100%; background-color: #f4f6f9; padding: 32px 0; }\r\n    .email-inner  { max-width: 600px; margin: 0 auto; background: #ffffff;\r\n                    border-radius: 8px; overflow: hidden;\r\n                    box-shadow: 0 2px 8px rgba(0,0,0,.08); }\r\n\r\n    /* -- Header -- */\r\n    .email-header { \r\n      background: #002366;\r\n      text-align: center;\r\n      padding: 14px;\r\n    }\r\n    .email-logo   { \r\n      display: inline-block;\r\n      background: white;\r\n      border-radius: 4px;\r\n      padding: 3px 12px;\r\n      font-size: 11px;\r\n      font-weight: 700;\r\n      color: #002366;\r\n    }\r\n\r\n    /* -- Body -- */\r\n    .email-body   { \r\n      padding: 16px 18px;\r\n\r\n      h2 {\r\n          font-size: 13px;\r\n          color: #002366;\r\n          margin: 0 0 6px;\r\n\r\n          em {\r\n              font-style: normal;\r\n              color: #FF6F00;\r\n          }\r\n      }\r\n\r\n      p {\r\n          font-size: 11px;\r\n          color: #444;\r\n          line-height: 1.5;\r\n          margin: 0 0 6px;\r\n      }\r\n    }\r\n\r\n    /* -- Secciones / pasos -- */\r\n    .ewt-email-section {\r\n      margin: 10px 0;\r\n      padding: 9px 11px;\r\n      background: #FFF4E6;\r\n      border-left: 4px solid #FF6F00;\r\n\r\n      h3 {\r\n        font-size: 11px;\r\n        font-weight: 700;\r\n        color: #002366;\r\n        margin: 0 0 4px;\r\n      }\r\n\r\n      p {\r\n          font-size: 11px;\r\n          color: #555;\r\n          margin: 0;\r\n      }\r\n    };\r\n    .step-block   { margin: 24px 0; padding: 20px 24px;\r\n                    border-left: 4px solid #0d6efd; background: #f8faff;\r\n                    border-radius: 0 6px 6px 0; }\r\n    .step-block h3 { margin: 0 0 8px; font-size: 16px; color: #1a3a5c; }\r\n    .step-block p  { margin: 0; font-size: 14px; color: #555; }\r\n\r\n    /* -- Botones tiendas -- */\r\n    .store-row    { margin-top: 14px; }\r\n    .store-btn    { display: inline-block; margin-right: 10px; padding: 8px 18px;\r\n                    background: #1a3a5c; color: #ffffff; border-radius: 20px;\r\n                    font-size: 13px; text-decoration: none; }\r\n\r\n    /* -- QR -- */\r\n    .qr-wrapper   { text-align: center; margin: 16px 0; }\r\n    .qr-wrapper img { width: 130px; height: 130px; }\r\n    .qr-caption   { text-align: center; font-size: 13px; color: #888; margin-top: 6px; }\r\n\r\n    /* -- Código de invitación -- */\r\n    .invite-code  { text-align: center; font-size: 28px; font-weight: 700;\r\n                    letter-spacing: 4px; color: #002366;\r\n                    background: #eef4ff; border: 2px dashed #002366;\r\n                    border-radius: 8px; padding: 16px; margin: 16px 0; }\r\n\r\n    /* -- Contacto -- */\r\n    .contact-box  { \r\n      background: #F8F9FF;\r\n      padding: 9px;\r\n      border: 2px solid #002366;\r\n      border-radius: 4px;\r\n      margin: 10px 0;\r\n      font-size: 10px;\r\n      color: #002366;\r\n      line-height: 1.8;\r\n      text-align: center;\r\n    }\r\n\r\n    /* -- Footer -- */\r\n    .email-footer { \r\n      background: #002366;\r\n      color: white;\r\n      padding: 10px;\r\n      text-align: center;\r\n      font-size: 9px;\r\n      line-height: 1.6;\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"email-outer\">\r\n    <div class=\"email-inner\">\r\n\r\n      <!-- Encabezado -->\r\n      <div class=\"email-header\">\r\n        <div class=\"email-logo\">CRC de México®, S.A. de C.V.</div>\r\n      </div>\r\n\r\n      <!-- Cuerpo -->\r\n      <div class=\"email-body\">\r\n        <h2>Estimado(a): <em>{{destinatario}}</em>,</h2>\r\n        <p>Gracias por utilizar nuestros servicios. Aquí están los detalles de su acceso:</p>\r\n\r\n        <!-- Paso 1 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 1. Descarga nuestra App</h3>\r\n          <p>Accede fácilmente desde tu dispositivo móvil descargando la aplicación.</p>\r\n          <div class=\"store-row\">\r\n            <a href=\"#\" class=\"store-btn\">&#9654; Google Play</a>\r\n            <a href=\"#\" class=\"store-btn\">&#9654; App Store</a>\r\n          </div>\r\n        </div>\r\n\r\n        <!-- Paso 2 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 2. Para acceder</h3>\r\n          <p>Abre la app, selecciona 'Continuar como invitado', presiona el ícono de QR y escanea el código.</p>\r\n          <div class=\"qr-wrapper\">\r\n            <img src=\"data:image/svg+xml;base64,{{qrBase64}}\" alt=\"Código QR de acceso\" />\r\n          </div>\r\n          <p class=\"qr-caption\">Escanea este código QR para validar tu acceso.</p>\r\n        </div>\r\n\r\n        <!-- Paso 3 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 3. Alternativa - Código manual</h3>\r\n          <p>Copia el código, pégalo en 'Canjear código' y presiona 'Canjear'. Mantén la app en vista de credenciales frente al lector.</p>\r\n          <div class=\"invite-code\">{{codigoInvitacion}}</div>\r\n          <p style=\"font-size:13px;color:#888;text-align:center;\">\r\n            Válido hasta: {{fechaExpiracion}}\r\n          </p>\r\n        </div>\r\n\r\n        <!-- Datos de contacto -->\r\n        <div class=\"contact-box\">\r\n          &#128222; <strong>Tel:</strong> +52 (443) 340 0992<br/>\r\n          &#128231; <strong>Email:</strong> omorales@crcdemexico.com.mx<br/>\r\n          &#128205; <strong>Dirección:</strong> Lic. Antonio del Moral 45, Nueva Chapultepec, 58280 Morelia, Mich.\r\n        </div>\r\n      </div>\r\n\r\n      <!-- Pie -->\r\n      <div class=\"email-footer\">\r\n        &copy; 2026 CRC de México®, S.A. de C.V.. Todos los derechos reservados.<br/>\r\n        Este es un mensaje automático, por favor no responda a este correo.\r\n      </div>\r\n\r\n    </div>\r\n  </div>\r\n</body>\r\n</html>", Valor2 = "", Valor3 = "", editable = 1, lectura = 1, EmpresaClienteId = clientCompany.Id, TipoConfiguracion = new Guid("E1A2B3C4-D5E6-7F89-A0B1-C2D3E4F50001"), UsuarioCreadorId = currentUserId},
                    new ConfiguracionesReqDTO(){ NombreParametro = "Plantilla correo Wallet", ValorGuid = null, Valor1 = "<!DOCTYPE html>\r\n<html lang=\"es\">\r\n<head>\r\n  <meta charset=\"UTF-8\" />\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />\r\n  <title>Código de Acceso - Plataforma HID</title>\r\n  <style>\r\n    /* -- Reset básico compatible con clientes de correo -- */\r\n    body, table, td, a { -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }\r\n    table, td { mso-table-lspace: 0pt; mso-table-rspace: 0pt; }\r\n    img { -ms-interpolation-mode: bicubic; border: 0; }\r\n    body { margin: 0; padding: 0; background-color: #f4f6f9; font-family: Arial, sans-serif; }\r\n\r\n    /* -- Layout -- */\r\n    .email-outer  { width: 100%; background-color: #f4f6f9; padding: 32px 0; }\r\n    .email-inner  { max-width: 600px; margin: 0 auto; background: #ffffff;\r\n                    border-radius: 8px; overflow: hidden;\r\n                    box-shadow: 0 2px 8px rgba(0,0,0,.08); }\r\n\r\n    /* -- Header -- */\r\n    .email-header { \r\n      background: #002366;\r\n      text-align: center;\r\n      padding: 14px;\r\n    }\r\n    .email-logo   { \r\n      display: inline-block;\r\n      background: white;\r\n      border-radius: 4px;\r\n      padding: 3px 12px;\r\n      font-size: 11px;\r\n      font-weight: 700;\r\n      color: #002366;\r\n    }\r\n\r\n    /* -- Body -- */\r\n    .email-body   { \r\n      padding: 16px 18px;\r\n\r\n      h2 {\r\n          font-size: 13px;\r\n          color: #002366;\r\n          margin: 0 0 6px;\r\n\r\n          em {\r\n              font-style: normal;\r\n              color: #FF6F00;\r\n          }\r\n      }\r\n\r\n      p {\r\n          font-size: 11px;\r\n          color: #444;\r\n          line-height: 1.5;\r\n          margin: 0 0 6px;\r\n      }\r\n    }\r\n\r\n    /* -- Secciones / pasos -- */\r\n    .ewt-email-section {\r\n      margin: 10px 0;\r\n      padding: 9px 11px;\r\n      background: #FFF4E6;\r\n      border-left: 4px solid #FF6F00;\r\n\r\n      h3 {\r\n        font-size: 11px;\r\n        font-weight: 700;\r\n        color: #002366;\r\n        margin: 0 0 4px;\r\n      }\r\n\r\n      p {\r\n          font-size: 11px;\r\n          color: #555;\r\n          margin: 0;\r\n      }\r\n    };\r\n    .step-block   { margin: 24px 0; padding: 20px 24px;\r\n                    border-left: 4px solid #0d6efd; background: #f8faff;\r\n                    border-radius: 0 6px 6px 0; }\r\n    .step-block h3 { margin: 0 0 8px; font-size: 16px; color: #1a3a5c; }\r\n    .step-block p  { margin: 0; font-size: 14px; color: #555; }\r\n\r\n    /* -- Botones tiendas -- */\r\n    .store-row    { margin-top: 14px; }\r\n    .store-btn    { display: inline-block; margin-right: 10px; padding: 8px 18px;\r\n                    background: #1a3a5c; color: #ffffff; border-radius: 20px;\r\n                    font-size: 13px; text-decoration: none; }\r\n\r\n    /* -- QR -- */\r\n    .qr-wrapper   { text-align: center; margin: 16px 0; }\r\n    .qr-wrapper img { width: 130px; height: 130px; }\r\n    .qr-caption   { text-align: center; font-size: 13px; color: #888; margin-top: 6px; }\r\n\r\n    /* -- Código de invitación -- */\r\n    .invite-code  { text-align: center; font-size: 28px; font-weight: 700;\r\n                    letter-spacing: 4px; color: #002366;\r\n                    background: #eef4ff; border: 2px dashed #002366;\r\n                    border-radius: 8px; padding: 16px; margin: 16px 0; }\r\n\r\n    /* -- Contacto -- */\r\n    .contact-box  { \r\n      background: #F8F9FF;\r\n      padding: 9px;\r\n      border: 2px solid #002366;\r\n      border-radius: 4px;\r\n      margin: 10px 0;\r\n      font-size: 10px;\r\n      color: #002366;\r\n      line-height: 1.8;\r\n      text-align: center;\r\n    }\r\n\r\n    /* -- Footer -- */\r\n    .email-footer { \r\n      background: #002366;\r\n      color: white;\r\n      padding: 10px;\r\n      text-align: center;\r\n      font-size: 9px;\r\n      line-height: 1.6;\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"email-outer\">\r\n    <div class=\"email-inner\">\r\n\r\n      <!-- Encabezado -->\r\n      <div class=\"email-header\">\r\n        <div class=\"email-logo\">CRC de México®, S.A. de C.V.</div>\r\n      </div>\r\n\r\n      <!-- Cuerpo -->\r\n      <div class=\"email-body\">\r\n        <h2>Estimado(a): <em>{{destinatario}}</em>,</h2>\r\n        <p>Gracias por utilizar nuestros servicios. Aquí están los detalles de su acceso:</p>\r\n\r\n        <!-- Paso 1 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 1. Descarga nuestra App</h3>\r\n          <p>Accede fácilmente desde tu dispositivo móvil descargando la aplicación.</p>\r\n          <div class=\"store-row\">\r\n            <a href=\"#\" class=\"store-btn\">&#9654; Google Play</a>\r\n            <a href=\"#\" class=\"store-btn\">&#9654; App Store</a>\r\n          </div>\r\n        </div>\r\n\r\n        <!-- Paso 2 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 2. Para acceder</h3>\r\n          <p>Abre la app, selecciona 'Continuar como invitado', presiona el ícono de QR y escanea el código.</p>\r\n          <div class=\"qr-wrapper\">\r\n            <img src=\"data:image/svg+xml;base64,{{qrBase64}}\" alt=\"Código QR de acceso\" />\r\n          </div>\r\n          <p class=\"qr-caption\">Escanea este código QR para validar tu acceso.</p>\r\n        </div>\r\n\r\n        <!-- Paso 3 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 3. Alternativa - Código manual</h3>\r\n          <p>Copia el código, pégalo en 'Canjear código' y presiona 'Canjear'. Mantén la app en vista de credenciales frente al lector.</p>\r\n          <div class=\"invite-code\">{{codigoInvitacion}}</div>\r\n          <p style=\"font-size:13px;color:#888;text-align:center;\">\r\n            Válido hasta: {{fechaExpiracion}}\r\n          </p>\r\n        </div>\r\n\r\n        <!-- Datos de contacto -->\r\n        <div class=\"contact-box\">\r\n          &#128222; <strong>Tel:</strong> +52 (443) 340 0992<br/>\r\n          &#128231; <strong>Email:</strong> omorales@crcdemexico.com.mx<br/>\r\n          &#128205; <strong>Dirección:</strong> Lic. Antonio del Moral 45, Nueva Chapultepec, 58280 Morelia, Mich.\r\n        </div>\r\n      </div>\r\n\r\n      <!-- Pie -->\r\n      <div class=\"email-footer\">\r\n        &copy; 2026 CRC de México®, S.A. de C.V.. Todos los derechos reservados.<br/>\r\n        Este es un mensaje automático, por favor no responda a este correo.\r\n      </div>\r\n\r\n    </div>\r\n  </div>\r\n</body>\r\n</html>", Valor2 = "", Valor3 = "", editable = 1, lectura = 1, EmpresaClienteId = clientCompany.Id, TipoConfiguracion = new Guid("0078A82D-44C5-4EA9-9AD7-61FF6578BACF"), UsuarioCreadorId = currentUserId}
                };
                if (settingCommon.Count() != 0)
                {
                    await _configuracionService.CreateSettingsForCompany(settingCommon, clientCompany.Id, currentUserId);
                }

                var user = new Usuario()
                {
                    Correo = clientCompany.CorreoElectronico,
                    Contrasena = passwordHash,
                    Vence = 2,
                    FechaVencimiento = null,
                    PerfilId = new Guid("0043801F-F691-45C9-BCDA-3E131E3766F2"),
                    TipoUsuarioId = new Guid("2228D6FB-CBDD-4672-9A06-A6E054157E6D"),
                    EmpresaClienteId = clientCompany.Id,
                    Clave = null
                };
                var userResponse = await _usuarioService.Create(user, password, currentUserId, clientCompany.Id);
                if (userResponse == true)
                {
                    var email = new CorreoEnviarUsuario()
                    {
                        Nombre = "Sin nombre",
                        Correo = clientCompany.CorreoElectronico,
                        Contrasena = password
                    };

                    await _correoEnviarService.SendUserEmail(email, currentUserId, clientCompany.Id);
                }
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> CreateWithSettingEncrypted(EmpresaCliente clientCompany, List<SettingsGroupTapDTO>? settingHIDEncrypted, List<SettingsGroupTapDTO>? settingWalletEncrypted, string password, string passwordHash, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }

                clientCompany.Id = Guid.NewGuid();
                clientCompany.UsuarioCreadorId = currentUserId;
                clientCompany.FechaCreacion = DateTime.Now;
                clientCompany.Estado = 1;

                await _unitOfWork.EmpresaClienteRepository.Add(clientCompany);
                await _unitOfWork.SaveChangesAsync();

                if (clientCompany.UsaCredencialesHID == 1)
                {
                    if (settingHIDEncrypted != null)
                    {
                        var settingHID = settingHIDEncrypted
                            .Where(grupo => grupo.Items != null)
                            .SelectMany(grupo => grupo.Items)
                            .Select(item => new ConfiguracionesReqDTO()
                            {
                                NombreParametro = item.Nombre,
                                ValorGuid = item.ValorGuid,
                                Valor1 = item.Valor1,
                                Valor2 = "",
                                Valor3 = "",
                                editable = 1,
                                lectura = 1,
                                EmpresaClienteId = clientCompany.Id,
                                TipoConfiguracion = item.TipoConfiguracion,
                                UsuarioCreadorId = currentUserId
                            })
                            .ToList();

                        if (settingHID.Count() != 0)
                        {
                            await _configuracionService.CreateSettingsForCompany(settingHID, clientCompany.Id, currentUserId);

                            var jsonOptions = new JsonSerializerOptions
                            {
                                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                WriteIndented = false,
                                ReferenceHandler = ReferenceHandler.IgnoreCycles
                            };
                            var task = new TestConnectionDTO()
                            {
                                CustomerId = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("742CE98B-684B-4A76-BA0D-CF62621FC3E7"))?.Valor1,
                                ClientId = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("BB617929-5F49-4FDC-8C28-62435505B600"))?.Valor1,
                                ClientSecretOrCertificate = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("29625587-4A45-495A-B728-203608694C44"))?.Valor1,
                                IdpAuthenticationUrl = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("60ADEBFE-01B5-497A-828B-CF3801F37495"))?.Valor1,
                                ApiUrl = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("9B02E35B-A069-4BF5-B9CA-337A59455347"))?.Valor1,
                                ApplicationId = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("788F90F3-0CE3-4E96-B4BA-38DA1CFE105B"))?.Valor1,
                                ApplicationVersion = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("FF5E7D45-FCED-4169-B4EB-BA70B43F7BB6"))?.Valor1,
                                EmpresaClienteId = clientCompany.Id
                            };
                            var taskNew = new Tarea
                            {
                                Id = Guid.NewGuid(),
                                TipoTareaId = new Guid("D333E531-6DAE-49B5-AA40-3301FE4EE2E9"),
                                Fecha = DateTime.Now,
                                Pendiente = 1,
                                Status = 1,
                                Estado = 1,
                                Marca = 1,
                                ValorEnvio = System.Text.Json.JsonSerializer.Serialize(task, jsonOptions),
                                ValorRetorno = "",
                                FechaCreacion = DateTime.Now,
                                UsuarioCreadorId = currentUserId,
                                EmpresaClienteId = clientCompany.Id
                            };
                            await _unitOfWork.TareaRepository.Add(taskNew);
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                }
                if (clientCompany.UsaCredencialesWallet == 1)
                {
                    if (settingWalletEncrypted != null)
                    {
                        var settingWallet = settingWalletEncrypted
                            .Where(grupo => grupo.Items != null)
                            .SelectMany(grupo => grupo.Items)
                            .Select(item => new ConfiguracionesReqDTO()
                            {
                                NombreParametro = item.Nombre,
                                ValorGuid = item.ValorGuid,
                                Valor1 = item.Valor1,
                                Valor2 = "",
                                Valor3 = "",
                                editable = 1,
                                lectura = 1,
                                EmpresaClienteId = clientCompany.Id,
                                TipoConfiguracion = item.TipoConfiguracion,
                                UsuarioCreadorId = currentUserId
                            })
                            .ToList();

                        if (settingWallet.Count() != 0)
                        {
                            await _configuracionService.CreateSettingsForCompany(settingWallet, clientCompany.Id, currentUserId);

                            var jsonOptions = new JsonSerializerOptions
                            {
                                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                WriteIndented = false,
                                ReferenceHandler = ReferenceHandler.IgnoreCycles
                            };
                            var task = new TestConnectionDTO()
                            {
                                CustomerId = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("742CE98B-684B-4A76-BA0D-CF62621FC3E7"))?.Valor1,
                                ClientId = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("BB617929-5F49-4FDC-8C28-62435505B600"))?.Valor1,
                                ClientSecretOrCertificate = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("29625587-4A45-495A-B728-203608694C44"))?.Valor1,
                                IdpAuthenticationUrl = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("60ADEBFE-01B5-497A-828B-CF3801F37495"))?.Valor1,
                                ApiUrl = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("9B02E35B-A069-4BF5-B9CA-337A59455347"))?.Valor1,
                                ApplicationId = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("788F90F3-0CE3-4E96-B4BA-38DA1CFE105B"))?.Valor1,
                                ApplicationVersion = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("FF5E7D45-FCED-4169-B4EB-BA70B43F7BB6"))?.Valor1,
                                EmpresaClienteId = clientCompany.Id
                            };
                            var taskNew = new Tarea
                            {
                                Id = Guid.NewGuid(),
                                TipoTareaId = new Guid("DD909EF0-E527-47FB-A0A3-204794A81F12"),
                                Fecha = DateTime.Now,
                                Pendiente = 1,
                                Status = 1,
                                Estado = 1,
                                Marca = 1,
                                ValorEnvio = System.Text.Json.JsonSerializer.Serialize(task, jsonOptions),
                                ValorRetorno = "",
                                FechaCreacion = DateTime.Now,
                                UsuarioCreadorId = currentUserId,
                                EmpresaClienteId = clientCompany.Id
                            };
                            await _unitOfWork.TareaRepository.Add(taskNew);
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                }

                List<ConfiguracionesReqDTO> settingCommon = new List<ConfiguracionesReqDTO>()
                {
                    new ConfiguracionesReqDTO(){ NombreParametro = "Plantilla correo HID", ValorGuid = null, Valor1 = "...", Valor2 = "", Valor3 = "", editable = 1, lectura = 1, EmpresaClienteId = clientCompany.Id, TipoConfiguracion = new Guid("E1A2B3C4-D5E6-7F89-A0B1-C2D3E4F50001"), UsuarioCreadorId = currentUserId},
                    new ConfiguracionesReqDTO(){ NombreParametro = "Plantilla correo Wallet", ValorGuid = null, Valor1 = "...", Valor2 = "", Valor3 = "", editable = 1, lectura = 1, EmpresaClienteId = clientCompany.Id, TipoConfiguracion = new Guid("0078A82D-44C5-4EA9-9AD7-61FF6578BACF"), UsuarioCreadorId = currentUserId}
                };
                if (settingCommon.Count() != 0)
                {
                    await _configuracionService.CreateSettingsForCompany(settingCommon, clientCompany.Id, currentUserId);
                }

                var user = new Usuario()
                {
                    Correo = clientCompany.CorreoElectronico,
                    Contrasena = passwordHash,
                    Vence = 2,
                    FechaVencimiento = null,
                    PerfilId = new Guid("0043801F-F691-45C9-BCDA-3E131E3766F2"),
                    TipoUsuarioId = new Guid("2228D6FB-CBDD-4672-9A06-A6E054157E6D"),
                    EmpresaClienteId = clientCompany.Id,
                    Clave = null
                };
                var userResponse = await _usuarioService.Create(user, password, currentUserId, clientCompany.Id);
                if (userResponse == true)
                {
                    var email = new CorreoEnviarUsuario()
                    {
                        Nombre = "Sin nombre",
                        Correo = clientCompany.CorreoElectronico,
                        Contrasena = password
                    };

                    await _correoEnviarService.SendUserEmail(email, currentUserId, clientCompany.Id);
                }

                var notificationTemplate = new PlantillaNotificacion()
                {
                    Nombre = "Plantilla de notificación de alta de usuario",
                    CuerpoPlantilla = "Hola,\r\n\r\n    ¡Bienvenido(a) al Portal WebVisitsMobile!\r\n\r\n    Nos alegra tenerte con nosotros. A continuación, encontrarás tus credenciales para acceder al portal:\r\n\r\n    Usuario: #Usuario\r\n    Contraseña: #Contrasena\r\n    Link: \r\n\r\n    Recuerda cambiar tu contraseña después de iniciar sesión por primera vez para mantener tu cuenta segura.\r\n\r\n    Si tienes alguna pregunta o necesitas ayuda, no dudes en contactarnos.\r\n\r\n    Saludos cordiales.\r\n",
                    NotificarEmail = 2,
                    NotificarTeams = 2,
                    Identificador = null,
                    TipoPlantillaNotificacionId = new Guid("00EBBDAB-B1F6-4A5E-90DE-4333BA36F18B"),
                    EmpresaClienteId = clientCompany.Id
                };

                await _plantillaNotificacionService.Create(notificationTemplate, currentUserId, clientCompany.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateWithSettingEncrypted(EmpresaCliente clientCompany, List<SettingsGroupTapDTO>? settingHIDEncrypted, List<SettingsGroupTapDTO>? settingWalletEncrypted, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) return false;

                // Actualizar datos de la empresa
                EmpresaCliente clientCompanyUpdate = await _unitOfWork.EmpresaClienteRepository.GetById(clientCompany.Id);
                if (clientCompanyUpdate == null) return false;

                clientCompanyUpdate.RazonSocial = clientCompany.RazonSocial;
                clientCompanyUpdate.RFC = clientCompany.RFC;
                clientCompanyUpdate.TelefonoEmpresa = clientCompany.TelefonoEmpresa;
                clientCompanyUpdate.TelefonoMovil = clientCompany.TelefonoMovil;
                clientCompanyUpdate.CorreoElectronico = clientCompany.CorreoElectronico;

                clientCompanyUpdate.UsaCredencialesHID = clientCompany.UsaCredencialesHID;
                clientCompanyUpdate.UsaCredencialesWallet = clientCompany.UsaCredencialesWallet;

                clientCompanyUpdate.PaisId = clientCompany.PaisId;
                clientCompanyUpdate.EstadoId = clientCompany.EstadoId;
                clientCompanyUpdate.CiudadId = clientCompany.CiudadId;

                clientCompanyUpdate.FechaModificacion = DateTime.UtcNow;
                clientCompanyUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.EmpresaClienteRepository.Update(clientCompanyUpdate);
                await _unitOfWork.SaveChangesAsync();

                // Si desactivó HID → desactivar todas sus configuraciones
                var settingDelete = await _configuracionService.DeleteAllSettingsByCompany(clientCompany.Id);
                if (settingDelete == true)
                {
                    if (clientCompany.UsaCredencialesHID == 1)
                    {
                        if (settingHIDEncrypted != null)
                        {
                            //var datos_encriptados = await _encriptacionService.EncriptarCadena(settingHIDEncrypted);
                            //var datos_encriptados_cadena = JsonConvert.SerializeObject(datos_encriptados);
                            //List<ConfiguracionesReqDTO> settingHID = new List<ConfiguracionesReqDTO>()
                            //{
                            //    new ConfiguracionesReqDTO(){ NombreParametro = "Usa credenciales HID", ValorGuid = null, Valor1 = datos_encriptados_cadena, Valor2 = "", Valor3 = "", editable = 1, lectura = 1, EmpresaClienteId = clientCompany.Id, TipoConfiguracion = new Guid("BB164E4E-F6F3-4C6A-9CE4-0B646B2A0433"), UsuarioCreadorId = currentUserId}
                            //};
                            //await _configuracionService.CreateSettingsForCompany(settingHID, clientCompany.Id, currentUserId);

                            var settingHID = settingHIDEncrypted
                            .Where(grupo => grupo.Items != null)
                            .SelectMany(grupo => grupo.Items)
                            .Select(item => new ConfiguracionesReqDTO()
                            {
                                NombreParametro = item.Nombre,
                                ValorGuid = item.ValorGuid,
                                Valor1 = item.Valor1,
                                Valor2 = "",
                                Valor3 = "",
                                editable = 1,
                                lectura = 1,
                                EmpresaClienteId = clientCompany.Id,
                                TipoConfiguracion = item.TipoConfiguracion,
                                UsuarioCreadorId = currentUserId
                            })
                            .ToList();

                            if (settingHID.Count() != 0)
                            {
                                await _configuracionService.CreateSettingsForCompany(settingHID, clientCompany.Id, currentUserId);

                                var jsonOptions = new JsonSerializerOptions
                                {
                                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                    WriteIndented = false,
                                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                                };
                                var task = new TestConnectionDTO()
                                {
                                    CustomerId = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("742CE98B-684B-4A76-BA0D-CF62621FC3E7"))?.Valor1,
                                    ClientId = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("BB617929-5F49-4FDC-8C28-62435505B600"))?.Valor1,
                                    ClientSecretOrCertificate = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("29625587-4A45-495A-B728-203608694C44"))?.Valor1,
                                    IdpAuthenticationUrl = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("60ADEBFE-01B5-497A-828B-CF3801F37495"))?.Valor1,
                                    ApiUrl = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("9B02E35B-A069-4BF5-B9CA-337A59455347"))?.Valor1,
                                    ApplicationId = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("788F90F3-0CE3-4E96-B4BA-38DA1CFE105B"))?.Valor1,
                                    ApplicationVersion = settingHID.FirstOrDefault(x => x.TipoConfiguracion == new Guid("FF5E7D45-FCED-4169-B4EB-BA70B43F7BB6"))?.Valor1,
                                    EmpresaClienteId = clientCompany.Id
                                };
                                var taskNew = new Tarea
                                {
                                    Id = Guid.NewGuid(),
                                    TipoTareaId = new Guid("D333E531-6DAE-49B5-AA40-3301FE4EE2E9"),
                                    Fecha = DateTime.Now,
                                    Pendiente = 1,
                                    Status = 1,
                                    Estado = 1,
                                    Marca = 1,
                                    ValorEnvio = System.Text.Json.JsonSerializer.Serialize(task, jsonOptions),
                                    ValorRetorno = "",
                                    FechaCreacion = DateTime.Now,
                                    UsuarioCreadorId = currentUserId,
                                    EmpresaClienteId = clientCompany.Id
                                };
                                await _unitOfWork.TareaRepository.Add(taskNew);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }
                    if (clientCompany.UsaCredencialesWallet == 1)
                    {
                        //if (settingWalletEncrypted != null)
                        //{
                        //    var datos_encriptados = await _encriptacionService.EncriptarCadena(settingWalletEncrypted);
                        //    var datos_encriptados_cadena = JsonConvert.SerializeObject(datos_encriptados);
                        //    List<ConfiguracionesReqDTO> settingWallet = new List<ConfiguracionesReqDTO>()
                        //{
                        //    new ConfiguracionesReqDTO(){ NombreParametro = "Usa credenciales Wallet", ValorGuid = null, Valor1 = datos_encriptados_cadena, Valor2 = "", Valor3 = "", editable = 1, lectura = 1, EmpresaClienteId = clientCompany.Id, TipoConfiguracion = new Guid("10058B5D-8B95-4C27-9ED1-0426762154FD"), UsuarioCreadorId = currentUserId}
                        //};
                        //    await _configuracionService.CreateSettingsForCompany(settingWallet, clientCompany.Id, currentUserId);
                        //}

                        if (settingWalletEncrypted != null)
                        {
                            var settingWallet = settingWalletEncrypted
                                .Where(grupo => grupo.Items != null)
                                .SelectMany(grupo => grupo.Items)
                                .Select(item => new ConfiguracionesReqDTO()
                                {
                                    NombreParametro = item.Nombre,
                                    ValorGuid = item.ValorGuid,
                                    Valor1 = item.Valor1,
                                    Valor2 = "",
                                    Valor3 = "",
                                    editable = 1,
                                    lectura = 1,
                                    EmpresaClienteId = clientCompany.Id,
                                    TipoConfiguracion = item.TipoConfiguracion,
                                    UsuarioCreadorId = currentUserId
                                })
                                .ToList();

                            if (settingWallet.Count() != 0)
                            {
                                await _configuracionService.CreateSettingsForCompany(settingWallet, clientCompany.Id, currentUserId);

                                var jsonOptions = new JsonSerializerOptions
                                {
                                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                    WriteIndented = false,
                                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                                };
                                var task = new TestConnectionDTO()
                                {
                                    CustomerId = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("742CE98B-684B-4A76-BA0D-CF62621FC3E7"))?.Valor1,
                                    ClientId = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("BB617929-5F49-4FDC-8C28-62435505B600"))?.Valor1,
                                    ClientSecretOrCertificate = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("29625587-4A45-495A-B728-203608694C44"))?.Valor1,
                                    IdpAuthenticationUrl = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("60ADEBFE-01B5-497A-828B-CF3801F37495"))?.Valor1,
                                    ApiUrl = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("9B02E35B-A069-4BF5-B9CA-337A59455347"))?.Valor1,
                                    ApplicationId = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("788F90F3-0CE3-4E96-B4BA-38DA1CFE105B"))?.Valor1,
                                    ApplicationVersion = settingWallet.FirstOrDefault(x => x.TipoConfiguracion == new Guid("FF5E7D45-FCED-4169-B4EB-BA70B43F7BB6"))?.Valor1,
                                    EmpresaClienteId = clientCompany.Id
                                };
                                var taskNew = new Tarea
                                {
                                    Id = Guid.NewGuid(),
                                    TipoTareaId = new Guid("DD909EF0-E527-47FB-A0A3-204794A81F12"),
                                    Fecha = DateTime.Now,
                                    Pendiente = 1,
                                    Status = 1,
                                    Estado = 1,
                                    Marca = 1,
                                    ValorEnvio = System.Text.Json.JsonSerializer.Serialize(task, jsonOptions),
                                    ValorRetorno = "",
                                    FechaCreacion = DateTime.Now,
                                    UsuarioCreadorId = currentUserId,
                                    EmpresaClienteId = clientCompany.Id
                                };
                                await _unitOfWork.TareaRepository.Add(taskNew);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }

                    List<ConfiguracionesReqDTO> settingCommon = new List<ConfiguracionesReqDTO>()
                    {
                        new ConfiguracionesReqDTO(){ NombreParametro = "Plantilla correo HID", ValorGuid = null, Valor1 = "<!DOCTYPE html>\r\n<html lang=\"es\">\r\n<head>\r\n  <meta charset=\"UTF-8\" />\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />\r\n  <title>Código de Acceso - Plataforma HID</title>\r\n  <style>\r\n    /* -- Reset básico compatible con clientes de correo -- */\r\n    body, table, td, a { -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }\r\n    table, td { mso-table-lspace: 0pt; mso-table-rspace: 0pt; }\r\n    img { -ms-interpolation-mode: bicubic; border: 0; }\r\n    body { margin: 0; padding: 0; background-color: #f4f6f9; font-family: Arial, sans-serif; }\r\n\r\n    /* -- Layout -- */\r\n    .email-outer  { width: 100%; background-color: #f4f6f9; padding: 32px 0; }\r\n    .email-inner  { max-width: 600px; margin: 0 auto; background: #ffffff;\r\n                    border-radius: 8px; overflow: hidden;\r\n                    box-shadow: 0 2px 8px rgba(0,0,0,.08); }\r\n\r\n    /* -- Header -- */\r\n    .email-header { \r\n      background: #002366;\r\n      text-align: center;\r\n      padding: 14px;\r\n    }\r\n    .email-logo   { \r\n      display: inline-block;\r\n      background: white;\r\n      border-radius: 4px;\r\n      padding: 3px 12px;\r\n      font-size: 11px;\r\n      font-weight: 700;\r\n      color: #002366;\r\n    }\r\n\r\n    /* -- Body -- */\r\n    .email-body   { \r\n      padding: 16px 18px;\r\n\r\n      h2 {\r\n          font-size: 13px;\r\n          color: #002366;\r\n          margin: 0 0 6px;\r\n\r\n          em {\r\n              font-style: normal;\r\n              color: #FF6F00;\r\n          }\r\n      }\r\n\r\n      p {\r\n          font-size: 11px;\r\n          color: #444;\r\n          line-height: 1.5;\r\n          margin: 0 0 6px;\r\n      }\r\n    }\r\n\r\n    /* -- Secciones / pasos -- */\r\n    .ewt-email-section {\r\n      margin: 10px 0;\r\n      padding: 9px 11px;\r\n      background: #FFF4E6;\r\n      border-left: 4px solid #FF6F00;\r\n\r\n      h3 {\r\n        font-size: 11px;\r\n        font-weight: 700;\r\n        color: #002366;\r\n        margin: 0 0 4px;\r\n      }\r\n\r\n      p {\r\n          font-size: 11px;\r\n          color: #555;\r\n          margin: 0;\r\n      }\r\n    };\r\n    .step-block   { margin: 24px 0; padding: 20px 24px;\r\n                    border-left: 4px solid #0d6efd; background: #f8faff;\r\n                    border-radius: 0 6px 6px 0; }\r\n    .step-block h3 { margin: 0 0 8px; font-size: 16px; color: #1a3a5c; }\r\n    .step-block p  { margin: 0; font-size: 14px; color: #555; }\r\n\r\n    /* -- Botones tiendas -- */\r\n    .store-row    { margin-top: 14px; }\r\n    .store-btn    { display: inline-block; margin-right: 10px; padding: 8px 18px;\r\n                    background: #1a3a5c; color: #ffffff; border-radius: 20px;\r\n                    font-size: 13px; text-decoration: none; }\r\n\r\n    /* -- QR -- */\r\n    .qr-wrapper   { text-align: center; margin: 16px 0; }\r\n    .qr-wrapper img { width: 130px; height: 130px; }\r\n    .qr-caption   { text-align: center; font-size: 13px; color: #888; margin-top: 6px; }\r\n\r\n    /* -- Código de invitación -- */\r\n    .invite-code  { text-align: center; font-size: 28px; font-weight: 700;\r\n                    letter-spacing: 4px; color: #002366;\r\n                    background: #eef4ff; border: 2px dashed #002366;\r\n                    border-radius: 8px; padding: 16px; margin: 16px 0; }\r\n\r\n    /* -- Contacto -- */\r\n    .contact-box  { \r\n      background: #F8F9FF;\r\n      padding: 9px;\r\n      border: 2px solid #002366;\r\n      border-radius: 4px;\r\n      margin: 10px 0;\r\n      font-size: 10px;\r\n      color: #002366;\r\n      line-height: 1.8;\r\n      text-align: center;\r\n    }\r\n\r\n    /* -- Footer -- */\r\n    .email-footer { \r\n      background: #002366;\r\n      color: white;\r\n      padding: 10px;\r\n      text-align: center;\r\n      font-size: 9px;\r\n      line-height: 1.6;\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"email-outer\">\r\n    <div class=\"email-inner\">\r\n\r\n      <!-- Encabezado -->\r\n      <div class=\"email-header\">\r\n        <div class=\"email-logo\">CRC de México®, S.A. de C.V.</div>\r\n      </div>\r\n\r\n      <!-- Cuerpo -->\r\n      <div class=\"email-body\">\r\n        <h2>Estimado(a): <em>{{destinatario}}</em>,</h2>\r\n        <p>Gracias por utilizar nuestros servicios. Aquí están los detalles de su acceso:</p>\r\n\r\n        <!-- Paso 1 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 1. Descarga nuestra App</h3>\r\n          <p>Accede fácilmente desde tu dispositivo móvil descargando la aplicación.</p>\r\n          <div class=\"store-row\">\r\n            <a href=\"#\" class=\"store-btn\">&#9654; Google Play</a>\r\n            <a href=\"#\" class=\"store-btn\">&#9654; App Store</a>\r\n          </div>\r\n        </div>\r\n\r\n        <!-- Paso 2 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 2. Para acceder</h3>\r\n          <p>Abre la app, selecciona 'Continuar como invitado', presiona el ícono de QR y escanea el código.</p>\r\n          <div class=\"qr-wrapper\">\r\n            <img src=\"data:image/svg+xml;base64,{{qrBase64}}\" alt=\"Código QR de acceso\" />\r\n          </div>\r\n          <p class=\"qr-caption\">Escanea este código QR para validar tu acceso.</p>\r\n        </div>\r\n\r\n        <!-- Paso 3 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 3. Alternativa - Código manual</h3>\r\n          <p>Copia el código, pégalo en 'Canjear código' y presiona 'Canjear'. Mantén la app en vista de credenciales frente al lector.</p>\r\n          <div class=\"invite-code\">{{codigoInvitacion}}</div>\r\n          <p style=\"font-size:13px;color:#888;text-align:center;\">\r\n            Válido hasta: {{fechaExpiracion}}\r\n          </p>\r\n        </div>\r\n\r\n        <!-- Datos de contacto -->\r\n        <div class=\"contact-box\">\r\n          &#128222; <strong>Tel:</strong> +52 (443) 340 0992<br/>\r\n          &#128231; <strong>Email:</strong> omorales@crcdemexico.com.mx<br/>\r\n          &#128205; <strong>Dirección:</strong> Lic. Antonio del Moral 45, Nueva Chapultepec, 58280 Morelia, Mich.\r\n        </div>\r\n      </div>\r\n\r\n      <!-- Pie -->\r\n      <div class=\"email-footer\">\r\n        &copy; 2026 CRC de México®, S.A. de C.V.. Todos los derechos reservados.<br/>\r\n        Este es un mensaje automático, por favor no responda a este correo.\r\n      </div>\r\n\r\n    </div>\r\n  </div>\r\n</body>\r\n</html>", Valor2 = "", Valor3 = "", editable = 1, lectura = 1, EmpresaClienteId = clientCompany.Id, TipoConfiguracion = new Guid("E1A2B3C4-D5E6-7F89-A0B1-C2D3E4F50001"), UsuarioCreadorId = currentUserId},
                        new ConfiguracionesReqDTO(){ NombreParametro = "Plantilla correo Wallet", ValorGuid = null, Valor1 = "<!DOCTYPE html>\r\n<html lang=\"es\">\r\n<head>\r\n  <meta charset=\"UTF-8\" />\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />\r\n  <title>Código de Acceso - Plataforma HID</title>\r\n  <style>\r\n    /* -- Reset básico compatible con clientes de correo -- */\r\n    body, table, td, a { -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }\r\n    table, td { mso-table-lspace: 0pt; mso-table-rspace: 0pt; }\r\n    img { -ms-interpolation-mode: bicubic; border: 0; }\r\n    body { margin: 0; padding: 0; background-color: #f4f6f9; font-family: Arial, sans-serif; }\r\n\r\n    /* -- Layout -- */\r\n    .email-outer  { width: 100%; background-color: #f4f6f9; padding: 32px 0; }\r\n    .email-inner  { max-width: 600px; margin: 0 auto; background: #ffffff;\r\n                    border-radius: 8px; overflow: hidden;\r\n                    box-shadow: 0 2px 8px rgba(0,0,0,.08); }\r\n\r\n    /* -- Header -- */\r\n    .email-header { \r\n      background: #002366;\r\n      text-align: center;\r\n      padding: 14px;\r\n    }\r\n    .email-logo   { \r\n      display: inline-block;\r\n      background: white;\r\n      border-radius: 4px;\r\n      padding: 3px 12px;\r\n      font-size: 11px;\r\n      font-weight: 700;\r\n      color: #002366;\r\n    }\r\n\r\n    /* -- Body -- */\r\n    .email-body   { \r\n      padding: 16px 18px;\r\n\r\n      h2 {\r\n          font-size: 13px;\r\n          color: #002366;\r\n          margin: 0 0 6px;\r\n\r\n          em {\r\n              font-style: normal;\r\n              color: #FF6F00;\r\n          }\r\n      }\r\n\r\n      p {\r\n          font-size: 11px;\r\n          color: #444;\r\n          line-height: 1.5;\r\n          margin: 0 0 6px;\r\n      }\r\n    }\r\n\r\n    /* -- Secciones / pasos -- */\r\n    .ewt-email-section {\r\n      margin: 10px 0;\r\n      padding: 9px 11px;\r\n      background: #FFF4E6;\r\n      border-left: 4px solid #FF6F00;\r\n\r\n      h3 {\r\n        font-size: 11px;\r\n        font-weight: 700;\r\n        color: #002366;\r\n        margin: 0 0 4px;\r\n      }\r\n\r\n      p {\r\n          font-size: 11px;\r\n          color: #555;\r\n          margin: 0;\r\n      }\r\n    };\r\n    .step-block   { margin: 24px 0; padding: 20px 24px;\r\n                    border-left: 4px solid #0d6efd; background: #f8faff;\r\n                    border-radius: 0 6px 6px 0; }\r\n    .step-block h3 { margin: 0 0 8px; font-size: 16px; color: #1a3a5c; }\r\n    .step-block p  { margin: 0; font-size: 14px; color: #555; }\r\n\r\n    /* -- Botones tiendas -- */\r\n    .store-row    { margin-top: 14px; }\r\n    .store-btn    { display: inline-block; margin-right: 10px; padding: 8px 18px;\r\n                    background: #1a3a5c; color: #ffffff; border-radius: 20px;\r\n                    font-size: 13px; text-decoration: none; }\r\n\r\n    /* -- QR -- */\r\n    .qr-wrapper   { text-align: center; margin: 16px 0; }\r\n    .qr-wrapper img { width: 130px; height: 130px; }\r\n    .qr-caption   { text-align: center; font-size: 13px; color: #888; margin-top: 6px; }\r\n\r\n    /* -- Código de invitación -- */\r\n    .invite-code  { text-align: center; font-size: 28px; font-weight: 700;\r\n                    letter-spacing: 4px; color: #002366;\r\n                    background: #eef4ff; border: 2px dashed #002366;\r\n                    border-radius: 8px; padding: 16px; margin: 16px 0; }\r\n\r\n    /* -- Contacto -- */\r\n    .contact-box  { \r\n      background: #F8F9FF;\r\n      padding: 9px;\r\n      border: 2px solid #002366;\r\n      border-radius: 4px;\r\n      margin: 10px 0;\r\n      font-size: 10px;\r\n      color: #002366;\r\n      line-height: 1.8;\r\n      text-align: center;\r\n    }\r\n\r\n    /* -- Footer -- */\r\n    .email-footer { \r\n      background: #002366;\r\n      color: white;\r\n      padding: 10px;\r\n      text-align: center;\r\n      font-size: 9px;\r\n      line-height: 1.6;\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"email-outer\">\r\n    <div class=\"email-inner\">\r\n\r\n      <!-- Encabezado -->\r\n      <div class=\"email-header\">\r\n        <div class=\"email-logo\">CRC de México®, S.A. de C.V.</div>\r\n      </div>\r\n\r\n      <!-- Cuerpo -->\r\n      <div class=\"email-body\">\r\n        <h2>Estimado(a): <em>{{destinatario}}</em>,</h2>\r\n        <p>Gracias por utilizar nuestros servicios. Aquí están los detalles de su acceso:</p>\r\n\r\n        <!-- Paso 1 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 1. Descarga nuestra App</h3>\r\n          <p>Accede fácilmente desde tu dispositivo móvil descargando la aplicación.</p>\r\n          <div class=\"store-row\">\r\n            <a href=\"#\" class=\"store-btn\">&#9654; Google Play</a>\r\n            <a href=\"#\" class=\"store-btn\">&#9654; App Store</a>\r\n          </div>\r\n        </div>\r\n\r\n        <!-- Paso 2 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 2. Para acceder</h3>\r\n          <p>Abre la app, selecciona 'Continuar como invitado', presiona el ícono de QR y escanea el código.</p>\r\n          <div class=\"qr-wrapper\">\r\n            <img src=\"data:image/svg+xml;base64,{{qrBase64}}\" alt=\"Código QR de acceso\" />\r\n          </div>\r\n          <p class=\"qr-caption\">Escanea este código QR para validar tu acceso.</p>\r\n        </div>\r\n\r\n        <!-- Paso 3 -->\r\n        <div class=\"ewt-email-section\">\r\n          <h3>Paso 3. Alternativa - Código manual</h3>\r\n          <p>Copia el código, pégalo en 'Canjear código' y presiona 'Canjear'. Mantén la app en vista de credenciales frente al lector.</p>\r\n          <div class=\"invite-code\">{{codigoInvitacion}}</div>\r\n          <p style=\"font-size:13px;color:#888;text-align:center;\">\r\n            Válido hasta: {{fechaExpiracion}}\r\n          </p>\r\n        </div>\r\n\r\n        <!-- Datos de contacto -->\r\n        <div class=\"contact-box\">\r\n          &#128222; <strong>Tel:</strong> +52 (443) 340 0992<br/>\r\n          &#128231; <strong>Email:</strong> omorales@crcdemexico.com.mx<br/>\r\n          &#128205; <strong>Dirección:</strong> Lic. Antonio del Moral 45, Nueva Chapultepec, 58280 Morelia, Mich.\r\n        </div>\r\n      </div>\r\n\r\n      <!-- Pie -->\r\n      <div class=\"email-footer\">\r\n        &copy; 2026 CRC de México®, S.A. de C.V.. Todos los derechos reservados.<br/>\r\n        Este es un mensaje automático, por favor no responda a este correo.\r\n      </div>\r\n\r\n    </div>\r\n  </div>\r\n</body>\r\n</html>", Valor2 = "", Valor3 = "", editable = 1, lectura = 1, EmpresaClienteId = clientCompany.Id, TipoConfiguracion = new Guid("0078A82D-44C5-4EA9-9AD7-61FF6578BACF"), UsuarioCreadorId = currentUserId}
                    };
                    if (settingCommon.Count() != 0)
                    {
                        await _configuracionService.CreateSettingsForCompany(settingCommon, clientCompany.Id, currentUserId);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateWithHID(EmpresaCliente clientCompany, List<ConfiguracionesReqDTO>? settings, Guid usuarioActualId)
        {
            try
            {
                if (usuarioActualId == Guid.Empty) return false;

                // Actualizar datos de la empresa
                EmpresaCliente clientCompanyUpdate = await _unitOfWork.EmpresaClienteRepository.GetById(clientCompany.Id);
                if (clientCompanyUpdate == null) return false;

                clientCompanyUpdate.RazonSocial = clientCompany.RazonSocial;
                clientCompanyUpdate.RFC = clientCompany.RFC;
                clientCompanyUpdate.TelefonoEmpresa = clientCompany.TelefonoEmpresa;
                clientCompanyUpdate.TelefonoMovil = clientCompany.TelefonoMovil;
                clientCompanyUpdate.CorreoElectronico = clientCompany.CorreoElectronico;
                clientCompanyUpdate.UsaCredencialesHID = clientCompany.UsaCredencialesHID;
                clientCompanyUpdate.PaisId = clientCompany.PaisId;
                clientCompanyUpdate.EstadoId = clientCompany.EstadoId;
                clientCompanyUpdate.CiudadId = clientCompany.CiudadId;
                clientCompanyUpdate.FechaModificacion = DateTime.UtcNow;
                clientCompanyUpdate.UsuarioModificadorId = usuarioActualId;

                _unitOfWork.EmpresaClienteRepository.Update(clientCompanyUpdate);
                await _unitOfWork.SaveChangesAsync();

                // Si desactivó HID → desactivar todas sus configuraciones
                if (clientCompany.UsaCredencialesHID == 2)
                {
                    await _configuracionService.DeleteAllSettingsByCompany(clientCompany.Id);
                    return true;
                }

                // Si activó HID con configuraciones → actualizar cada una
                if (settings != null && settings.Any())
                {
                    var listUpdate = settings.Select(c => new ConfiguracionesListReqDTO
                    {
                        Id = c.Id,
                        Valor1 = c.Valor1
                    }).ToList();

                    await _configuracionService.Update(listUpdate);
                }

                var emailHID = await _configuracionService.GetByTypeSettingAndCompanyId(new Guid("E1A2B3C4-D5E6-7F89-A0B1-C2D3E4F50001"), clientCompanyUpdate.Id);
                if (emailHID == null)
                {
                    var emailHIDConfig = new Configuraciones
                    {
                        NombreParametro = "Plantilla correo HID",
                        ValorGuid = null,
                        Valor1 = "",
                        Valor2 = "",
                        Valor3 = "",
                        editable = 1,
                        lectura = 1,
                        EmpresaClienteId = clientCompany.Id,
                        TipoConfiguracion = new Guid("E1A2B3C4-D5E6-7F89-A0B1-C2D3E4F50001"),
                        UsuarioCreadorId = usuarioActualId
                    };
                    await _configuracionService.Create(emailHIDConfig, usuarioActualId);
                }

                var emailWallet = await _configuracionService.GetByTypeSettingAndCompanyId(new Guid("0078A82D-44C5-4EA9-9AD7-61FF6578BACF"), clientCompanyUpdate.Id);
                if (emailWallet == null)
                {
                    var emailWalletConfig = new Configuraciones
                    {
                        NombreParametro = "Plantilla correo Wallet",
                        ValorGuid = null,
                        Valor1 = "",
                        Valor2 = "",
                        Valor3 = "",
                        editable = 1,
                        lectura = 1,
                        EmpresaClienteId = clientCompany.Id,
                        TipoConfiguracion = new Guid("0078A82D-44C5-4EA9-9AD7-61FF6578BACF"),
                        UsuarioCreadorId = usuarioActualId
                    };
                    await _configuracionService.Create(emailWalletConfig, usuarioActualId);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Update(EmpresaCliente clientCompany, Guid usuarioActualId)
        {
            try
            {
                EmpresaCliente clientCompanyUpdate = await _unitOfWork.EmpresaClienteRepository.GetById(clientCompany.Id);
                clientCompanyUpdate.RazonSocial = clientCompany.RazonSocial;
                clientCompanyUpdate.RFC = clientCompany.RFC;
                clientCompanyUpdate.TelefonoEmpresa = clientCompany.TelefonoEmpresa;
                clientCompanyUpdate.TelefonoMovil = clientCompany.TelefonoMovil;
                clientCompanyUpdate.CorreoElectronico = clientCompany.CorreoElectronico;
                clientCompanyUpdate.UsaCredencialesHID = clientCompany.UsaCredencialesHID;

                clientCompanyUpdate.FechaModificacion = DateTime.Now;
                clientCompanyUpdate.UsuarioModificadorId = usuarioActualId;

                _unitOfWork.EmpresaClienteRepository.Update(clientCompanyUpdate);
                await _unitOfWork.SaveChangesAsync();

                if (clientCompany.UsaCredencialesHID == 2)
                {
                    return await _configuracionService.DeactivateAllSettingByCompany(clientCompany.Id, usuarioActualId);
                }
                else
                {
                    return await _configuracionService.CreateInitialSettingsForCompany(clientCompany.Id, usuarioActualId);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<EmpresaCliente?> GetByRFC(string rfc)
        {
            try
            {
                EmpresaCliente clientCompany = await _unitOfWork.EmpresaClienteRepository.GetCompanyClient(l => l.RFC == rfc);
                return clientCompany;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<EmpresaCliente?> GetByRazonSocial(string socialReason)
        {
            try
            {
                EmpresaCliente clientCompany = await _unitOfWork.EmpresaClienteRepository.GetCompanyClient(l => l.RazonSocial == socialReason);
                return clientCompany;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CompanyClientWithSetting> GetWithSetting(Guid companyClientId)
        {
            try
            {
                var data = await _unitOfWork.EmpresaClienteRepository.GetCompanyClientWithSetting(companyClientId);
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CompanyWithSettingDecrypt> GetWithSettingEncrypted(Guid companyClientId)
        {
            try
            {
                var data = await _unitOfWork.EmpresaClienteRepository.GetCompanyWithSettingEncrypted(companyClientId);
                if (data == null)
                    return null;

                var result = new CompanyWithSettingDecrypt
                {
                    Id = data.Id,
                    RazonSocial = data.RazonSocial,
                    RFC = data.RFC,
                    TelefonoEmpresa = data.TelefonoEmpresa,
                    TelefonoMovil = data.TelefonoMovil,
                    CorreoElectronico = data.CorreoElectronico,
                    UsaCredencialesHID = data.UsaCredencialesHID,
                    UsaCredencialesWallet = data.UsaCredencialesWallet,
                    Pais = data.Pais,
                    PaisEstado = data.PaisEstado,
                    Ciudad = data.Ciudad,

                    CredencialesHID = data.CredencialesHID,
                    CredencialesWallet = data.CredencialesWallet
                };

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
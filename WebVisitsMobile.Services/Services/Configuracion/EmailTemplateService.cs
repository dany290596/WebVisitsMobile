using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Models.Configuracion.Email;
using WebVisitsMobile.Services.Interfaces.Configuracion;

namespace WebVisitsMobile.Services.Services.Configuracion
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmailTemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ── GET — plantilla cruda ──────────────────────────────────────────────
        public async Task<EmailResponseDTO?> GetTemplate(Guid companyId)
        {
            var setting = await _unitOfWork.ConfiguracionesRepository.GetSetting(
                s => s.TipoConfiguracion == EmailTemplateConstants.EMAIL_TEMPLATE_HID
                  && s.EmpresaClienteId == companyId
                  && s.Estado == 1);

            if (setting == null) return null;

            return new EmailResponseDTO
            {
                Id = setting.Id,
                TemplateHtml = setting.Valor1 ?? string.Empty,
                UpdatedAt = setting.FechaModificacion ?? setting.FechaCreacion
            };
        }

        // ── GET /preview — plantilla con datos estáticos inyectados ───────────
        public async Task<string?> GetRenderedPreview(Guid companyId)
        {
            var dto = await GetTemplate(companyId);
            if (dto == null) return null;

            // Datos de muestra — el Servicio Windows los reemplaza en producción
            var rendered = dto.TemplateHtml
                .Replace(EmailTemplateConstants.PH_DESTINATARIO, "Juan Pérez García")
                .Replace(EmailTemplateConstants.PH_CODIGO_INVITACION, "HID-2025-DEMO-9472")
                .Replace(EmailTemplateConstants.PH_QR_BASE64, GetDemoQrBase64())
                .Replace(EmailTemplateConstants.PH_FECHA_EXPIRACION, DateTime.Now.AddDays(7)
                                                                           .ToString("dd/MM/yyyy"));
            return rendered;
        }

        // ── POST — crear o actualizar ──────────────────────────────────────────
        public async Task<bool> SaveTemplate(Guid companyId, Guid userId, string templateHtml)
        {
            try
            {
                var existing = await _unitOfWork.ConfiguracionesRepository.GetSetting(
                    s => s.TipoConfiguracion == EmailTemplateConstants.EMAIL_TEMPLATE_HID
                      && s.EmpresaClienteId == companyId);

                if (existing != null)
                {
                    // UPDATE
                    existing.Valor1 = templateHtml;
                    existing.FechaModificacion = DateTime.UtcNow;
                    existing.UsuarioModificadorId = userId;

                    // Si estaba inactivo, lo reactivamos
                    if (existing.Estado != 1)
                    {
                        existing.Estado = 1;
                        existing.FechaReactivacion = DateTime.UtcNow;
                        existing.UsuarioReactivadorId = userId;
                    }

                    _unitOfWork.ConfiguracionesRepository.Update(existing);
                }
                else
                {
                    // INSERT
                    var newSetting = new Configuraciones
                    {
                        Id = Guid.NewGuid(),
                        TipoConfiguracion = EmailTemplateConstants.EMAIL_TEMPLATE_HID,
                        NombreParametro = EmailTemplateConstants.EMAIL_TEMPLATE_HID_NAME,
                        Valor1 = templateHtml,
                        EmpresaClienteId = companyId,
                        UsuarioCreadorId = userId,
                        FechaCreacion = DateTime.UtcNow,
                        Estado = 1,
                        editable = 1,
                        lectura = 0
                    };

                    await _unitOfWork.ConfiguracionesRepository.Add(newSetting);
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ── Helper: QR de muestra en base64 (pequeño placeholder SVG) ─────────
        private static string GetDemoQrBase64()
        {
            // SVG simple que simula un QR — en producción el Servicio Windows
            // inyecta el base64 real del QR generado por HID Origo
            const string svgQr = @"<svg xmlns='http://www.w3.org/2000/svg' width='120' height='120' viewBox='0 0 120 120'>
              <rect width='120' height='120' fill='white'/>
              <rect x='10' y='10' width='40' height='40' fill='black'/>
              <rect x='15' y='15' width='30' height='30' fill='white'/>
              <rect x='20' y='20' width='20' height='20' fill='black'/>
              <rect x='70' y='10' width='40' height='40' fill='black'/>
              <rect x='75' y='15' width='30' height='30' fill='white'/>
              <rect x='80' y='20' width='20' height='20' fill='black'/>
              <rect x='10' y='70' width='40' height='40' fill='black'/>
              <rect x='15' y='75' width='30' height='30' fill='white'/>
              <rect x='20' y='80' width='20' height='20' fill='black'/>
              <rect x='55' y='55' width='10' height='10' fill='black'/>
              <rect x='70' y='55' width='10' height='10' fill='black'/>
              <rect x='85' y='55' width='10' height='10' fill='black'/>
              <rect x='55' y='70' width='10' height='10' fill='black'/>
              <rect x='85' y='70' width='10' height='10' fill='black'/>
              <rect x='70' y='85' width='10' height='10' fill='black'/>
              <rect x='100' y='70' width='10' height='10' fill='black'/>
            </svg>";

            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(svgQr));
        }
    }

    /// <summary>
    /// GUID fijo que identifica el registro de Configuraciones
    /// que almacena la plantilla HTML del correo HID.
    ///
    /// Se usa como TipoConfiguracion en la tabla Configuraciones.
    /// Genera uno nuevo con: Guid.NewGuid() — el de abajo ya está fijo
    /// para que sea consistente en todos los ambientes.
    /// </summary>
    public static class EmailTemplateConstants
    {
        /// <summary>
        /// TipoConfiguracion para la plantilla HTML del correo Wallet/HID.
        /// Valor: E1A2B3C4-D5E6-7F89-A0B1-C2D3E4F50001
        /// </summary>
        public static readonly Guid EMAIL_TEMPLATE_HID =
            Guid.Parse("E1A2B3C4-D5E6-7F89-A0B1-C2D3E4F50001");

        /// <summary>
        /// Nombre del parámetro tal como queda en NombreParametro.
        /// </summary>
        public const string EMAIL_TEMPLATE_HID_NAME = "Plantilla correo HID Wallet";

        // ── Placeholders que el Servicio Windows reemplaza al procesar cada tarea ──
        public const string PH_DESTINATARIO = "{{destinatario}}";
        public const string PH_CODIGO_INVITACION = "{{codigoInvitacion}}";
        public const string PH_QR_BASE64 = "{{qrBase64}}";
        public const string PH_FECHA_EXPIRACION = "{{fechaExpiracion}}";
    }
}
using Microsoft.Extensions.Hosting;
using QRCoder;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using WebVisitsMobile.Models.HID.UserHID;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.Interfaces.Organizacion.Email;

namespace WebVisitsMobile.Services.Services.Organizacion.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguracionService _configuracionService;
        private readonly IHostEnvironment _hostEnvironment;

        public EmailService(
            IConfiguracionService configuracionService,
            IHostEnvironment hostEnvironment
            )
        {
            _configuracionService = configuracionService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<bool> SendEmailUserHID(UserHIDReqDTO userHID)
        {
            var configurationEmail = await _configuracionService.GetById(new Guid("0BBB9E2B-C0BB-42DC-AC64-372D03040566"));
            if (configurationEmail == null)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(configurationEmail.Valor1))
            {
                Console.WriteLine("El destinatario no puede estar vacío.");
                return false;
            }

            if (!IsValidEmail(configurationEmail.Valor1))
            {
                Console.WriteLine("La dirección de correo no es válida.");
                return false;
            }

            try
            {
                string logoCid = "logoImageCID";
                string cuerpoMensaje = BuildEmailUserHIDBody(userHID, configurationEmail.Valor1, _hostEnvironment.ContentRootPath, logoCid);

                string logoPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "image", "Logo.png");
                byte[] logoBytes = File.ReadAllBytes(logoPath);

                using var cliente = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential("danielbr.estatus@gmail.com", "nhknszfneckejbca"),
                    Timeout = 15000
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress("danielbr.estatus@gmail.com", "CRC de México"),
                    Subject = "Código de Acceso - Plataforma HID",
                    IsBodyHtml = true
                };
                mensaje.To.Add(configurationEmail.Valor1);

                var htmlView = AlternateView.CreateAlternateViewFromString(cuerpoMensaje, null, MediaTypeNames.Text.Html);

                var logoImageResource = new LinkedResource(new MemoryStream(logoBytes), "image/png")
                {
                    ContentId = logoCid,
                    TransferEncoding = TransferEncoding.Base64,
                    ContentType = new ContentType("image/png"),
                };

                htmlView.LinkedResources.Add(logoImageResource);

                mensaje.AlternateViews.Add(htmlView);

                cliente.Send(mensaje);

                Console.WriteLine("Correo enviado correctamente a " + configurationEmail.Valor1);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error general enviando correo:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendEmailInvitationCodeHID(string destinatario, string codigo, DateTime expiracion, Guid clientCompanyId)
        {
            var setting = await _configuracionService.GetEmailSetting(clientCompanyId);

            if (string.IsNullOrWhiteSpace(destinatario) || string.IsNullOrWhiteSpace(codigo))
            {
                Console.WriteLine("El destinatario o código no pueden estar vacíos.");
                return false;
            }

            if (!IsValidEmail(destinatario))
            {
                Console.WriteLine("La dirección de correo no es válida.");
                return false;
            }

            try
            {
                string qrCid = "qrImageCID";
                string logoCid = "logoImageCID";
                string googlePlayCid = "googlePlayImageCID";
                string appStoreCid = "appStoreImageCID";

                string cuerpoMensaje = BuildEmailInvitationCodeHIDBody(destinatario, codigo, expiracion, _hostEnvironment.ContentRootPath, qrCid, logoCid, googlePlayCid, appStoreCid);

                byte[] qrBytes = GenerarQrBytes(codigo);

                string logoPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "image", "Logo.png");
                byte[] logoBytes = File.ReadAllBytes(logoPath);

                string googlePlayPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "image", "Android.png");
                byte[] googlePlayBytes = File.ReadAllBytes(googlePlayPath);

                string appStorePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "image", "Apple.png");
                byte[] appStoreBytes = File.ReadAllBytes(appStorePath);

                using var cliente = new SmtpClient(setting.SmtpServer, 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(setting.SenderEmail, setting.Password),
                    Timeout = 15000
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(setting.SenderEmail, "CRC de México"),
                    Subject = "Código de Acceso - Plataforma HID",
                    IsBodyHtml = true
                };
                mensaje.To.Add(destinatario);

                var htmlView = AlternateView.CreateAlternateViewFromString(cuerpoMensaje, null, MediaTypeNames.Text.Html);

                var qrImageResource = new LinkedResource(new MemoryStream(qrBytes), "image/png")
                {
                    ContentId = qrCid,
                    TransferEncoding = TransferEncoding.Base64,
                    ContentType = new ContentType("image/png"),
                    ContentLink = new Uri("cid:" + qrCid)
                };
                htmlView.LinkedResources.Add(qrImageResource);

                var logoImageResource = new LinkedResource(new MemoryStream(logoBytes), "image/png")
                {
                    ContentId = logoCid,
                    TransferEncoding = TransferEncoding.Base64,
                    ContentType = new ContentType("image/png"),
                };

                htmlView.LinkedResources.Add(logoImageResource);

                var googlePlayResource = new LinkedResource(new MemoryStream(googlePlayBytes), "image/png")
                {
                    ContentId = googlePlayCid,
                    TransferEncoding = TransferEncoding.Base64,
                    ContentType = new ContentType("image/png")
                };
                htmlView.LinkedResources.Add(googlePlayResource);

                var appStoreResource = new LinkedResource(new MemoryStream(appStoreBytes), "image/png")
                {
                    ContentId = appStoreCid,
                    TransferEncoding = TransferEncoding.Base64,
                    ContentType = new ContentType("image/png")
                };
                htmlView.LinkedResources.Add(appStoreResource);

                mensaje.AlternateViews.Add(htmlView);

                cliente.Send(mensaje);

                Console.WriteLine("Correo enviado correctamente a " + destinatario);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error general enviando correo:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                return await Task.FromResult(false);
            }
        }

        private static string BuildEmailInvitationCodeHIDBody(string destinatario, string codigo, DateTime expiracion, string contentRootPath, string qrCid, string logoCid, string googlePlayCid, string appStoreCid)
        {
            int diasRestantes = (int)Math.Ceiling((expiracion - DateTime.Now).TotalDays);
            string textoDias = diasRestantes > 0 ? $"{diasRestantes} días" : "Último día";

            return $@"
                    <html>
                    <head>
                      <meta charset='UTF-8'>
                      <style>
                        body {{
                          font-family: Arial, sans-serif;
                          margin: 0;
                          padding: 0;
                          background-color: #FFF4E6;
                        }}
                        .main {{
                          width: 100%;
                          background-color: #FFF4E6;
                          padding: 20px 0;
                        }}
                        .content-wrapper {{
                          width: 100%;
                          max-width: 600px;
                          margin: 0 auto;
                          background-color: #ffffff;
                          border: 1px solid #EAECEE;
                        }}
                        .header {{
                          background-color: #002366;
                          text-align: center;
                          padding: 20px;
                          border-radius: 8px 8px 0 0;
                        }}
                        .logo {{
                          max-width: 200px;
                        }}
                        .content {{
                          padding: 30px;
                        }}
                        .code {{
                          font-size: 24px;
                          color: #002366;
                          font-weight: bold;
                          margin: 20px 0;
                          padding: 15px;
                          background-color: #FFF4E6;
                          text-align: center;
                          border-radius: 5px;
                          border: 2px solid #FF6F00;
                        }}
                        .expira-box {{
                          background-color: #F8F9FF;
                          padding: 15px;
                          border-radius: 5px;
                          border: 2px solid #002366;
                          margin: 20px 0;
                          text-align: center;
                        }}
                        .footer {{
                          background-color: #002366;
                          color: white;
                          padding: 15px;
                          text-align: center;
                          font-size: 12px;
                          border-radius: 0 0 8px 8px;
                        }}
                        .servicios {{
                          margin: 20px 0;
                          padding: 15px;
                          background-color: #FFF4E6;
                          border-left: 4px solid #FF6F00;
                          border-radius: 4px;
                        }}
                        .dias-vigencia {{
                          color: #002366;
                          font-weight: bold;
                          font-size: 18px;
                        }}
                        .color-naranja {{ color: #FF6F00; }}
                        .color-azul {{ color: #002366; }}
                      </style>
                    </head>
                    <body>
                      <table class='main' width='100%' cellspacing='0' cellpadding='0'>
                        <tr>
                          <td align='center'>
                            <table class='content-wrapper' cellspacing='0' cellpadding='0'>
                              <tr>
                                <td class='header'>
                                  <img src='cid:{logoCid}' 
     width='200' 
     style='width:200px; height:auto; max-width:200px; display:block; margin:auto;' class='logo' alt='CRC México'>
                                </td>
                              </tr>
                              <tr>
                                <td class='content'>
                                  <h2 class='color-azul'>Estimado(a): {destinatario},</h2>
                                  <p>Gracias por utilizar nuestros servicios. Aquí están los detalles de su acceso:</p>

                    <div class='servicios'>
                      <h3 class='color-azul'>Paso 1. Descarga nuestra App</h3>
                      <p>Accede fácilmente desde tu dispositivo móvil:</p>
                      <p class='color-azul' style='text-align: center;'>
                        <a href='https://play.google.com/store/apps/details?id=com.crc.webvisitsmeeting&pcampaignid=web_share' target='_blank'>
                          <img src='cid:{googlePlayCid}' alt='Android' 
         width='80' 
         height='80'
         style='display:block; width:80px; height:80px; border:0; outline:none; text-decoration:none; max-width:80px;'
         >
                        </a>
                        <a href='https://apps.apple.com/mx/app/webvisits-meeting/id6742790255' target='_blank'>
                          <img src='cid:{appStoreCid}' alt='iOS' 
         width='80' 
         height='80'
         style='display:block; width:80px; height:80px; border:0; outline:none; text-decoration:none; max-width:80px;'
         >
                        </a>
                      </p>
                    </div>                                  

                                  <div class='servicios'>
                                    <h3 class='color-azul'>Paso 2. Para acceder</h3>
                                    <ul>
                                      <li><span class='color-naranja'>▶</span> Abra la aplicación</li>
                                      <li><span class='color-naranja'>▶</span> Seleccione 'Continuar como invitado'</li>
                                      <li><span class='color-naranja'>▶</span> Presione el ícono de QR</li>
                                      <li><span class='color-naranja'>▶</span> Escanee el siguiente código:</li>
                                      <li>
                                       <img src='cid:{qrCid}' alt='QR Code' 
         width='120' 
         height='120'
         style='display:block; width:120px; height:120px; border:0; outline:none; text-decoration:none; max-width:120px;' /><div class='color-azul'>Escanea este código QR para validar tu acceso.</div><div class='color-azul'></div>
                                      </li>
                                    </ul>
                                  </div>

                                  <div class='servicios'>
                                    <h3 class='color-azul'>Paso 3. Alternativa - Código manual: </h3>
                                    <ul>
                                      <li><span class='color-naranja'>▶</span> Copie este código: {codigo}</li>
                                      <li><span class='color-naranja'>▶</span> Péguelo en el campo en 'Canjear código' y presioné 'Canjear'</li>
                                      <li><span class='color-naranja'>▶</span> Nota: Para acceder, mantenga la app en la vista de credenciales y presente el dispositivo móvil frente al lector*
     - Si no se muestra la credencial, presione 'Refrescar'</li>
                                    </ul>
                                  </div>                                  

                                  <p class='color-azul'>Para cualquier consulta, contáctenos:</p>

                                  <div class='expira-box'>
                                    <p class='color-azul'>
                                      📞 <strong>Tel:</strong> +52 (443) 340 0992<br>
                                      📧 <strong>Email:</strong> omorales@crcdemexico.com.mx<br>
                                      📍 <strong>Dirección:</strong> Lic. Antonio del Moral 45, Nueva Chapultepec, 58280 Morelia, Mich.
                                    </p>
                                  </div>

                    

                                </td>
                              </tr>
                              <tr>
                                <td class='footer'>
                                  © {DateTime.Now.Year} CRC de México®, S.A. de C.V. Todos los derechos reservados.<br>
                                  Este es un mensaje automático, por favor no responda a este correo.
                                </td>
                              </tr>
                            </table>
                          </td>
                        </tr>
                      </table>
                    </body>
                    </html>";
        }

        private static string BuildEmailUserHIDBody(UserHIDReqDTO userHID, string email, string contentRootPath, string logoCid)
        {
            return $@"
                    <html>
                    <head>
                      <meta charset='UTF-8'>
                      <style>
                        body {{
                          font-family: Arial, sans-serif;
                          margin: 0;
                          padding: 0;
                          background-color: #FFF4E6;
                        }}
                        .main {{
                          width: 100%;
                          background-color: #FFF4E6;
                          padding: 20px 0;
                        }}
                        .content-wrapper {{
                          width: 100%;
                          max-width: 600px;
                          margin: 0 auto;
                          background-color: #ffffff;
                          border: 1px solid #EAECEE;
                        }}
                        .header {{
                          background-color: #002366;
                          text-align: center;
                          padding: 20px;
                          border-radius: 8px 8px 0 0;
                        }}
                        .logo {{
                          max-width: 200px;
                        }}
                        .content {{
                          padding: 30px;
                        }}
                        .code {{
                          font-size: 24px;
                          color: #002366;
                          font-weight: bold;
                          margin: 20px 0;
                          padding: 15px;
                          background-color: #FFF4E6;
                          text-align: center;
                          border-radius: 5px;
                          border: 2px solid #FF6F00;
                        }}
                        .expira-box {{
                          background-color: #F8F9FF;
                          padding: 15px;
                          border-radius: 5px;
                          border: 2px solid #002366;
                          margin: 20px 0;
                          text-align: center;
                        }}
                        .footer {{
                          background-color: #002366;
                          color: white;
                          padding: 15px;
                          text-align: center;
                          font-size: 12px;
                          border-radius: 0 0 8px 8px;
                        }}
                        .servicios {{
                          margin: 20px 0;
                          padding: 15px;
                          background-color: #ffe8e6;
                          border-left: 4px solid #FF6F00;
                          border-radius: 4px;
                        }}
                        .dias-vigencia {{
                          color: #002366;
                          font-weight: bold;
                          font-size: 18px;
                        }}
                        .color-naranja {{ color: #FF6F00; }}
                        .color-azul {{ color: #002366; }}
                      </style>
                    </head>
                    <body>
                      <table class='main' width='100%' cellspacing='0' cellpadding='0'>
                        <tr>
                          <td align='center'>
                            <table class='content-wrapper' cellspacing='0' cellpadding='0'>
                              <tr>
                                <td class='header'>
                                  <img src='cid:{logoCid}' style='max-width:200px;margin:10px auto;' class='logo' alt='CRC México'>
                                </td>
                              </tr>
                              <tr>
                                <td class='content'>
                                  <h2 class='color-azul'>Estimado(a): {email},</h2>
                                  <p>Gracias por utilizar nuestros servicios. Aquí están los detalles de su acceso:</p>                                     


                                  <div class='servicios'>
                                    <h3 class='color-azul'>Se ha detectado una inconsistencia de datos con respecto al siguiente usuario</h3>
                                    <ul>
                                      <li><span class='color-naranja'>▶</span> Usuario: {userHID.Nombre + " " + userHID.Apellidos}</li>
                                      <li><span class='color-naranja'>▶</span> Correo electrónico: {userHID.Email}</li>
                                      <li><span class='color-naranja'>▶</span> Teléfono: {userHID.Telefono}</li>
                                      <li><span class='color-naranja'>▶</span> Sitio: {userHID.Site} </li>
                                      <li><span class='color-naranja'>▶</span> Alerta: {userHID.Alert} </li>
                                      <li><span class='color-naranja'>▶</span> Número de licencias: {userHID.LicenseCount} </li>
                                    </ul>

                                    <p>
                                      Al intentar darlo de alta, el proveedor nos indicó que el usuario ya se encuentra registrado en su plataforma, 
                                      lo que impide completar el proceso correctamente desde nuestro lado.
                                    </p>
                                    <p>
                                      Solicitamos su apoyo para validar esta situación y determinar los pasos a seguir:
                                    </p>
                                    <ul>
                                      <li>✔ Confirmar si el usuario ya tiene un registro activo.</li>
                                      <li>✔ Indicar si es posible vincular el alta actual al registro existente.</li>
                                      <li>✔ O bien, proporcionar instrucciones para proceder con la actualización o eliminación del posible duplicado.</li>
                                    </ul>
                                    <p>
                                      Quedamos atentos a sus comentarios para resolver este caso lo antes posible.
                                    </p>
                                  </div>

                                  <p class='color-azul'>Para cualquier consulta, contáctenos:</p>
                                  <div class='expira-box'>
                                    <p class='color-azul'>
                                      📞 <strong>Tel:</strong> +52 (443) 340 0992<br>
                                      📧 <strong>Email:</strong> contacto@crc-mexico.com<br>
                                      📍 <strong>Dirección:</strong> Lic. Antonio del Moral 45, Nueva Chapultepec, 58280 Morelia, Mich.
                                    </p>
                                  </div>

                                </td>
                              </tr>
                              <tr>
                                <td class='footer'>
                                  © {DateTime.Now.Year} CRC de México®, S.A. de C.V. Todos los derechos reservados.<br>
                                  Este es un mensaje automático, por favor no responda a este correo.
                                </td>
                              </tr>
                            </table>
                          </td>
                        </tr>
                      </table>
                    </body>
                    </html>";
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static byte[] GenerarQrBytes(string texto)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        public async Task<bool> SendContractReportByEmail(byte[] pdfBytes, string destinatario, string asunto)
        {
            if (string.IsNullOrWhiteSpace(destinatario))
            {
                Console.WriteLine("El destinatario no puede estar vacío.");
                return false;
            }

            if (!IsValidEmail(destinatario))
            {
                Console.WriteLine("La dirección de correo no es válida.");
                return false;
            }

            try
            {
                string logoCid = "logoImageCID";

                // Construir el cuerpo del email
                string cuerpoMensaje = BuildContractReportEmailBody(_hostEnvironment.ContentRootPath, logoCid);

                // Leer el logo
                string logoPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "image", "Logo.png");
                byte[] logoBytes = File.ReadAllBytes(logoPath);

                using var cliente = new SmtpClient("smtp.titan.email", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential("cloud@crcdemexico.com.mx", "cL0Ud.2024"),
                    Timeout = 15000
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress("cloud@crcdemexico.com.mx", "CRC de México"),
                    Subject = asunto,
                    IsBodyHtml = true
                };
                mensaje.To.Add(destinatario);

                // Crear la vista HTML
                var htmlView = AlternateView.CreateAlternateViewFromString(cuerpoMensaje, null, MediaTypeNames.Text.Html);

                // Agregar el logo como recurso incrustado
                var logoImageResource = new LinkedResource(new MemoryStream(logoBytes), "image/png")
                {
                    ContentId = logoCid,
                    TransferEncoding = TransferEncoding.Base64,
                    ContentType = new ContentType("image/png"),
                };
                htmlView.LinkedResources.Add(logoImageResource);

                mensaje.AlternateViews.Add(htmlView);

                // Adjuntar el PDF
                using var pdfStream = new MemoryStream(pdfBytes);
                var pdfAttachment = new Attachment(pdfStream, "Contrato.pdf", MediaTypeNames.Application.Pdf);
                mensaje.Attachments.Add(pdfAttachment);

                await cliente.SendMailAsync(mensaje);

                Console.WriteLine($"Correo con contrato enviado correctamente a {destinatario}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error general enviando correo con contrato:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        private static string BuildContractReportEmailBody(string contentRootPath, string logoCid)
        {
            return $@"
    <html>
    <head>
      <meta charset='UTF-8'>
      <style>
        body {{
          font-family: Arial, sans-serif;
          margin: 0;
          padding: 0;
          background-color: #FFF4E6;
        }}
        .main {{
          width: 100%;
          background-color: #FFF4E6;
          padding: 20px 0;
        }}
        .content-wrapper {{
          width: 100%;
          max-width: 600px;
          margin: 0 auto;
          background-color: #ffffff;
          border: 1px solid #EAECEE;
        }}
        .header {{
          background-color: #002366;
          text-align: center;
          padding: 20px;
          border-radius: 8px 8px 0 0;
        }}
        .logo {{
          max-width: 200px;
        }}
        .content {{
          padding: 30px;
        }}
        .footer {{
          background-color: #002366;
          color: white;
          padding: 15px;
          text-align: center;
          font-size: 12px;
          border-radius: 0 0 8px 8px;
        }}
        .color-naranja {{ color: #FF6F00; }}
        .color-azul {{ color: #002366; }}
        .info-box {{
          background-color: #F8F9FF;
          padding: 15px;
          border-radius: 5px;
          border: 2px solid #002366;
          margin: 20px 0;
        }}
      </style>
    </head>
    <body>
      <table class='main' width='100%' cellspacing='0' cellpadding='0'>
        <tr>
          <td align='center'>
            <table class='content-wrapper' cellspacing='0' cellpadding='0'>
              <tr>
                <td class='header'>
                  <img src='cid:{logoCid}' style='max-width:200px;margin:10px auto;' class='logo' alt='CRC México'>
                </td>
              </tr>
              <tr>
                <td class='content'>
                  <h2 class='color-azul'>Estimado(a) cliente,</h2>
                  <p>Le informamos que el contrato de su licencia ha sido generado exitosamente.</p>
                  
                  <div class='info-box'>
                    <h3 class='color-azul'>📄 Contrato Adjunto</h3>
                    <p>En el archivo adjunto encontrará el contrato correspondiente a su licencia en formato PDF.</p>
                    <p class='color-naranja'>Por favor, revise cuidadosamente el documento.</p>
                  </div>

                  <p class='color-azul'>Para cualquier consulta, contáctenos:</p>
                  <div class='info-box'>
                    <p class='color-azul'>
                      📞 <strong>Tel:</strong> +52 (443) 340 0992<br>
                      📧 <strong>Email:</strong> contacto@crc-mexico.com<br>
                      📍 <strong>Dirección:</strong> Lic. Antonio del Moral 45, Nueva Chapultepec, 58280 Morelia, Mich.
                    </p>
                  </div>

                </td>
              </tr>
              <tr>
                <td class='footer'>
                  © {DateTime.Now.Year} CRC de México®, S.A. de C.V. Todos los derechos reservados.<br>
                  Este es un mensaje automático, por favor no responda a este correo.
                </td>
              </tr>
            </table>
          </td>
        </tr>
      </table>
    </body>
    </html>";
        }
    }
}
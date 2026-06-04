namespace WebVisitsMobile.Models.Configuracion.Email
{
    public class EmailReqDTO
    {
        /// <summary>
        /// HTML completo de la plantilla.
        /// Puede contener los siguientes placeholders dinámicos:
        ///   {{destinatario}}      → Nombre completo del destinatario
        ///   {{codigoInvitacion}}  → Código de acceso HID
        ///   {{qrBase64}}          → Imagen QR codificada en Base64
        ///   {{fechaExpiracion}}   → Fecha de vencimiento del código
        /// </summary>
        public string TemplateHtml { get; set; } = string.Empty;
    }
}
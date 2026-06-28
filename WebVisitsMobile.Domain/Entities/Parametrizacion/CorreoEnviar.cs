using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Parametrizacion
{
    public class CorreoEnviar : BaseEntity
    {
        public string? De { get; set; }
        public string? Para { get; set; }
        public string? Cc { get; set; }
        public string? Mensaje { get; set; }
        public byte? Enviado { get; set; }
        public byte? Marca { get; set; }
        public string? Asunto { get; set; }
        public string? Qr { get; set; }
        public string? Ics { get; set; }
        public string? OrganizadorCorreo { get; set; }
        public string? QrAcceso { get; set; }
        public Guid? EmpresaClienteId { get; set; }
    }
}
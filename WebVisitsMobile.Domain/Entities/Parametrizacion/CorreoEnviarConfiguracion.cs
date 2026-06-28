using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Parametrizacion
{
    public class CorreoEnviarConfiguracion : BaseEntity
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

        // OAuth
        public ConfiguracionCorreo? Tenant { get; set; }       // Id = 0555A333-7F4A-4BD1-8605-20DB85AB8F9C
        public ConfiguracionCorreo? Client { get; set; }       // Id = EEB83114-7C90-43B9-B4CF-16DF7EAA1B6C
        public ConfiguracionCorreo? ClientSecret { get; set; } // Id = 42220665-C2D5-48AD-9D05-E245C4784650
        public ConfiguracionCorreo? Correo { get; set; }       // Id = FE05BC78-00F5-4EA2-B58E-5FF7C3D60AD8

        // SMTP
        public ConfiguracionCorreo? CorreoSMTP { get; set; }       // 444B364E-D04A-453D-A2CD-ABD3AF06547E
        public ConfiguracionCorreo? ServidorSMTP { get; set; }     // 88454A1C-51A1-4F6D-9F61-8D4ED2F5446E
        public ConfiguracionCorreo? PuertoSMTP { get; set; }       // B0B0B87F-1D4D-41F5-AC65-A9C241E532BC
        public ConfiguracionCorreo? UsuarioSMTP { get; set; }      // 0D0DEF18-E291-4888-9885-5B48DC4035EE
        public ConfiguracionCorreo? PasswordSMTP { get; set; }     // 99022259-4E63-4189-8F80-D65994F6FEB3
        public ConfiguracionCorreo? RequiereSSL { get; set; }      // 0058117E-2B86-4122-80DC-228DCE6A9C46
        public ConfiguracionCorreo? TLS12 { get; set; }            // 7171F864-8663-45A1-BAB5-AB226B458BB1
        public ConfiguracionCorreo? TLS13 { get; set; }            // 0E1119B3-0397-48E5-B0A5-A5E77C73C017
        public ConfiguracionCorreo? ProtocoloCorreo { get; set; }  // 5FFF0424-DAC1-4DD9-9E5C-9CA4225BBD55
    }

    public class ConfiguracionCorreo
    {
        public Guid Id { get; set; }
        public string? NombreParametro { get; set; }
        public string? Valor1 { get; set; }
    }
}
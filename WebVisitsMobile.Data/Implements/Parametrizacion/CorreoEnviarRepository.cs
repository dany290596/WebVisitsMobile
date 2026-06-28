using Microsoft.EntityFrameworkCore;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Parametrizacion;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.Entities.Parametrizacion;

namespace WebVisitsMobile.Data.Implements.Parametrizacion
{
    public class CorreoEnviarRepository : Repository<CorreoEnviar>, ICorreoEnviarRepository
    {
        public CorreoEnviarRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<List<CorreoEnviarConfiguracion>> GetPendingEmailsWithConfig()
        {
            var configuracionIds = new Dictionary<string, Guid>
            {
                // OAuth
                { "Tenant", Guid.Parse("0555A333-7F4A-4BD1-8605-20DB85AB8F9C") },
                { "Client", Guid.Parse("EEB83114-7C90-43B9-B4CF-16DF7EAA1B6C") },
                { "ClientSecret", Guid.Parse("42220665-C2D5-48AD-9D05-E245C4784650") },
                { "Correo", Guid.Parse("FE05BC78-00F5-4EA2-B58E-5FF7C3D60AD8") },

                // SMTP
                { "CorreoSMTP", Guid.Parse("444B364E-D04A-453D-A2CD-ABD3AF06547E") },
                { "ServidorSMTP", Guid.Parse("88454A1C-51A1-4F6D-9F61-8D4ED2F5446E") },
                { "PuertoSMTP", Guid.Parse("B0B0B87F-1D4D-41F5-AC65-A9C241E532BC") },
                { "UsuarioSMTP", Guid.Parse("0D0DEF18-E291-4888-9885-5B48DC4035EE") },
                { "PasswordSMTP", Guid.Parse("99022259-4E63-4189-8F80-D65994F6FEB3") },
                { "RequiereSSL", Guid.Parse("0058117E-2B86-4122-80DC-228DCE6A9C46") },
                { "TLS12", Guid.Parse("7171F864-8663-45A1-BAB5-AB226B458BB1") },
                { "TLS13", Guid.Parse("0E1119B3-0397-48E5-B0A5-A5E77C73C017") },
                { "ProtocoloCorreo", Guid.Parse("5FFF0424-DAC1-4DD9-9E5C-9CA4225BBD55") }
            };

            // Traemos los correos pendientes
            var pendingEmails = await _context.Set<CorreoEnviar>()
                .AsNoTracking()
                .Where(c => c.Enviado == null || c.Enviado != 1)
                .ToListAsync();

            // Traemos todas las configuraciones necesarias
            var configs = await _context.Set<Configuraciones>()
                .AsNoTracking()
                .Where(c => configuracionIds.Values.Contains(c.Id))
                .ToListAsync();

            // Mapear cada correo a un objeto con sus 4 configuraciones
            var resultado = pendingEmails.Select(correo =>
            {
                //ConfiguracionGeneral SafeConfig(string key) =>
                //    configs.FirstOrDefault(c => c.Id == configuracionIds[key] && c.EmpresaId == correo.EmpresaId)
                //    ?? new ConfiguracionGeneral();
                Domain.Entities.Parametrizacion.ConfiguracionCorreo SafeConfig(string key)
                {
                    var config = configs.FirstOrDefault(c =>
                        c.Id == configuracionIds[key]);

                    if (config == null)
                        return new Domain.Entities.Parametrizacion.ConfiguracionCorreo();

                    return new Domain.Entities.Parametrizacion.ConfiguracionCorreo
                    {
                        Id = config.Id,
                        NombreParametro = config.NombreParametro,
                        Valor1 = config.Valor1
                    };
                }

                var correoConfig = new CorreoEnviarConfiguracion
                {
                    De = correo.De,
                    Para = correo.Para,
                    Cc = correo.Cc,
                    Mensaje = correo.Mensaje,
                    Enviado = correo.Enviado,
                    Marca = correo.Marca,
                    Asunto = correo.Asunto,
                    Qr = correo.Qr,
                    Ics = correo.Ics,
                    OrganizadorCorreo = correo.OrganizadorCorreo,
                    QrAcceso = correo.QrAcceso,

                    Id = correo.Id,
                    UsuarioCreadorId = correo.UsuarioCreadorId,
                    UsuarioModificadorId = correo.UsuarioModificadorId,
                    UsuarioBajaId = correo.UsuarioBajaId,
                    UsuarioReactivadorId = correo.UsuarioReactivadorId,
                    FechaCreacion = correo.FechaCreacion,
                    FechaModificacion = correo.FechaModificacion,
                    FechaBaja = correo.FechaBaja,
                    FechaReactivacion = correo.FechaReactivacion,
                    Estado = correo.Estado,
                    EmpresaClienteId = correo.EmpresaClienteId,

                    // OAuth
                    Tenant = SafeConfig("Tenant"),
                    Client = SafeConfig("Client"),
                    ClientSecret = SafeConfig("ClientSecret"),
                    Correo = SafeConfig("Correo"),

                    // SMTP
                    CorreoSMTP = SafeConfig("CorreoSMTP"),
                    ServidorSMTP = SafeConfig("ServidorSMTP"),
                    PuertoSMTP = SafeConfig("PuertoSMTP"),
                    UsuarioSMTP = SafeConfig("UsuarioSMTP"),
                    PasswordSMTP = SafeConfig("PasswordSMTP"),
                    RequiereSSL = SafeConfig("RequiereSSL"),
                    TLS12 = SafeConfig("TLS12"),
                    TLS13 = SafeConfig("TLS13"),
                    ProtocoloCorreo = SafeConfig("ProtocoloCorreo")
                };

                return correoConfig;
            }).ToList();

            return resultado;
        }
    }
}
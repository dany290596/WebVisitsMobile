using System.Linq;

namespace WebVisitsMobile.Domain.Entities.Configuracion
{
    public static class ConfiguracionCorreoEmpresaKeys
    {
        public static readonly Guid TipoAutenticacion = Guid.Parse("28A7D3B4-0BD0-4315-9178-FC63EC3CB3EB");

        public static readonly Guid SmtpCorreo = Guid.Parse("B93A945C-332B-4AEC-A15C-E43524F7CE3B");
        public static readonly Guid SmtpServidor = Guid.Parse("9F16BC6E-A399-4BF0-9030-E23BC080F8D6");
        public static readonly Guid SmtpPuerto = Guid.Parse("6BC07BDD-2529-4C27-862E-4BE5EA2434F8");
        public static readonly Guid SmtpUsuario = Guid.Parse("357C5EE9-4ED6-46FA-AAC5-8C531D580269");
        public static readonly Guid SmtpPassword = Guid.Parse("B21D948D-87C7-41D8-9C9A-E63A70FF8AA4");
        public static readonly Guid SmtpSsl = Guid.Parse("D0B6A0A0-7EAE-4505-9C0A-015AFBE542C9");
        public static readonly Guid SmtpTls12 = Guid.Parse("575F2675-5B70-43FF-ADE8-0C854A925718");
        public static readonly Guid SmtpTls13 = Guid.Parse("BD055521-FBD7-408F-87EC-FE4ECE36D4DC");

        public static readonly Guid OAuthTenant = Guid.Parse("7E8ADD79-7804-4DFB-9CB6-81D66F7F89B1");
        public static readonly Guid OAuthClient = Guid.Parse("DCCB8FD4-2684-40DB-AC3F-3CDF13ABBB65");
        public static readonly Guid OAuthClientSecret = Guid.Parse("5EC8DAA4-C445-4B32-A0A8-ED6A7ADE5F65");
        public static readonly Guid OAuthCorreo = Guid.Parse("028FF609-9A10-46BC-A4BC-1BAABF4B747F");

        public const string TipoSmtp = "SMTP";
        public const string TipoOAuth = "OAuth";

        public static readonly IReadOnlyDictionary<Guid, string> Nombres = new Dictionary<Guid, string>
        {
            [TipoAutenticacion] = "Tipo de autenticación",
            [SmtpCorreo] = "Correo SMTP",
            [SmtpServidor] = "Servidor SMTP",
            [SmtpPuerto] = "Puerto de salida SMTP",
            [SmtpUsuario] = "Usuario SMTP",
            [SmtpPassword] = "Contraseña SMTP",
            [SmtpSsl] = "SSL",
            [SmtpTls12] = "Requiere TLS 1.2",
            [SmtpTls13] = "Requiere TLS 1.3",
            [OAuthTenant] = "Tenant",
            [OAuthClient] = "Client",
            [OAuthClientSecret] = "Client Secret",
            [OAuthCorreo] = "Correo OAuth"
        };

        public static readonly IReadOnlyCollection<Guid> Smtp = new[]
        {
            TipoAutenticacion, SmtpCorreo, SmtpServidor, SmtpPuerto, SmtpUsuario, SmtpPassword, SmtpSsl, SmtpTls12, SmtpTls13
        };

        public static readonly IReadOnlyCollection<Guid> OAuth = new[]
        {
            TipoAutenticacion, OAuthTenant, OAuthClient, OAuthClientSecret, OAuthCorreo
        };

        public static readonly IReadOnlyCollection<Guid> Todas = Nombres.Keys.ToArray();

        public static List<Configuraciones> GetDefaultEmailConfiguration()
        {
            return new ()
            {
                new()
                {
                    TipoConfiguracion = TipoAutenticacion,
                    NombreParametro = Nombres[TipoAutenticacion],
                    Valor1 = TipoSmtp,
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = SmtpCorreo,
                    NombreParametro = Nombres[SmtpCorreo],
                    Valor1 = string.Empty,
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = SmtpServidor,
                    NombreParametro = Nombres[SmtpServidor],
                    Valor1 = string.Empty,
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = SmtpPuerto,
                    NombreParametro = Nombres[SmtpPuerto],
                    Valor1 = "587",
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = SmtpUsuario,
                    NombreParametro = Nombres[SmtpUsuario],
                    Valor1 = string.Empty,
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = SmtpPassword,
                    NombreParametro = Nombres[SmtpPassword],
                    Valor1 = string.Empty,
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = SmtpSsl,
                    NombreParametro = Nombres[SmtpSsl],
                    Valor1 = "false",
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = SmtpTls12,
                    NombreParametro = Nombres[SmtpTls12],
                    Valor1 = "true",
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = SmtpTls13,
                    NombreParametro = Nombres[SmtpTls13],
                    Valor1 = "false",
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = OAuthTenant,
                    NombreParametro = Nombres[OAuthTenant],
                    Valor1 = string.Empty,
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = OAuthClient,
                    NombreParametro = Nombres[OAuthClient],
                    Valor1 = string.Empty,
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = OAuthClientSecret,
                    NombreParametro = Nombres[OAuthClientSecret],
                    Valor1 = string.Empty,
                    editable = 1,
                    lectura = 1
                },
                new()
                {
                    TipoConfiguracion = OAuthCorreo,
                    NombreParametro = Nombres[OAuthCorreo],
                    Valor1 = string.Empty,
                    editable = 1,
                    lectura = 1
                }
            };
        }
    }
}

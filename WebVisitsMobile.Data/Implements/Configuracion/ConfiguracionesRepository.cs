using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Configuracion;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.Entities.Encriptacion;

namespace WebVisitsMobile.Data.Implements.Configuracion
{
    public class ConfiguracionesRepository : Repository<Configuraciones>, IConfiguracionesRepository
    {
        public ConfiguracionesRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<IEnumerable<Configuraciones>> GetAllSetting()
        {
            return await _context.Configuraciones
              .Include(cr => cr.EmpresaCliente)
              .OrderByDescending(u => u.FechaCreacion)
              .ThenBy(u => u.Estado)
              .ToListAsync();
        }

        public async Task<Configuraciones> GetSetting(Expression<Func<Configuraciones, bool>> predicate)
        {
            return await _context.Configuraciones.
                FirstOrDefaultAsync(predicate);
        }

        public async Task AddRangeSetting(IEnumerable<Configuraciones> settings)
        {
            await _context.Configuraciones.AddRangeAsync(settings);
        }

        public async Task<List<SettingsGroup>> GetGroupByCompany()
        {
            var settings = await _context.Configuraciones
                .Where(c => c.Estado == 1 && c.EmpresaCliente.Estado == 1)
                .Include(c => c.EmpresaCliente)
                .AsNoTracking()
                .ToListAsync();

            var requiredKeys = new List<string>
            {
                "742CE98B-684B-4A76-BA0D-CF62621FC3E7",
                "BB617929-5F49-4FDC-8C28-62435505B600",
                "29625587-4A45-495A-B728-203608694C44",
                "60ADEBFE-01B5-497A-828B-CF3801F37495",
                "9B02E35B-A069-4BF5-B9CA-337A59455347",
                "82481E61-4BF5-44CE-B222-3283F7BC02F9",
                "40E1A0B9-9144-490E-BF75-7663F3447118",
                "788F90F3-0CE3-4E96-B4BA-38DA1CFE105B",
                "FF5E7D45-FCED-4169-B4EB-BA70B43F7BB6",
                "C98EE139-92FB-4E71-94B7-AE258DD1929A",
                //"5006A3E3-1E78-4341-9253-C2189A7C8974",
                //"5F9327BE-42D6-46B9-BF0E-DB7176371A20",
                //"9914DCB1-B370-4FC5-8CA3-D5ADD1605AF9",
                //"A90006CA-A3E8-4576-A8B0-25B1C5438D55"
            };

            var result = new List<SettingsGroup>();

            var grouped = settings
                .GroupBy(c => new { c.EmpresaClienteId, c.EmpresaCliente.RazonSocial });

            foreach (var group in grouped)
            {
                var stringKeySettings = group
                    .Where(x => x.TipoConfiguracion != Guid.Empty)
                    .GroupBy(x => x.TipoConfiguracion)
                    .ToDictionary(
                        g => g.Key.ToString().ToUpper(),
                        g => g.First().Valor1 ?? string.Empty
                    );

                // ✅ VALIDAR QUE EXISTAN TODAS LAS CLAVES OBLIGATORIAS
                bool isValid = requiredKeys.All(k =>
                    stringKeySettings.ContainsKey(k) &&
                    !string.IsNullOrWhiteSpace(stringKeySettings[k]));

                if (!isValid)
                    continue; // ❌ Empresa incompleta → la saltamos

                var appSetting = new AppSetting
                {
                    CustomerId = stringKeySettings["742CE98B-684B-4A76-BA0D-CF62621FC3E7"],
                    ClientId = stringKeySettings["BB617929-5F49-4FDC-8C28-62435505B600"],
                    ClientSecretOrCertificate = stringKeySettings["29625587-4A45-495A-B728-203608694C44"],

                    IdpAuthenticationUrl = stringKeySettings["60ADEBFE-01B5-497A-828B-CF3801F37495"],
                    ApiUrl = stringKeySettings["9B02E35B-A069-4BF5-B9CA-337A59455347"],
                    CallbackAndEventUrl = stringKeySettings["82481E61-4BF5-44CE-B222-3283F7BC02F9"],
                    PremiumReportUrl = stringKeySettings.GetValueOrDefault("84BA81E1-56C0-4BEE-A57F-D05C13BB544A"),
                    CredentialManagementURL = stringKeySettings["5006A3E3-1E78-4341-9253-C2189A7C8974"],
                    UsersURL = stringKeySettings["5F9327BE-42D6-46B9-BF0E-DB7176371A20"],
                    EventsURL = stringKeySettings["9914DCB1-B370-4FC5-8CA3-D5ADD1605AF9"],
                    TransactionURL = stringKeySettings["A90006CA-A3E8-4576-A8B0-25B1C5438D55"],

                    ContentType = stringKeySettings["40E1A0B9-9144-490E-BF75-7663F3447118"],
                    AcceptType = stringKeySettings.GetValueOrDefault("4B6BCEFA-20CA-48B9-92FA-5396C7C94202"),
                    ApplicationId = stringKeySettings["788F90F3-0CE3-4E96-B4BA-38DA1CFE105B"],
                    ApplicationVersion = stringKeySettings["FF5E7D45-FCED-4169-B4EB-BA70B43F7BB6"],


                    PartNumberField = stringKeySettings["C98EE139-92FB-4E71-94B7-AE258DD1929A"],

                    AutoDetectPartNumber = stringKeySettings.GetValueOrDefault("D539FF01-17F0-4C29-9E17-668A5591ACE5"),
                    SelectPartNumber = stringKeySettings.GetValueOrDefault("18A0E41D-960E-4F52-9604-D0C773A87F9C"),
                    ManualEntryPartNumber = stringKeySettings.GetValueOrDefault("32DC2E87-E6A4-48D7-AF0E-B967ED2BBF49")
                };

                if (!IsValidFormat(appSetting))
                    continue;

                result.Add(new SettingsGroup
                {
                    EmpresaClienteId = group.Key.EmpresaClienteId,
                    EmpresaClienteNombre = group.Key.RazonSocial,
                    Settings = appSetting
                });
            }

            return result;
        }

        public async Task<IEnumerable<SettingsGroupEncrypted>> GetGroupByCompanyEncrypted()
        {
            var settings = await _context.Configuraciones
                .Where(c => c.Estado == 1 && c.EmpresaCliente.Estado == 1)
                .Include(c => c.EmpresaCliente)
                .AsNoTracking()
                .ToListAsync();

            var requiredKeys = new List<string>
            {
                "BB164E4E-F6F3-4C6A-9CE4-0B646B2A0433",
                "10058B5D-8B95-4C27-9ED1-0426762154FD"
            };

            var result = new List<SettingsGroupEncrypted>();
            var grouped = settings
                .GroupBy(c => new { c.EmpresaClienteId, c.EmpresaCliente.RazonSocial, c.EmpresaCliente.UsaCredencialesHID, c.EmpresaCliente.UsaCredencialesWallet });

            foreach (var group in grouped)
            {
                var stringKeySettings = group
                    .Where(x => x.TipoConfiguracion != Guid.Empty)
                    .GroupBy(x => x.TipoConfiguracion)
                    .ToDictionary(
                        g => g.Key.ToString().ToUpper(),
                        g => g.First().Valor1 ?? string.Empty
                    );

                bool isValid = true;

                if (group.Key.UsaCredencialesHID == 1)
                {
                    isValid &= stringKeySettings.TryGetValue(
                        "BB164E4E-F6F3-4C6A-9CE4-0B646B2A0433",
                        out var hidValue)
                        && !string.IsNullOrWhiteSpace(hidValue);
                }

                // Si usa Wallet, debe existir la configuración Wallet
                if (group.Key.UsaCredencialesWallet == 1)
                {
                    isValid &= stringKeySettings.TryGetValue(
                        "10058B5D-8B95-4C27-9ED1-0426762154FD",
                        out var walletValue)
                        && !string.IsNullOrWhiteSpace(walletValue);
                }

                if (!isValid)
                    continue; // ❌ Empresa incompleta → la saltamos

                key? credencialesHID = null;
                key? credencialesWallet = null;

                // HID
                if (stringKeySettings.TryGetValue("BB164E4E-F6F3-4C6A-9CE4-0B646B2A0433", out var hidJson) &&
                    !string.IsNullOrWhiteSpace(hidJson) &&
                    hidJson != "0" &&
                    !hidJson.Equals("undefined", StringComparison.OrdinalIgnoreCase))
                {
                    credencialesHID = JsonSerializer.Deserialize<key>(hidJson);
                }

                // Wallet
                if (stringKeySettings.TryGetValue("10058B5D-8B95-4C27-9ED1-0426762154FD", out var walletJson) &&
                    !string.IsNullOrWhiteSpace(walletJson) &&
                    walletJson != "0" &&
                    !walletJson.Equals("undefined", StringComparison.OrdinalIgnoreCase))
                {
                    credencialesWallet = JsonSerializer.Deserialize<key>(walletJson);
                }

                result.Add(new SettingsGroupEncrypted
                {
                    EmpresaClienteId = group.Key.EmpresaClienteId,
                    EmpresaClienteNombre = group.Key.RazonSocial,
                    UsaCredencialesHID = group.Key.UsaCredencialesHID,
                    UsaCredencialesWallet = group.Key.UsaCredencialesWallet,
                    CredencialesHID = credencialesHID,
                    CredencialesWallet = credencialesWallet
                });
            }

            return result;
        }

        public void DeleteRange(IEnumerable<Configuraciones> settings)
        {
            _context.Configuraciones.RemoveRange(settings);
        }

        public IQueryable<Configuraciones> GetAllSettingQueryable()
        {
            return _context.Configuraciones
                .Include(cr => cr.EmpresaCliente)
                .AsQueryable();
        }

        private bool IsValidFormat(AppSetting settings)
        {
            // CustomerId: solo dígitos, al menos 4
            if (!long.TryParse(settings.CustomerId, out _) || settings.CustomerId.Length < 4)
                return false;

            // ClientId: debe contener un guion y no ser todo mayúsculas repetidas
            if (!settings.ClientId.Contains("-") || settings.ClientId.Length < 10 || settings.ClientId.All(c => c == settings.ClientId[0]))
                return false;

            // URLs: deben comenzar con http:// o https://
            if (!settings.IdpAuthenticationUrl.StartsWith("http://") && !settings.IdpAuthenticationUrl.StartsWith("https://"))
                return false;
            if (!settings.ApiUrl.StartsWith("http://") && !settings.ApiUrl.StartsWith("https://"))
                return false;
            if (!settings.CallbackAndEventUrl.StartsWith("http://") && !settings.CallbackAndEventUrl.StartsWith("https://"))
                return false;

            // ApplicationId: mínimo 5 caracteres
            if (string.IsNullOrWhiteSpace(settings.ApplicationId) || settings.ApplicationId.Length < 5)
                return false;

            // ClientSecret: mínimo 6 caracteres
            if (string.IsNullOrWhiteSpace(settings.ClientSecretOrCertificate) || settings.ClientSecretOrCertificate.Length < 6)
                return false;

            return true;
        }

        public async Task<SettingAccountEmail> GetSettingOfAccountEmail()
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

            var configs = await _context.Set<Configuraciones>()
                .AsNoTracking()
                .Where(c => configuracionIds.Values.Contains(c.Id))
                .ToListAsync();

            ConfiguracionCorreo? ObtenerConfiguracion(string key)
            {
                var id = configuracionIds[key];

                var config = configs.FirstOrDefault(c => c.Id == id);

                if (config == null)
                    return null;

                return new ConfiguracionCorreo
                {
                    Id = config.Id,
                    NombreParametro = config.NombreParametro,
                    Valor1 = config.Valor1
                };
            }

            return new SettingAccountEmail
            {
                // OAuth
                Tenant = ObtenerConfiguracion("Tenant"),
                Client = ObtenerConfiguracion("Client"),
                ClientSecret = ObtenerConfiguracion("ClientSecret"),
                Correo = ObtenerConfiguracion("Correo"),

                // SMTP
                CorreoSMTP = ObtenerConfiguracion("CorreoSMTP"),
                ServidorSMTP = ObtenerConfiguracion("ServidorSMTP"),
                PuertoSMTP = ObtenerConfiguracion("PuertoSMTP"),
                UsuarioSMTP = ObtenerConfiguracion("UsuarioSMTP"),
                PasswordSMTP = ObtenerConfiguracion("PasswordSMTP"),
                RequiereSSL = ObtenerConfiguracion("RequiereSSL"),
                TLS12 = ObtenerConfiguracion("TLS12"),
                TLS13 = ObtenerConfiguracion("TLS13"),
                ProtocoloCorreo = ObtenerConfiguracion("ProtocoloCorreo")
            };
        }
    }
}
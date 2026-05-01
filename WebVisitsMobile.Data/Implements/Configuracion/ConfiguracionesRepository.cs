using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Configuracion;
using WebVisitsMobile.Domain.Entities.Configuracion;

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

        public async Task<List<SettingsGroup>> GetSettingGroupByCompany()
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
                "C98EE139-92FB-4E71-94B7-AE258DD1929A"
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
    }
}
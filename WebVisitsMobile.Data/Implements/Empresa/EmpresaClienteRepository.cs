using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Empresa;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.Entities.Empresa;

namespace WebVisitsMobile.Data.Implements.Empresa
{
    public class EmpresaClienteRepository : Repository<EmpresaCliente>, IEmpresaClienteRepository
    {
        public EmpresaClienteRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<EmpresaCliente> GetCompanyClient(Expression<Func<EmpresaCliente, bool>> predicate)
        {
            return await _context.EmpresaCliente
                .Include(x => x.Pais)
                .Include(x => x.PaisEstado)
                .Include(x => x.Ciudad)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<EmpresaCliente>> GetAllCompanyClient()
        {
            return await _context.EmpresaCliente
                .Include(x => x.Pais)
                .Include(x => x.PaisEstado)
                .Include(x => x.Ciudad)
                .OrderByDescending(u => u.FechaCreacion)
                .ThenBy(u => u.Estado)
                .ToListAsync();
        }

        public async Task<CompanyClientWithSetting> GetCompanyClientWithSetting(Guid companyClientId)
        {
            try
            {
                var hidSettingIds = new HashSet<Guid>
                {
                    Guid.Parse("742CE98B-684B-4A76-BA0D-CF62621FC3E7"),
                    Guid.Parse("BB617929-5F49-4FDC-8C28-62435505B600"),
                    Guid.Parse("29625587-4A45-495A-B728-203608694C44"),
                    Guid.Parse("60ADEBFE-01B5-497A-828B-CF3801F37495"),
                    Guid.Parse("9B02E35B-A069-4BF5-B9CA-337A59455347"),
                    Guid.Parse("82481E61-4BF5-44CE-B222-3283F7BC02F9"),
                    Guid.Parse("84BA81E1-56C0-4BEE-A57F-D05C13BB544A"),
                    Guid.Parse("5006A3E3-1E78-4341-9253-C2189A7C8974"),
                    Guid.Parse("5F9327BE-42D6-46B9-BF0E-DB7176371A20"),
                    Guid.Parse("9914DCB1-B370-4FC5-8CA3-D5ADD1605AF9"),
                    Guid.Parse("A90006CA-A3E8-4576-A8B0-25B1C5438D55"),
                    Guid.Parse("40E1A0B9-9144-490E-BF75-7663F3447118"),
                    Guid.Parse("4B6BCEFA-20CA-48B9-92FA-5396C7C94202"),
                    Guid.Parse("788F90F3-0CE3-4E96-B4BA-38DA1CFE105B"),
                    Guid.Parse("FF5E7D45-FCED-4169-B4EB-BA70B43F7BB6"),
                    Guid.Parse("C98EE139-92FB-4E71-94B7-AE258DD1929A"),
                    Guid.Parse("D539FF01-17F0-4C29-9E17-668A5591ACE5"),
                    Guid.Parse("18A0E41D-960E-4F52-9604-D0C773A87F9C"),
                    Guid.Parse("32DC2E87-E6A4-48D7-AF0E-B967ED2BBF49")
                };

                // Consulta 1: datos de la empresa
                var empresa = await _context.EmpresaCliente
                    .Include(x => x.Pais)
                    .Include(x => x.PaisEstado)
                    .Include(x => x.Ciudad)
                    .FirstOrDefaultAsync(x => x.Id == companyClientId);

                if (empresa == null) return null;

                var result = new CompanyClientWithSetting
                {
                    Id = empresa.Id,
                    RazonSocial = empresa.RazonSocial,
                    RFC = empresa.RFC,
                    TelefonoEmpresa = empresa.TelefonoEmpresa,
                    TelefonoMovil = empresa.TelefonoMovil,
                    CorreoElectronico = empresa.CorreoElectronico,
                    UsaCredencialesHID = empresa.UsaCredencialesHID,
                    UsaCredencialesWallet = empresa.UsaCredencialesWallet,
                    Pais = empresa.Pais!,
                    PaisEstado = empresa.PaisEstado!,
                    Ciudad = empresa.Ciudad!
                };

                // Consulta 2: configuraciones HID (solo si aplica)
                if (empresa.UsaCredencialesHID == 1)
                {
                    var settings = await _context.Configuraciones
                        .Where(c => c.EmpresaClienteId == companyClientId
                                 && c.Estado == 1
                                 && hidSettingIds.Contains(c.TipoConfiguracion))
                        .ToListAsync();

                    Setting? Get(string guid)
                    {
                        var c = settings.FirstOrDefault(x => x.TipoConfiguracion == Guid.Parse(guid));
                        if (c == null) return null;
                        return new Setting
                        {
                            Id = c.Id,
                            UsuarioCreadorId = c.UsuarioCreadorId,
                            UsuarioModificadorId = c.UsuarioModificadorId,
                            UsuarioBajaId = c.UsuarioBajaId,
                            UsuarioReactivadorId = c.UsuarioReactivadorId,
                            FechaCreacion = c.FechaCreacion,
                            FechaModificacion = c.FechaModificacion,
                            FechaBaja = c.FechaBaja,
                            FechaReactivacion = c.FechaReactivacion,
                            Estado = c.Estado,
                            NombreParametro = c.NombreParametro,
                            ValorGuid = c.ValorGuid,
                            Valor1 = c.Valor1,
                            Valor2 = c.Valor2,
                            Valor3 = c.Valor3,
                            editable = c.editable,
                            lectura = c.lectura,
                            EmpresaClienteId = c.EmpresaClienteId,
                            TipoConfiguracion = c.TipoConfiguracion
                        };
                    }

                    result.CustomerId = Get("742CE98B-684B-4A76-BA0D-CF62621FC3E7");
                    result.ClientId = Get("BB617929-5F49-4FDC-8C28-62435505B600");
                    result.ClientSecretOrCertificate = Get("29625587-4A45-495A-B728-203608694C44");
                    result.IdpAuthenticationUrl = Get("60ADEBFE-01B5-497A-828B-CF3801F37495");
                    result.ApiUrl = Get("9B02E35B-A069-4BF5-B9CA-337A59455347");
                    result.CallbackAndEventUrl = Get("82481E61-4BF5-44CE-B222-3283F7BC02F9");
                    result.PremiumReportUrl = Get("84BA81E1-56C0-4BEE-A57F-D05C13BB544A");
                    result.CredentialManagementURL = Get("5006A3E3-1E78-4341-9253-C2189A7C8974");
                    result.UsersURL = Get("5F9327BE-42D6-46B9-BF0E-DB7176371A20");
                    result.EventsURL = Get("9914DCB1-B370-4FC5-8CA3-D5ADD1605AF9");
                    result.TransactionURL = Get("A90006CA-A3E8-4576-A8B0-25B1C5438D55");
                    result.ContentType = Get("40E1A0B9-9144-490E-BF75-7663F3447118");
                    result.AcceptType = Get("4B6BCEFA-20CA-48B9-92FA-5396C7C94202");
                    result.ApplicationId = Get("788F90F3-0CE3-4E96-B4BA-38DA1CFE105B");
                    result.ApplicationVersion = Get("FF5E7D45-FCED-4169-B4EB-BA70B43F7BB6");
                    result.PartNumberField = Get("C98EE139-92FB-4E71-94B7-AE258DD1929A");
                    result.AutoDetectPartNumber = Get("D539FF01-17F0-4C29-9E17-668A5591ACE5");
                    result.SelectPartNumber = Get("18A0E41D-960E-4F52-9604-D0C773A87F9C");
                    result.ManualEntryPartNumber = Get("32DC2E87-E6A4-48D7-AF0E-B967ED2BBF49");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
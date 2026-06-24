using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.HID;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Implements.HID
{
    public class CredencialHIDRepository : Repository<CredencialHid>, ICredencialHIDRepository
    {
        public CredencialHIDRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<CredencialHid> GetCredentialHID(Expression<Func<CredencialHid, bool>> predicate)
        {
            return await _context.CredencialHid
                .Include(i => i.LicenciaHidUser)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<CredencialHid>> GetAllCredentialHID()
        {
            return await _context.CredencialHid
                .Include(i => i.DipositivosHid)
                .Include(i => i.LicenciaHidUser)
                .OrderByDescending(u => u.FechaCreacion)
                .ThenBy(u => u.Estado)
                .ToListAsync();
        }

        public async Task<string?> GetCredencialWalletMasReciente(Guid licenciaHidUserId)
        {
            var credencial = await _context.CredencialHid
                .Where(c => c.Usuarioid == licenciaHidUserId)
                .OrderByDescending(c => c.FechaCreacion)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return credencial?.CredencialValor;
        }

        public async Task<CredencialHid> GetCredentialHIDExternalId(Guid id)
        {
            return await _context.CredencialHid
                 .Where(u => u.ExternalId == id)
                 .OrderByDescending(u => u.FechaCreacion)
                 .AsNoTracking()
                 .FirstOrDefaultAsync();
        }

        public async Task<string?> GetCredencialWalletMasRecienteWatch(Guid licenciaHidUserId)
        {
            var credencial = await _context.CredencialHid
                .Where(c => c.Usuarioid == licenciaHidUserId && c.TipoCredencial=="Wallet Watch")
                .OrderByDescending(c => c.FechaCreacion)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return credencial?.CredencialValor;
        }
    }
}
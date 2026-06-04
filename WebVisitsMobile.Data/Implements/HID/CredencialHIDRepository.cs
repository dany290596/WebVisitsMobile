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
    }
}
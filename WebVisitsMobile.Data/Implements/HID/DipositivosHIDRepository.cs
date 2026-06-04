using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.HID;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Implements.HID
{
    public class DipositivosHIDRepository : Repository<DipositivosHid>, IDipositivosHIDRepository
    {
        public DipositivosHIDRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<DipositivosHid> GetDevice(Expression<Func<DipositivosHid, bool>> predicate)
        {
            return await _context.DipositivosHid
                .Include(i => i.LicenciaHidUser)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<DipositivosHid>> GetAllDevice()
        {
            return await _context.DipositivosHid.
               Include(i => i.LicenciaHidUser)
               .OrderByDescending(u => u.FechaCreacion)
               .ThenBy(u => u.Estado)
               .ToListAsync();
        }
    }
}
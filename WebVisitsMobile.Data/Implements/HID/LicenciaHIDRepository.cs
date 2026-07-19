using Microsoft.EntityFrameworkCore;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.HID;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Implements.HID
{
    public class LicenciaHIDRepository : Repository<LicenciaHID>, ILicenciaHIDRepository
    {
        public LicenciaHIDRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<IEnumerable<LicenciaHID>> GetAllLicense()
        {
            return await _context.LicenciaHID
                .Include(i => i.EmpresaCliente)
                .OrderByDescending(u => u.FechaCreacion)
                .ThenBy(u => u.Estado)
                .ToListAsync();
        }
    }
}
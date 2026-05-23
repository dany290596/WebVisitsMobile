using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.HID;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Implements.HID
{
    public class UsuarioHidTipoCredencialRepository : Repository<UsuarioHidTipoCredencial>, IUsuarioHidTipoCredencialRepository
    {
        public UsuarioHidTipoCredencialRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<UsuarioHidTipoCredencial> GetUserHidTypeCredential(Expression<Func<UsuarioHidTipoCredencial, bool>> predicate)
        {
            return await _context.UsuarioHidTipoCredencial
                .Include(l => l.LicenciaHidUser)
                .Include(l => l.TipoCredencial)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<UsuarioHidTipoCredencial>> GetAllUserHidTypeCredential()
        {
            return await _context.UsuarioHidTipoCredencial
                .Include(x => x.LicenciaHidUser)
                .Include(x => x.LicenciaHidUser.LicenciaHID)
                .Include(x => x.TipoCredencial)
                .OrderByDescending(u => u.FechaCreacion)
                .ThenBy(u => u.Estado)
                .ToListAsync();
        }
    }
}
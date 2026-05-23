using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Empresa;
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
    }
}
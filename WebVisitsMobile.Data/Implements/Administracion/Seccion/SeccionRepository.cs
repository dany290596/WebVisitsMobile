using Microsoft.EntityFrameworkCore;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Administracion.Seccion;
using WebVisitsMobile.Domain.Entities.Administracion.Modulo;
using WebVisitsMobile.Domain.Entities.Administracion.Seccion;

namespace WebVisitsMobile.Data.Implements.Administracion.Seccion
{
    public class SeccionRepository : Repository<Domain.Entities.Administracion.Seccion.Seccion>, ISeccionRepository
    {
        public SeccionRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<IEnumerable<SeccionesPorModulo>> GetSectionsGroupedByModule(Guid? perfilId)
        {
            var query = _context.Seccion
                .Include(s => s.Modulo)
                .Include(s => s.PerfilPermisoSecciones)
                .AsQueryable();

            if (perfilId != null)
            {
                query = query.Where(s => s.PerfilPermisoSecciones.Any(p => p.PerfilId == perfilId));
            }

            return await query
                .GroupBy(s => new { s.Modulo.Id, s.Modulo.Nombre, s.Modulo.Imagen })
                .Select(g => new SeccionesPorModulo
                {
                    ModuloId = g.Key.Id,
                    ModuloNombre = g.Key.Nombre,
                    ModuloImagen = g.Key.Imagen!,
                    Secciones = g.OrderBy(s => s.Orden).Select(s => new Secciones
                    {
                        PerfilPermisoSeccionId = s.PerfilPermisoSecciones
                            .Where(p => p.PerfilId == perfilId)
                            .Select(p => (Guid?)p.Id)
                            .FirstOrDefault(),
                        SeccionId = s.Id,
                        Nombre = s.Nombre,
                        Orden = s.Orden,
                        Path = s.Path,

                        Permiso = s.PerfilPermisoSecciones
                            .Where(p => p.PerfilId == perfilId)
                            .Select(p => (byte?)p.Permiso)
                            .FirstOrDefault() ?? 0,

                        Vence = s.PerfilPermisoSecciones
                            .Where(p => p.PerfilId == perfilId)
                            .Select(p => (byte?)p.Vence)
                            .FirstOrDefault() ?? 0,

                        FechaVencimiento = s.PerfilPermisoSecciones
                            .Where(p => p.PerfilId == perfilId)
                            .Select(p => p.FechaVencimiento)
                            .FirstOrDefault()
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Administracion.Seccion.Seccion>> GetAllSection()
        {
            return await _context.Seccion
                .Include(x => x.Modulo)
                .OrderByDescending(u => u.FechaCreacion)
                .ThenBy(u => u.Estado)
                .ToListAsync();
        }
    }
}
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
            var seccionesExcluidas = new[]
            {
                Guid.Parse("98ba1bd1-47c3-4533-88a0-b52992cc16fd"),
                //Guid.Parse("88e9733e-1d92-4b7a-8368-2380b3ec463c")
            };

            var seccionDebug = Guid.Parse("5eb4575c-4588-4cd4-a6a3-3f15978a4f93");

            var query = _context.Seccion
                .Include(s => s.Modulo)
                .Include(s => s.PerfilPermisoSecciones)
                .Where(s => !seccionesExcluidas.Contains(s.Id))
                .AsQueryable();

            if (perfilId != null)
            {
                query = query.Where(s => s.PerfilPermisoSecciones.Any(p => p.PerfilId == perfilId));
            }

            var existeDespuesFiltro = await query.AnyAsync(s => s.Id == seccionDebug);
            var existe = await query.AnyAsync(s => s.Id == seccionDebug);

            Console.WriteLine($"SECCION {seccionDebug} DESPUES DEL WHERE: {existe}");
            Console.WriteLine($"PERFIL RECIBIDO: {perfilId}");

            var resultado = await query
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
                    })
                    .OrderByDescending(u => u.Nombre)
                    .ThenBy(u => u.Nombre)
                    .ToList()
                })
                .OrderByDescending(u => u.ModuloNombre)
                .ThenBy(u => u.ModuloNombre)
                .ToListAsync();

            // LOG AQUI
            var seccionFinal = resultado
                .SelectMany(m => m.Secciones)
                .FirstOrDefault(s => s.SeccionId == Guid.Parse("98ba1bd1-47c3-4533-88a0-b52992cc16fd"));

            Console.WriteLine($"SECCION FINAL: {seccionFinal?.SeccionId}");
            Console.WriteLine($"NOMBRE FINAL: {seccionFinal?.Nombre}");
            Console.WriteLine($"PERMISO FINAL: {seccionFinal?.Permiso}");
            return resultado.Where(m => m.Secciones.Any());
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
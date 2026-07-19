using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Administracion.Perfil;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Implements.Administracion.Sesion
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<Usuario> GetUserForCredentials(Login login)
        {
            return await _entities
                .Include(i => i.TipoUsuario)
                .Include(i => i.Perfil)
                .FirstOrDefaultAsync(x => x.Correo.ToLower() == login.Email.ToLower());
        }

        public IEnumerable<Usuario> GetAllUser()
        {
            return _context.Usuario.Include(i => i.Perfil).Include(i => i.TipoUsuario).AsEnumerable();
        }

        public async Task<Usuario> GetFirstOrDefaultUser(Expression<Func<Usuario, bool>> predicate)
        {
            return await _context.Usuario.
                Include(i => i.Perfil).
                Include(i => i.TipoUsuario).
                Include(i => i.EmpresaCliente).
                FirstOrDefaultAsync(predicate);
        }

        public async Task<Usuario> GetUser(Expression<Func<Usuario, bool>> predicate)
        {
            return await _context.Usuario.
                FirstOrDefaultAsync(predicate);
        }

        public Guid GetSelectUserByUserTypeNameAndUserMail(string typeUserName, string userMail)
        {
            return _context.Usuario.OrderBy(ob => ob.Id).Where(s => s.TipoUsuario.Nombre == typeUserName && s.Correo == userMail).Select(s => s.Id).First();
        }

        public async Task<bool> EmailExistsAsync(string email, Guid excludedUserId)
        {
            return await _context.Usuario
                .AnyAsync(u => u.Correo == email && u.Id != excludedUserId);
        }

        public async Task<AccountInfo?> GetAccountInfo(Guid userId)
        {
            // 1. Obtener usuario con perfil y tipo de usuario
            var usuario = await _context.Usuario
                .Include(u => u.Perfil)
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.Id == userId && u.Estado == 1);

            if (usuario == null)
                return null;

            var account = new AccountInfo
            {
                UsuarioId = usuario.Id,
                Correo = usuario.Correo,
                Perfil = usuario.Perfil?.Nombre,
                TipoUsuario = usuario.TipoUsuario?.Nombre,
                Estado = usuario.Estado,
                Licencia = new LicenciaHID()
            };

            // 2. Si tiene empresa directa, obtenerla con ubicación
            if (usuario.EmpresaClienteId.HasValue)
            {
                var empresa = await _context.EmpresaCliente
                    .Include(e => e.Pais)
                    .Include(e => e.PaisEstado)
                    .Include(e => e.Ciudad)
                    .FirstOrDefaultAsync(e => e.Id == usuario.EmpresaClienteId.Value && e.Estado == 1);

                if (empresa != null)
                {
                    account.EmpresaCliente = new EmpresaCliente
                    {
                        Id = empresa.Id,
                        RazonSocial = empresa.RazonSocial,
                        RFC = empresa.RFC,
                        TelefonoEmpresa = empresa.TelefonoEmpresa,
                        TelefonoMovil = empresa.TelefonoMovil,
                        CorreoElectronico = empresa.CorreoElectronico,
                        UsaCredencialesHID = empresa.UsaCredencialesHID,
                        UsaCredencialesWallet = empresa.UsaCredencialesWallet,
                        PaisId = empresa.PaisId,
                        EstadoId = empresa.EstadoId,
                        CiudadId = empresa.CiudadId,
                        Pais = empresa.Pais,
                        PaisEstado = empresa.PaisEstado,
                        Ciudad = empresa.Ciudad,
                        FechaCreacion = empresa.FechaCreacion,
                        FechaModificacion = empresa.FechaModificacion,
                        Estado = empresa.Estado,
                        UsuarioCreadorId = empresa.UsuarioCreadorId
                    };

                    var licencia = await _context.LicenciaHID
                        .FirstOrDefaultAsync(l => l.EmpresaClienteId == empresa.Id && l.Estado == 1);

                    if (licencia != null)
                    {
                        account.Licencia = new LicenciaHID
                        {
                            Id = licencia.Id,
                            NumeroParte = licencia.NumeroParte,
                            Nombre = licencia.Nombre,
                            EmpresaClienteId = licencia.EmpresaClienteId,
                            CantidadTotal = licencia.CantidadTotal,
                            CantidadDisponible = licencia.CantidadDisponible,
                            CantidadConsumida = licencia.CantidadConsumida,
                            FechaInicio = licencia.FechaInicio,
                            FechaFin = licencia.FechaFin,
                            EstadoLicencia = licencia.EstadoLicencia,
                            EstadoPeriodo = licencia.EstadoPeriodo,
                            MensajeEstado = licencia.MensajeEstado,
                            FechaCreacion = licencia.FechaCreacion,
                            Estado = licencia.Estado,
                            UsuarioCreadorId = licencia.UsuarioCreadorId
                        };
                    }
                }
            }

            // 3. Obtener permisos del perfil del usuario
            var permisos = await _context.PerfilPermisoSeccion
                .Include(pps => pps.Seccion)
                    .ThenInclude(s => s.Modulo)
                .Where(pps => pps.PerfilId == usuario.PerfilId && pps.Estado == 1)
                .ToListAsync();

            account.Permisos = permisos.Select(pps => new PerfilPermisoSeccionInfo
            {
                Modulo = pps.Seccion?.Modulo?.Nombre ?? "Sin módulo",
                Seccion = pps.Seccion?.Nombre ?? "Sin sección",
                Path = pps.Seccion?.Path ?? "",
                Permiso = pps.Permiso,
                PermisoDescripcion = pps.Permiso switch
                {
                    1 => "Lectura",
                    2 => "Lectura y Escritura",
                    3 => "Lectura, Escritura y Eliminación",
                    _ => "Desconocido"
                }
            }).ToList();

            return account;
        }
    }
}
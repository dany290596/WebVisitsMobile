using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.HID;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Implements.HID
{
    public class LicenciaUserHIDRepository : Repository<LicenciaHidUser>, ILicenciaUserHIDRepository
    {
        public enum EstadoLicenciaEnum
        {
            Desconocido,
            FechasInvalidas,
            Activa,
            Caducada
        }

        public LicenciaUserHIDRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<LicenciaHidUser> GetUserHID(Expression<Func<LicenciaHidUser, bool>> predicate)
        {
            return await _context.LicenciaHidUser
                .Include(l => l.LicenciaHID)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<LicenciaHidUser>> GetAllUserHID()
        {
            return await _context.LicenciaHidUser
                .Include(x => x.LicenciaHID)
                .OrderByDescending(u => u.FechaCreacion)
                .ThenBy(u => u.Estado)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusOnly(Guid userHIDId, string newInvitationActivity, int newStatus, Guid currentUserId)
        {
            var userHID = new LicenciaHidUser
            {
                Id = userHIDId,
                InvitacionActividad = newInvitationActivity,
                Status = newStatus,
                FechaModificacion = DateTime.Now,
                UsuarioModificadorId = currentUserId
            };

            _context.LicenciaHidUser.Attach(userHID);
            _context.Entry(userHID).Property(t => t.InvitacionActividad).IsModified = true;
            _context.Entry(userHID).Property(t => t.Status).IsModified = true;
            _context.Entry(userHID).Property(t => t.FechaModificacion).IsModified = true;
            _context.Entry(userHID).Property(t => t.UsuarioModificadorId).IsModified = true;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<UserHIDExpired>> GetAllUsersHIDExpired()
        {
            var ahora = DateTime.UtcNow;

            var usuariosCaducados = await _context.LicenciaHidUser
                .Where(u =>
                    u.Estado == 1 &&
                    u.FechaFin.HasValue &&
                    u.FechaFin.Value <= ahora)
                .AsNoTracking()
                .ToListAsync();

            var resultado = usuariosCaducados
                .Select(u =>
                {
                    var fechaFinUtc = DateTime.SpecifyKind(u.FechaFin!.Value, DateTimeKind.Local).ToUniversalTime();

                    return new UserHIDExpired
                    {
                        Id = u.Id,
                        UserId = u.UserId,
                        Email = u.Email,
                        FechaInicio = u.FechaInicio,
                        FechaFin = u.FechaFin,
                        EstaCaducada = fechaFinUtc <= ahora,
                        DiasCaducados = (int)(ahora - fechaFinUtc).TotalDays,
                        DiasRestantes = 0,
                        EstadoLicencia = EstadoLicenciaEnum.Caducada.ToString()
                    };
                })
                .Where(x => x.EstaCaducada)
                .ToList();

            return resultado;
        }

        public async Task<UserHIDExpired> GetUserHIDExpired(Guid id)
        {
            var usuario = await _context.LicenciaHidUser.FirstOrDefaultAsync(u => u.Id == id);
            var hoy = DateTime.UtcNow.Date;
            var estadoLicencia = EstadoLicenciaEnum.Desconocido;
            bool estaCaducada = false;
            int diasCaducados = 0;
            int diasRestantes = 0;

            if (usuario == null)
            {
                return new UserHIDExpired
                {
                    Id = id,
                    UserId = usuario.UserId,
                    Email = null,
                    FechaInicio = null,
                    FechaFin = null,
                    EstaCaducada = false,
                    DiasCaducados = 0,
                    DiasRestantes = 0,
                    EstadoLicencia = estadoLicencia.ToString()
                };
            }

            if (!usuario.FechaInicio.HasValue || !usuario.FechaFin.HasValue)
            {
                estadoLicencia = EstadoLicenciaEnum.FechasInvalidas;
                return new UserHIDExpired
                {
                    Id = usuario.Id,
                    UserId = usuario.UserId,
                    Email = usuario.Email,
                    FechaInicio = usuario.FechaInicio,
                    FechaFin = usuario.FechaFin,
                    EstaCaducada = false,
                    DiasCaducados = 0,
                    DiasRestantes = 0,
                    EstadoLicencia = estadoLicencia.ToString()
                };
            }

            if (usuario.FechaFin.Value.Date < hoy)
            {
                estaCaducada = true;
                diasCaducados = (hoy - usuario.FechaFin.Value.Date).Days;
                estadoLicencia = EstadoLicenciaEnum.Caducada;
            }
            else
            {
                estaCaducada = false;
                diasRestantes = (usuario.FechaFin.Value.Date - hoy).Days;
                estadoLicencia = EstadoLicenciaEnum.Activa;
            }

            return new UserHIDExpired
            {
                Id = usuario.Id,
                UserId = usuario.UserId,
                Email = usuario.Email,
                FechaInicio = usuario.FechaInicio,
                FechaFin = usuario.FechaFin,
                EstaCaducada = estaCaducada,
                DiasCaducados = diasCaducados,
                DiasRestantes = diasRestantes,
                EstadoLicencia = estadoLicencia.ToString()
            };
        }

        public async Task<LicenciaHidUser?> GetUserHIDWithCredential(Guid? externalId)
        {
            // Validar que ambos parámetros sean obligatorios
            if (!externalId.HasValue)
                return null;

            // Buscar el usuario incluyendo sus credenciales
            var user = await _context.LicenciaHidUser
                .Include(u => u.Credenciales)
                .FirstOrDefaultAsync(u =>
                     u.ExternalId == externalId.Value);

            // Si no tiene credenciales, devolver null
            if (user == null || user.Credenciales == null || !user.Credenciales.Any())
                return null;

            return user;
        }

        public async Task<List<LicenciaHidUser>> GetAllLicenciasExpiradas()
        {
            var ahora = DateTime.Now;

            return await _context.LicenciaHidUser
                .Where(u => u.FechaFin.HasValue
                         && u.FechaFin.Value < ahora
                         && u.Estado == 1
                         && u.Plataforma != null)
                .Select(u => new LicenciaHidUser
                {
                    Id            = u.Id,
                    EmpresaClienteId = u.EmpresaClienteId,
                    FechaFin      = u.FechaFin,
                    Plataforma    = u.Plataforma,
                    ExternalId    = u.ExternalId,
                    UsuarioWalletId = u.UsuarioWalletId
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExisteEmailEnLicenciaHidUser(string email)
        {
            return await _context.LicenciaHidUser
                .AnyAsync(u => u.Email == email);
        }

        public async Task<LicenciaHidUser?> GetLicenciaVigenteByEmail(string email)
        {
            var ahora = DateTime.Now;

            return await _context.LicenciaHidUser
                .Where(u => u.Email == email
                         && u.UsuarioWalletId != null
                         && u.FechaInicio.HasValue && u.FechaInicio.Value <= ahora
                         && u.FechaFin.HasValue    && u.FechaFin.Value    >= ahora)
                .OrderByDescending(u => u.FechaCreacion)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }


        public async Task<LicenciaHidUser?> GetUserActivoEmail(string email)
        {
            var ahora = DateTime.Now;

            return await _context.LicenciaHidUser
                .Where(u => u.Email == email
                         && u.UsuarioWalletId != null
                         && u.Status==3
                         && u.FechaInicio.HasValue && u.FechaInicio.Value <= ahora
                         && u.FechaFin.HasValue && u.FechaFin.Value >= ahora)
                .OrderByDescending(u => u.FechaCreacion)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<LicenciaHidUser> GetUserWalletId(Guid id)
        {
            return await _context.LicenciaHidUser
                 .Where(u => u.UsuarioWalletId == id)
                 .OrderByDescending(u => u.FechaCreacion)
                 .AsNoTracking()
                 .FirstOrDefaultAsync();
        }


    }
}
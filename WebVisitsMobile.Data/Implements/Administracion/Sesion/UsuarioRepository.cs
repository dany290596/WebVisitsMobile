using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;

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
    }
}
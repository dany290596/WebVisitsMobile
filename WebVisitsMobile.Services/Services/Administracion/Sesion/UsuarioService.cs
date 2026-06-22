using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Services.QueryFilters.Administracion.Sesion;

namespace WebVisitsMobile.Services.Services.Administracion.Sesion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOption;

        public UsuarioService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOption = options.Value;
        }

        public async Task<PagedList<Usuario>> GetAll(UsuarioQueryFilter filters, Guid clientCompanyId)
        {
            filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOption.DefaultPageNumber) : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOption.DefaultPageSize) : filters.PageSize;

            IEnumerable<Usuario> usuarios = null;

            if (filters.DatosCompletos == 0)
            {
                usuarios = _unitOfWork.UsuarioRepository.GetAll();
            }
            else
            {
                usuarios = _unitOfWork.UsuarioRepository.GetAllUser();
            }

            if (filters.Correo != null) { usuarios = usuarios.Where(x => x.Correo.ToLower().Contains(filters.Correo.ToLower())); }
            if (filters.EmpresaId != null && filters.EmpresaId != Guid.Empty) { usuarios = usuarios.Where(x => x.EmpresaClienteId == filters.EmpresaId); }
            if (filters.TipoUsuarioId != null && filters.TipoUsuarioId != Guid.Empty) { usuarios = usuarios.Where(x => x.TipoUsuarioId == filters.TipoUsuarioId); }
            if (filters.PerfilId != null && filters.PerfilId != Guid.Empty) { usuarios = usuarios.Where(x => x.PerfilId == filters.PerfilId); }
            if (filters.IdAsociado != null && filters.IdAsociado != Guid.Empty) { usuarios = usuarios.Where(x => x.IdAsociado == filters.IdAsociado); }
            if (filters.Vence != null && filters.Vence > 0) { usuarios = usuarios.Where(x => x.Vence == filters.Vence); }

            if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { usuarios = usuarios.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
            if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { usuarios = usuarios.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
            if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { usuarios = usuarios.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
            if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { usuarios = usuarios.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
            if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { usuarios = usuarios.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
            if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { usuarios = usuarios.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
            if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { usuarios = usuarios.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
            if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { usuarios = usuarios.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
            if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { usuarios = usuarios.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
            if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { usuarios = usuarios.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
            if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { usuarios = usuarios.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
            if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { usuarios = usuarios.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
            if (filters.Estado != null && filters.Estado > 0) { usuarios = usuarios.Where(x => x.Estado == filters.Estado); }
            if (filters.EmpresaId != null && filters.EmpresaId != Guid.Empty) { usuarios = usuarios.Where(x => x.EmpresaClienteId == filters.EmpresaId); }

            var pagedUsuarios = PagedList<Usuario>.Create(usuarios, filters.PageNumber, filters.PageSize);

            return pagedUsuarios;
        }

        public async Task<Usuario> GetUserForCredentials(Login login)
        {
            try
            {
                return await _unitOfWork.UsuarioRepository.GetUserForCredentials(login);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> Create(Usuario user, string password, Guid currentUserId, Guid clientCompanyId)
        {
            bool booOk = false;

            try
            {
                if (currentUserId == Guid.Empty) { return false; }

                user.Id = Guid.NewGuid();
                user.Estado = 1;
                user.FechaCreacion = DateTime.Now;
                user.UsuarioCreadorId = currentUserId;
                user.IdAsociado = Guid.NewGuid();

                await _unitOfWork.UsuarioRepository.Add(user);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> Update(Usuario usuario, Guid currentUserId, Guid clientCompanyId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                if (usuario.Id == Guid.Empty) { return false; }
                Usuario currentUser = await _unitOfWork.UsuarioRepository.GetById(usuario.Id);
                if (currentUser == null) { return false; }
                currentUser.Correo = usuario.Correo;
                if (usuario.IdAsociado != Guid.Empty)
                {
                    currentUser.IdAsociado = usuario.IdAsociado;
                }

                currentUser.EmpresaClienteId = clientCompanyId;


                currentUser.FechaVencimiento = usuario.FechaVencimiento;
                currentUser.PerfilId = usuario.PerfilId;
                currentUser.TipoUsuarioId = usuario.TipoUsuarioId;
                currentUser.Vence = usuario.Vence;
                currentUser.FechaModificacion = DateTime.Now;
                currentUser.UsuarioModificadorId = currentUserId;

                if (!string.IsNullOrWhiteSpace(usuario.Contrasena))
                {
                    currentUser.Contrasena = usuario.Contrasena;
                }

                _unitOfWork.UsuarioRepository.Update(currentUser);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Usuario> GetById(Guid id)
        {
            return await _unitOfWork.UsuarioRepository.GetById(id);
        }

        public async Task<bool> Inactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;
            try
            {
                Usuario data = await _unitOfWork.UsuarioRepository.GetById(id);
                if (data == null) { return false; }
                data.FechaBaja = DateTime.Now;
                data.UsuarioBajaId = currentUserId;
                data.Estado = 2;

                _unitOfWork.UsuarioRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> Reactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;
            try
            {
                Usuario data = await _unitOfWork.UsuarioRepository.GetById(id);
                if (data == null) { return false; }
                data.FechaReactivacion = DateTime.Now;
                data.UsuarioReactivadorId = currentUserId;
                data.Estado = 1;

                _unitOfWork.UsuarioRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<Usuario> GetUserValid(Guid id)
        {

            try
            {
                if (id == Guid.Empty) { return null; }

                Usuario user = await _unitOfWork.UsuarioRepository.GetById(id);

                if (user == null || user.Estado != 1) { return null; }

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Usuario> GetUserById(Guid id)
        {
            Usuario user = await _unitOfWork.UsuarioRepository.GetFirstOrDefaultUser(g => g.Id == id);
            return user;
        }

        public async Task<Usuario?> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email inválido", nameof(email));

            return await _unitOfWork.UsuarioRepository
                .GetFirstOrDefaultUser(u => u.Correo == email);
        }

        public async Task<bool> ValidateUserEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email inválido", nameof(email));

            var usuarios = _unitOfWork.UsuarioRepository.GetAll();

            return usuarios.Any(x => x.Correo.ToLower() == email.ToLower());
        }

        public async Task<bool> SendRecoveryCode(string correo, string numero, string clave)
        {
            //try
            //{

            //    Usuario usuario = await _unitOfWork.UsuarioRepository.GetUser(x => x.Correo.ToLower() == correo.ToLower());

            //    bool agregarClave = await AddUserPassword(usuario, clave);

            //    CorreoEnviar correoEnviar = new CorreoEnviar();
            //    correoEnviar.De = "proyectos@crcdemexico.com.mx";
            //    correoEnviar.Para = correo;
            //    correoEnviar.Cc = "Contraseña";
            //    correoEnviar.Mensaje = "Estimado/a usuario/a,\n\n"
            //        + "Hemos recibido su solicitud para cambiar la contraseña de su cuenta.\n\n"
            //        + "Su código de verificación es: " + numero + "\n\n"
            //        + "Si no has solicitado este código, por favor ignora este mensaje. Es posible que alguien haya ingresado tu dirección de correo electrónico por error.\n\n"
            //        + "Saludos cordiales,\n"
            //        + "El equipo de soporte.";

            //    correoEnviar.Enviado = 2;
            //    correoEnviar.Marca = 1;
            //    correoEnviar.Asunto = "Cambio de contraseña";
            //    bool enviarCorreo = await correoEnviarServices.InsertCorreoEnviar(correoEnviar, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"));

            //    return enviarCorreo;
            //}
            //catch (Exception ex)
            //{

            //    return false;
            //}

            return true;
        }

        public async Task<bool> AddUserPassword(Usuario usuario, string clave)
        {
            bool booOk = false;
            //try
            //{
            //    ClaveRecuperacion claveRecuperacion = new ClaveRecuperacion();
            //    claveRecuperacion.Clave = clave;
            //    claveRecuperacion.FechaVigencia = DateTime.Now.AddMinutes(30);
            //    var datos_clave = JsonConvert.SerializeObject(claveRecuperacion);
            //    var datos_encriptados = encriptacionServices.EncriptarCadena(datos_clave);
            //    var datos_encriptados_cadena = JsonConvert.SerializeObject(datos_encriptados);


            //    usuario.Clave = datos_encriptados_cadena;

            //    _unitOfWork.UsuarioRepository.Update(usuario);
            //    await _unitOfWork.SaveChangesAsync();

            //    booOk = true;
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}

            return booOk;
        }
    }
}
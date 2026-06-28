using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Administracion.Sesion;

namespace WebVisitsMobile.Services.Services.Administracion.Sesion
{
    public class SesionService : ISesionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOption;
        public SesionService(
             IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOption = options.Value;
        }

        public async Task<bool> Insert(Domain.Entities.Administracion.Sesion.Sesion SesionUsuario, Guid UsuarioActualId, Guid EmpresaId)
        {
            bool booOk = false;

            try
            {
                if (UsuarioActualId == Guid.Empty)
                {
                    return false;
                }

                if (SesionUsuario.Id == Guid.Empty)
                {
                    SesionUsuario.Id = Guid.NewGuid();
                }

                SesionUsuario.Estado = 1;
                SesionUsuario.FechaCreacion = DateTime.Now;
                SesionUsuario.UsuarioCreadorId = UsuarioActualId;


                await _unitOfWork.SesionRepository.Add(SesionUsuario);
                await _unitOfWork.SaveChangesAsync();

                if (SesionUsuario.UsuarioId != null)
                {
                    bool CerrarSession = await CerrarSesion((Guid)SesionUsuario.UsuarioId, SesionUsuario);
                }

                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> CerrarSesion(Guid idUsuario, Domain.Entities.Administracion.Sesion.Sesion sesionUsuarioCurrent)
        {
            try
            {
                Domain.Entities.Administracion.Sesion.Sesion sesionUsuario = await GetSesionActivoByUsuario(idUsuario);

                if (sesionUsuario == null)
                {
                    return true;
                }

                if (sesionUsuario.DireccionIp != sesionUsuarioCurrent.DireccionIp)
                {
                    bool inactivar = await CerrarSesionesUsuario(idUsuario, sesionUsuarioCurrent);
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<Domain.Entities.Administracion.Sesion.Sesion> GetSesionActivoByUsuario(Guid idUsuario)
        {
            try
            {
                IEnumerable<Domain.Entities.Administracion.Sesion.Sesion> sesionUsuarios = _unitOfWork.SesionRepository.GetAll().Where(w => w.UsuarioId == idUsuario && w.Estado == 1);

                if (sesionUsuarios.Count() == 0)
                {
                    return null;
                }
                Domain.Entities.Administracion.Sesion.Sesion sesion = sesionUsuarios.FirstOrDefault()!;

                return sesion;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<bool> CerrarSesionesUsuario(Guid idUsuario, Domain.Entities.Administracion.Sesion.Sesion SesionUsuario)
        {
            try
            {
                IEnumerable<Domain.Entities.Administracion.Sesion.Sesion> sesionUsuarios = _unitOfWork.SesionRepository.GetAll().Where(w => w.UsuarioId == idUsuario && w.Estado == 1);

                sesionUsuarios = sesionUsuarios.Where(x => x.Id != SesionUsuario.Id);

                if (sesionUsuarios.Count() == 0)
                {
                    return true;
                }

                foreach (var item in sesionUsuarios.ToList())
                {
                    bool inactivar = await InactivarSesionUsuario(item.Id, SesionUsuario.UsuarioCreadorId, new Guid());
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> InactivarSesionUsuario(Guid id, Guid usuarioActualId, Guid empresaId)
        {
            bool booOk = false;

            try
            {
                if (usuarioActualId == Guid.Empty) { return false; }

                Domain.Entities.Administracion.Sesion.Sesion userGroup = await GetById(id, empresaId);

                if (userGroup == null) { return false; }

                userGroup.FechaBaja = DateTime.Now;
                userGroup.UsuarioBajaId = usuarioActualId;
                userGroup.Estado = 2;
                userGroup.FechaFin = DateTime.Now;

                _unitOfWork.SesionRepository.Update(userGroup);

                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                return false;
            }

            return booOk;
        }

        public async Task<Domain.Entities.Administracion.Sesion.Sesion> GetById(Guid id, Guid empresaId)
        {
            try
            {
                return await _unitOfWork.SesionRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> VerifyFirstConnection(Guid userId)
        {
            bool booOk = false;
            try
            {
                int numeroSesiones = await _unitOfWork.SesionRepository.NumberOfSessions(userId);

                if (numeroSesiones <= 1)
                {
                    booOk = true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }

            return booOk;
        }
    }
}
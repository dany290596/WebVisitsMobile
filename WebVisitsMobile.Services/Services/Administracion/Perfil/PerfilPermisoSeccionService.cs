using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Administracion.Perfil;
using WebVisitsMobile.Services.Interfaces.Administracion.Perfil;

namespace WebVisitsMobile.Services.Services.Administracion.Perfil
{
    public class PerfilPermisoSeccionService : IPerfilPermisoSeccionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PerfilPermisoSeccionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Create(PerfilPermisoSeccion perfilPermisoSeccion, Guid currentUserId)
        {
            perfilPermisoSeccion.Id = Guid.NewGuid();
            perfilPermisoSeccion.Estado = 1;
            perfilPermisoSeccion.FechaCreacion = DateTime.Now;
            perfilPermisoSeccion.UsuarioCreadorId = currentUserId;

            await _unitOfWork.PerfilPermisoSeccionRepository.Add(perfilPermisoSeccion);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> Inactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;
            try
            {
                PerfilPermisoSeccion data = await _unitOfWork.PerfilPermisoSeccionRepository.GetById(id);
                data.FechaBaja = DateTime.Now;
                data.UsuarioBajaId = currentUserId;
                data.Estado = 2;

                _unitOfWork.PerfilPermisoSeccionRepository.Update(data);
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
                PerfilPermisoSeccion data = await _unitOfWork.PerfilPermisoSeccionRepository.GetById(id);
                data.FechaReactivacion = DateTime.Now;
                data.UsuarioReactivadorId = currentUserId;
                data.Estado = 1;

                _unitOfWork.PerfilPermisoSeccionRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> DeleteByProfile(Guid profileId)
        {
            try
            {
                if (profileId == Guid.Empty) { return false; }

                _unitOfWork.PerfilPermisoSeccionRepository.DeleteByProfile(profileId);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> PostProfilePermissionSectionMultiple(ICollection<PerfilPermisoSeccion> permissions, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                if (permissions.Count > 0)
                {
                    foreach (PerfilPermisoSeccion item in permissions)
                    {
                        await Create(item, currentUserId);
                    }
                }

                booOk = true;
            }
            catch (Exception ex)
            {

            }

            return booOk;
        }
    }
}
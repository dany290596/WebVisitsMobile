using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Administracion.Perfil;
using WebVisitsMobile.Services.QueryFilters.Administracion.Perfil;

namespace WebVisitsMobile.Services.Services.Administracion.Perfil
{
    public class PerfilService : IPerfilService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        private readonly IPerfilPermisoSeccionService _perfilPermisoSeccionService;

        public PerfilService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options,
            IPerfilPermisoSeccionService perfilPermisoSeccionService
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
            _perfilPermisoSeccionService = perfilPermisoSeccionService;
        }

        public async Task<Domain.Entities.Administracion.Perfil.Perfil?> GetById(Guid id, Guid empresaId)
        {
            try
            {
                Domain.Entities.Administracion.Perfil.Perfil perfil = await _unitOfWork.PerfilRepository.GetById(id);
                if (empresaId == Guid.Empty || perfil == null)
                {
                    return null;
                }

                return perfil;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PagedList<Domain.Entities.Administracion.Perfil.Perfil> GetAll(PerfilQueryFilter filters, Guid empresaId)
        {
            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<Domain.Entities.Administracion.Perfil.Perfil> data;

                if (filters.DatosCompletos == 0)
                {
                    data = _unitOfWork.PerfilRepository.GetAll();
                }
                else
                {
                    data = _unitOfWork.PerfilRepository.GetAll();
                }

                if (filters.Nombre != null) { data = data.Where(x => x.Nombre.ToLower().Contains(filters.Nombre.ToLower())); }

                if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { data = data.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
                if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { data = data.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
                if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { data = data.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
                if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { data = data.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
                if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
                if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
                if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
                if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
                if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
                if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
                if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
                if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
                if (filters.Estado != null && filters.Estado > 0) { data = data.Where(x => x.Estado == filters.Estado); }

                var paged = PagedList<Domain.Entities.Administracion.Perfil.Perfil>.Create(data, filters.PageNumber, filters.PageSize);

                return paged;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Inactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;
            try
            {
                Domain.Entities.Administracion.Perfil.Perfil data = await _unitOfWork.PerfilRepository.GetById(id);
                if (data == null) { return false; }
                data.FechaBaja = DateTime.Now;
                data.UsuarioBajaId = currentUserId;
                data.Estado = 2;

                _unitOfWork.PerfilRepository.Update(data);
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
                Domain.Entities.Administracion.Perfil.Perfil data = await _unitOfWork.PerfilRepository.GetById(id);
                if (data == null) { return false; }
                data.FechaReactivacion = DateTime.Now;
                data.UsuarioReactivadorId = currentUserId;
                data.Estado = 1;

                _unitOfWork.PerfilRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> Create(Domain.Entities.Administracion.Perfil.Perfil perfil, Guid currentUserId, Guid clientCompanyId)
        {
            bool booOk = false;

            try
            {
                if (currentUserId == Guid.Empty) { return false; }

                perfil.Id = Guid.NewGuid();
                perfil.Estado = 1;
                perfil.FechaCreacion = DateTime.Now;
                perfil.UsuarioCreadorId = currentUserId;

                if (perfil.PerfilPermisoSecciones.Count > 0)
                {
                    perfil.PerfilPermisoSecciones.ToList().ForEach(x =>
                    {
                        x.Id = Guid.NewGuid();
                        x.Estado = 1;
                        x.FechaCreacion = DateTime.Now;
                        x.UsuarioCreadorId = currentUserId;
                    });
                }

                await _unitOfWork.PerfilRepository.Add(perfil);

                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> Update(Domain.Entities.Administracion.Perfil.Perfil perfil, Guid currentUserId, Guid clientCompanyId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                if (perfil.Id == Guid.Empty) { return false; }

                Domain.Entities.Administracion.Perfil.Perfil currentPerfil = await _unitOfWork.PerfilRepository.GetById(perfil.Id);

                if (currentPerfil == null) { return false; }

                currentPerfil.Nombre = perfil.Nombre;
                currentPerfil.FechaModificacion = DateTime.Now;
                currentPerfil.UsuarioModificadorId = currentUserId;

                _unitOfWork.PerfilRepository.Update(currentPerfil);

                await _unitOfWork.SaveChangesAsync();

                bool ok = await _perfilPermisoSeccionService.DeleteByProfile(currentPerfil.Id);

                if (ok && perfil.PerfilPermisoSecciones.Count > 0)
                {
                    await _perfilPermisoSeccionService.PostProfilePermissionSectionMultiple(perfil.PerfilPermisoSecciones, currentUserId);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Domain.Entities.Administracion.Perfil.Perfil> GetPerfilConPermisos(Guid id)
        {
            Domain.Entities.Administracion.Perfil.Perfil perfil = await _unitOfWork.PerfilRepository.GetByIdConPermisos(id);
            return perfil;
        }

        public async Task<bool> ExistsName(string name)
        {
            try
            {
                var data = await _unitOfWork.PerfilRepository.GetProfile(n => n.Nombre == name);
                if (data == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ExistsNameForUpdate(Guid perfilId, string name)
        {
            var currentPerfil = await _unitOfWork.PerfilRepository.GetById(perfilId);

            if (currentPerfil == null)
            {
                return false;
            }

            // Si el nombre no cambió, permitir guardar
            if (currentPerfil.Nombre.Trim().ToLower() == name.Trim().ToLower())
            {
                return false;
            }

            var data = await _unitOfWork.PerfilRepository.GetProfile(
                x => x.Nombre.Trim().ToLower() == name.Trim().ToLower()
            );

            return data != null;
        }
    }
}
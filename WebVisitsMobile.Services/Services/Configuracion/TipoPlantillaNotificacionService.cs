using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.QueryFilters.Configuracion;

namespace WebVisitsMobile.Services.Services.Configuracion
{
    public class TipoPlantillaNotificacionService : ITipoPlantillaNotificacionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;

        public TipoPlantillaNotificacionService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<TipoPlantillaNotificacion> GetById(Guid id, Guid empresaId)
        {
            try
            {
                TipoPlantillaNotificacion data = await _unitOfWork.TipoPlantillaNotificacionRepository.GetById(id);
                if (empresaId == Guid.Empty || data == null)
                {
                    return null;
                }

                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PagedList<TipoPlantillaNotificacion> GetAll(TipoPlantillaNotificacionQueryFilter filters, Guid empresaId)
        {
            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<TipoPlantillaNotificacion> data;

                if (filters.DatosCompletos == 0)
                {
                    data = _unitOfWork.TipoPlantillaNotificacionRepository.GetAll();
                }
                else
                {
                    data = _unitOfWork.TipoPlantillaNotificacionRepository.GetAll();
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

                var paged = PagedList<TipoPlantillaNotificacion>.Create(data, filters.PageNumber, filters.PageSize);

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
                TipoPlantillaNotificacion data = await _unitOfWork.TipoPlantillaNotificacionRepository.GetById(id);
                if (data == null) { return false; }
                data.FechaBaja = DateTime.Now;
                data.UsuarioBajaId = currentUserId;
                data.Estado = 2;

                _unitOfWork.TipoPlantillaNotificacionRepository.Update(data);
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
                TipoPlantillaNotificacion data = await _unitOfWork.TipoPlantillaNotificacionRepository.GetById(id);
                if (data == null) { return false; }
                data.FechaReactivacion = DateTime.Now;
                data.UsuarioReactivadorId = currentUserId;
                data.Estado = 1;

                _unitOfWork.TipoPlantillaNotificacionRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> Create(TipoPlantillaNotificacion data, Guid currentUserId, Guid clientCompanyId)
        {
            bool booOk = false;

            try
            {
                if (currentUserId == Guid.Empty) { return false; }

                data.Id = Guid.NewGuid();
                data.Estado = 1;
                data.FechaCreacion = DateTime.Now;
                data.UsuarioCreadorId = currentUserId;

                await _unitOfWork.TipoPlantillaNotificacionRepository.Add(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> Update(TipoPlantillaNotificacion data, Guid currentUserId, Guid clientCompanyId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                if (data.Id == Guid.Empty) { return false; }

                TipoPlantillaNotificacion current = await _unitOfWork.TipoPlantillaNotificacionRepository.GetById(data.Id);

                if (current == null) { return false; }

                current.Nombre = data.Nombre;
                current.FechaModificacion = DateTime.Now;
                current.UsuarioModificadorId = currentUserId;

                _unitOfWork.TipoPlantillaNotificacionRepository.Update(current);

                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
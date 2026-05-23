using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Ubicacion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Ubicacion;
using WebVisitsMobile.Services.QueryFilters.Ubicacion;

namespace WebVisitsMobile.Services.Services.Ubicacion
{
    public class PaisService : IPaisService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        public PaisService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<PagedList<Pais>> GetAll(PaisQueryFilter filters, Guid empresaId)
        {
            PagedList<Pais> pagedData = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<Pais> data;

                if (filters.DatosCompletos == 0)
                {
                    data = _unitOfWork.PaisRepository.GetAll();
                }
                else
                {
                    data = _unitOfWork.PaisRepository.GetAll();
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

                pagedData = PagedList<Pais>.Create(data, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedData;
        }

        public async Task<Pais?> GetById(Guid dataId, Guid empresaId)
        {
            try
            {
                Pais data = await _unitOfWork.PaisRepository.GetById(dataId);
                if (data == null) { return null; }
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Pais?> Create(Pais data, Guid currentUserId)
        {
            try
            {
                data.Id = Guid.NewGuid();
                data.UsuarioCreadorId = currentUserId;
                data.FechaCreacion = DateTime.Now;
                data.Estado = 1;

                await _unitOfWork.PaisRepository.Add(data);
                await _unitOfWork.SaveChangesAsync();

                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> Update(Pais data, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }

                Pais dataUpdate = await _unitOfWork.PaisRepository.GetById(data.Id);
                if (dataUpdate == null) { return false; }

                dataUpdate.Nombre = data.Nombre;

                dataUpdate.FechaModificacion = DateTime.Now;
                dataUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.PaisRepository.Update(dataUpdate);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Inactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;
            try
            {
                Pais data = await _unitOfWork.PaisRepository.GetById(id);
                if (data == null) { return false; }

                data.FechaBaja = DateTime.Now;
                data.UsuarioBajaId = currentUserId;
                data.Estado = 2;

                _unitOfWork.PaisRepository.Update(data);
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
                Pais data = await _unitOfWork.PaisRepository.GetById(id);
                if (data == null) { return false; }

                data.FechaReactivacion = DateTime.Now;
                data.UsuarioReactivadorId = currentUserId;
                data.Estado = 1;

                _unitOfWork.PaisRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }
    }
}
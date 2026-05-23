using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Services.HID
{
    public class TipoCredencialService : ITipoCredencialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        public TipoCredencialService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<PagedList<TipoCredencial>> GetAll(TipoCredencialQueryFilter filters)
        {
            PagedList<TipoCredencial> pagedCredentialDevice = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<TipoCredencial> data;

                if (filters.DatosCompletos == 0)
                {
                    data = _unitOfWork.TipoCredencialRepository.GetAll();
                }
                else
                {
                    data = _unitOfWork.TipoCredencialRepository.GetAll();
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

                pagedCredentialDevice = PagedList<TipoCredencial>.Create(data, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedCredentialDevice;
        }

        public async Task<TipoCredencial> GetById(Guid dataId)
        {
            try
            {
                TipoCredencial data = await _unitOfWork.TipoCredencialRepository.GetById(dataId);
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TipoCredencial?> Create(TipoCredencial data, Guid currentUserId)
        {
            try
            {
                data.Id = Guid.NewGuid();
                data.UsuarioCreadorId = currentUserId;
                data.FechaCreacion = DateTime.Now;
                data.Estado = 1;
                await _unitOfWork.TipoCredencialRepository.Add(data);
                await _unitOfWork.SaveChangesAsync();

                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> Update(TipoCredencial data, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }

                TipoCredencial dataUpdate = await _unitOfWork.TipoCredencialRepository.GetById(data.Id);

                if (dataUpdate == null) { return false; }

                dataUpdate.Nombre = data.Nombre;

                dataUpdate.FechaModificacion = DateTime.Now;
                dataUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.TipoCredencialRepository.Update(dataUpdate);
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
                TipoCredencial data = await _unitOfWork.TipoCredencialRepository.GetById(id);
                data.FechaBaja = DateTime.Now;
                data.UsuarioBajaId = currentUserId;
                data.Estado = 2;

                _unitOfWork.TipoCredencialRepository.Update(data);
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
                TipoCredencial data = await _unitOfWork.TipoCredencialRepository.GetById(id);
                data.FechaReactivacion = DateTime.Now;
                data.UsuarioReactivadorId = currentUserId;
                data.Estado = 1;

                _unitOfWork.TipoCredencialRepository.Update(data);
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
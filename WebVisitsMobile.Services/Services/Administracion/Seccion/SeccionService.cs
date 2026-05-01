using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Administracion.Modulo;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Administracion.Seccion;
using WebVisitsMobile.Services.QueryFilters.Administracion.Seccion;
using WebVisitsMobile.Services.QueryFilters.Administracion.Sesion;

namespace WebVisitsMobile.Services.Services.Administracion.Seccion
{
    public class SeccionService : ISeccionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        public SeccionService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<IEnumerable<SeccionesPorModulo>> GetSectionsGroupedByModule(MenuQueryFilter filters)
        {
            try
            {
                return await _unitOfWork.SeccionRepository.GetSectionsGroupedByModule(filters.PerfilId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Domain.Entities.Administracion.Seccion.Seccion?> GetById(Guid id, Guid empresaId)
        {
            try
            {
                Domain.Entities.Administracion.Seccion.Seccion data = await _unitOfWork.SeccionRepository.GetById(id);
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

        public async Task<PagedList<Domain.Entities.Administracion.Seccion.Seccion>> GetAll(SeccionQueryFilter filters, Guid empresaId)
        {
            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<Domain.Entities.Administracion.Seccion.Seccion> data;

                if (filters.DatosCompletos == 0)
                {
                    data = _unitOfWork.SeccionRepository.GetAll();
                }
                else
                {
                    data = await _unitOfWork.SeccionRepository.GetAllSection();
                }

                if (filters.Nombre != null) { data = data.Where(x => x.Nombre.ToLower().Contains(filters.Nombre.ToLower())); }
                if (filters.ModuloId != null && filters.ModuloId != Guid.Empty) { data = data.Where(x => x.ModuloId == filters.ModuloId); }
                if (filters.Path != null) { data = data.Where(x => x.Path.ToLower().Contains(filters.Path.ToLower())); }
                if (filters.Orden != null && filters.Orden > 0) { data = data.Where(x => x.Orden == filters.Orden); }

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

                var paged = PagedList<Domain.Entities.Administracion.Seccion.Seccion>.Create(data, filters.PageNumber, filters.PageSize);

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
                Domain.Entities.Administracion.Seccion.Seccion data = await _unitOfWork.SeccionRepository.GetById(id);
                data.FechaBaja = DateTime.Now;
                data.UsuarioBajaId = currentUserId;
                data.Estado = 2;

                _unitOfWork.SeccionRepository.Update(data);
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
                Domain.Entities.Administracion.Seccion.Seccion data = await _unitOfWork.SeccionRepository.GetById(id);
                data.FechaReactivacion = DateTime.Now;
                data.UsuarioReactivadorId = currentUserId;
                data.Estado = 1;

                _unitOfWork.SeccionRepository.Update(data);
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
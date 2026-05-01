using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Administracion.Aplicacion;
using WebVisitsMobile.Services.QueryFilters.Administracion.Aplicacion;

namespace WebVisitsMobile.Services.Services.Administracion.Aplicacion
{
    public class AplicacionService : IAplicacionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        public AplicacionService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<Domain.Entities.Administracion.Aplicacion.Aplicacion?> GetById(Guid id, Guid empresaId)
        {
            try
            {
                Domain.Entities.Administracion.Aplicacion.Aplicacion perfil = await _unitOfWork.AplicacionRepository.GetById(id);
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

        public PagedList<Domain.Entities.Administracion.Aplicacion.Aplicacion> GetAll(AplicacionQueryFilter filters, Guid empresaId)
        {
            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<Domain.Entities.Administracion.Aplicacion.Aplicacion> data;

                if (filters.DatosCompletos == 0)
                {
                    data = _unitOfWork.AplicacionRepository.GetAll();
                }
                else
                {
                    data = _unitOfWork.AplicacionRepository.GetAll();
                }

                if (filters.Nombre != null) { data = data.Where(x => x.Nombre.ToLower().Contains(filters.Nombre.ToLower())); }
                if (filters.Estado != null && filters.Estado > 0) { data = data.Where(x => x.Estado == filters.Estado); }

                var paged = PagedList<Domain.Entities.Administracion.Aplicacion.Aplicacion>.Create(data, filters.PageNumber, filters.PageSize);

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
                Domain.Entities.Administracion.Aplicacion.Aplicacion data = await _unitOfWork.AplicacionRepository.GetById(id);
                data.FechaBaja = DateTime.Now;
                data.UsuarioBajaId = currentUserId;
                data.Estado = 2;

                _unitOfWork.AplicacionRepository.Update(data);
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
                Domain.Entities.Administracion.Aplicacion.Aplicacion data = await _unitOfWork.AplicacionRepository.GetById(id);
                data.FechaReactivacion = DateTime.Now;
                data.UsuarioReactivadorId = currentUserId;
                data.Estado = 1;

                _unitOfWork.AplicacionRepository.Update(data);
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
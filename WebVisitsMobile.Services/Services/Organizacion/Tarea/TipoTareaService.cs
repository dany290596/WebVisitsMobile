using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Organizacion.Tarea;
using WebVisitsMobile.Services.QueryFilters.Organizacion.Tarea;

namespace WebVisitsMobile.Services.Services.Organizacion.Tarea
{
    public class TipoTareaService : ITipoTareaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        public TipoTareaService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<PagedList<TipoTarea>> GetAll(TipoTareaQueryFilter filters)
        {
            PagedList<TipoTarea> pagedTaskType = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                var taskType = _unitOfWork.TipoTareaRepository.GetAll();

                if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { taskType = taskType.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
                if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { taskType = taskType.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
                if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { taskType = taskType.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
                if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { taskType = taskType.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
                if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { taskType = taskType.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
                if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { taskType = taskType.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
                if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { taskType = taskType.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
                if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { taskType = taskType.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
                if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { taskType = taskType.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
                if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { taskType = taskType.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
                if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { taskType = taskType.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
                if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { taskType = taskType.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
                if (filters.Estado != null && filters.Estado > 0) { taskType = taskType.Where(x => x.Estado == filters.Estado); }

                pagedTaskType = PagedList<TipoTarea>.Create(taskType, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedTaskType;
        }

        public async Task<bool> Create(TipoTarea taskType, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                taskType.Id = Guid.NewGuid();
                taskType.UsuarioCreadorId = currentUserId;
                taskType.FechaCreacion = DateTime.Now;
                taskType.Estado = 1;

                await _unitOfWork.TipoTareaRepository.Add(taskType);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;

            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> Update(TipoTarea taskType, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                TipoTarea taskTypeUpdate = await _unitOfWork.TipoTareaRepository.GetById(taskType.Id);

                if (taskTypeUpdate == null) { return false; }
                taskTypeUpdate.Nombre = taskType.Nombre;
                taskTypeUpdate.FechaModificacion = DateTime.Now;
                taskTypeUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.TipoTareaRepository.Update(taskTypeUpdate);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Reactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;
            try
            {
                TipoTarea taskType = await _unitOfWork.TipoTareaRepository.GetById(id);
                taskType.FechaReactivacion = DateTime.Now;
                taskType.UsuarioReactivadorId = currentUserId;
                taskType.Estado = 1;

                _unitOfWork.TipoTareaRepository.Update(taskType);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> Inactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;
            try
            {
                TipoTarea taskType = await _unitOfWork.TipoTareaRepository.GetById(id);
                taskType.FechaBaja = DateTime.Now;
                taskType.UsuarioBajaId = currentUserId;
                taskType.Estado = 2;

                _unitOfWork.TipoTareaRepository.Update(taskType);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<TipoTarea> GetById(Guid taskTypeId)
        {
            try
            {
                TipoTarea taskType = await _unitOfWork.TipoTareaRepository.GetById(taskTypeId);
                return taskType;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
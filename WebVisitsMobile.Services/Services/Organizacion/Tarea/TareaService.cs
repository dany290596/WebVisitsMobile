using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Models.Organizacion.Tarea.Tarea;
using WebVisitsMobile.Services.Interfaces.Organizacion.Tarea;
using WebVisitsMobile.Services.QueryFilters.Common;
using WebVisitsMobile.Services.QueryFilters.Organizacion.Tarea;

namespace WebVisitsMobile.Services.Services.Organizacion.Tarea
{
    public class TareaService : ITareaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        public TareaService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<PagedList<Domain.Entities.Organizacion.Tarea.Tarea>> GetAll(TareaQueryFilter filters)
        {
            PagedList<Domain.Entities.Organizacion.Tarea.Tarea> pagedTask = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                var task = await _unitOfWork.TareaRepository.GetAllTask();

                if (filters.Pendiente != null && filters.Pendiente != 0) { task = task.Where(x => x.Pendiente == filters.Pendiente); }
                if (filters.ReferenciaId != null && filters.ReferenciaId != Guid.Empty) { task = task.Where(x => x.ReferenciaId == filters.ReferenciaId); }
                if (filters.TipoTareaId != null && filters.TipoTareaId != Guid.Empty) { task = task.Where(x => x.TipoTareaId == filters.TipoTareaId); }
                if (filters.EmpresaClienteId != null && filters.EmpresaClienteId != Guid.Empty) { task = task.Where(x => x.EmpresaClienteId == filters.EmpresaClienteId); }

                if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { task = task.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
                if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { task = task.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
                if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { task = task.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
                if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { task = task.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
                if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { task = task.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
                if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { task = task.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
                if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { task = task.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
                if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { task = task.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
                if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { task = task.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
                if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { task = task.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
                if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { task = task.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
                if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { task = task.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
                if (filters.Estado != null && filters.Estado > 0) { task = task.Where(x => x.Estado == filters.Estado); }

                pagedTask = PagedList<Domain.Entities.Organizacion.Tarea.Tarea>.Create(task, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedTask;
        }

        public async Task<Domain.Entities.Organizacion.Tarea.Tarea> GetById(Guid taskId)
        {
            try
            {
                Domain.Entities.Organizacion.Tarea.Tarea task = await _unitOfWork.TareaRepository.GetTask(t => t.Id == taskId);
                return task;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Domain.Entities.Organizacion.Tarea.Tarea?> Create(Domain.Entities.Organizacion.Tarea.Tarea task, Guid currentUserId)
        {
            try
            {
                task.Id = Guid.NewGuid();
                task.UsuarioCreadorId = currentUserId;
                task.FechaCreacion = DateTime.Now;
                task.Estado = 1;

                await _unitOfWork.TareaRepository.Add(task);
                await _unitOfWork.SaveChangesAsync();

                return task;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> Update(Domain.Entities.Organizacion.Tarea.Tarea task, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }

                Domain.Entities.Organizacion.Tarea.Tarea taskUpdate = await _unitOfWork.TareaRepository.GetById(task.Id);

                if (taskUpdate == null) { return false; }

                taskUpdate.TipoTareaId = task.TipoTareaId;
                taskUpdate.Fecha = task.Fecha;
                taskUpdate.Pendiente = task.Pendiente;
                taskUpdate.Status = task.Status;
                taskUpdate.ValorEnvio = task.ValorEnvio;
                taskUpdate.ValorRetorno = task.ValorRetorno;
                taskUpdate.Marca = task.Marca;
                taskUpdate.FechaModificacion = DateTime.Now;
                taskUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.TareaRepository.Update(taskUpdate);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdatePending(Guid taskId, TareaPendingDTO task)
        {
            try
            {
                if (taskId == Guid.Empty) { return false; }
                Domain.Entities.Organizacion.Tarea.Tarea taskUpdate = await _unitOfWork.TareaRepository.GetById(taskId);
                if (taskUpdate == null) { return false; }
                taskUpdate.Pendiente = task.Pendiente;
                taskUpdate.ValorRetorno = task.ValorRetorno;
                taskUpdate.Marca = task.Marca;
                taskUpdate.FechaModificacion = DateTime.Now;
                taskUpdate.UsuarioModificadorId = task.UsuarioModificadorId;
                taskUpdate.Estado = 2;

                _unitOfWork.TareaRepository.Update(taskUpdate);
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
                if (currentUserId == Guid.Empty) { return false; }
                Domain.Entities.Organizacion.Tarea.Tarea task = await _unitOfWork.TareaRepository.GetById(id);
                if (task == null) { return false; }
                task.Pendiente = 3;
                task.FechaBaja = DateTime.Now;
                task.UsuarioBajaId = currentUserId;
                task.Estado = 2;

                _unitOfWork.TareaRepository.Update(task);
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
                Domain.Entities.Organizacion.Tarea.Tarea task = await _unitOfWork.TareaRepository.GetById(id);
                task.FechaReactivacion = DateTime.Now;
                task.UsuarioReactivadorId = currentUserId;
                task.Estado = 1;

                _unitOfWork.TareaRepository.Update(task);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<PagedList<TareaHID<T>>> GetAllByUserWallet<T>(BaseQueryFilter filters, Guid typeTaskId)
        {
            PagedList<TareaHID<T>> pagedTask = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                var task = await _unitOfWork.TareaRepository.GetAllByUserWallet<T>(typeTaskId);

                pagedTask = PagedList<TareaHID<T>>.Create(task, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedTask;
        }

        public async Task<PagedList<TareaHID<TareaPlantilla>>> GetAllByTemplate(BaseQueryFilter filters)
        {
            PagedList<TareaHID<TareaPlantilla>> pagedTask = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                var task = await _unitOfWork.TareaRepository.GetAllByTemplate();

                pagedTask = PagedList<TareaHID<TareaPlantilla>>.Create(task, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedTask;
        }
    }
}
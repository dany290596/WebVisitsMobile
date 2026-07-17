using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.QueryFilters.Configuracion;

namespace WebVisitsMobile.Services.Services.Configuracion
{
    public class PlantillaNotificacionService : IPlantillaNotificacionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;

        public PlantillaNotificacionService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<PlantillaNotificacion> GetById(Guid id, Guid clientCompanyId)
        {
            try
            {
                PlantillaNotificacion data = await _unitOfWork.PlantillaNotificacionRepository.GetNotificationTemplate(t => t.Id == id);
                if (clientCompanyId == Guid.Empty || data == null)
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

        public async Task<PlantillaNotificacion> GetByNotificationTemplate(Guid notificationTemplateTypeId, Guid clientCompanyId)
        {
            try
            {
                PlantillaNotificacion data = await _unitOfWork.PlantillaNotificacionRepository.GetNotificationTemplate(t => t.TipoPlantillaNotificacionId == notificationTemplateTypeId && t.EmpresaClienteId == clientCompanyId);
                if (clientCompanyId == Guid.Empty || data == null)
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

        public async Task<PagedList<PlantillaNotificacion>> GetAll(PlantillaNotificacionQueryFilter filters, Guid empresaId)
        {
            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<PlantillaNotificacion> data;

                if (filters.DatosCompletos == 0)
                {
                    data = _unitOfWork.PlantillaNotificacionRepository.GetAll();
                }
                else
                {
                    data = await _unitOfWork.PlantillaNotificacionRepository.GetAllNotificationTemplate();
                }

                if (filters.Nombre != null) { data = data.Where(x => x.Nombre.ToLower().Contains(filters.Nombre.ToLower())); }
                if (filters.EmpresaClienteId != null && filters.EmpresaClienteId != Guid.Empty) { data = data.Where(x => x.EmpresaClienteId == filters.EmpresaClienteId); }

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

                var paged = PagedList<PlantillaNotificacion>.Create(data, filters.PageNumber, filters.PageSize);

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
                PlantillaNotificacion data = await _unitOfWork.PlantillaNotificacionRepository.GetById(id);
                if (data == null) { return false; }
                data.FechaBaja = DateTime.Now;
                data.UsuarioBajaId = currentUserId;
                data.Estado = 2;

                _unitOfWork.PlantillaNotificacionRepository.Update(data);
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
                PlantillaNotificacion data = await _unitOfWork.PlantillaNotificacionRepository.GetById(id);
                if (data == null) { return false; }
                data.FechaReactivacion = DateTime.Now;
                data.UsuarioReactivadorId = currentUserId;
                data.Estado = 1;

                _unitOfWork.PlantillaNotificacionRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> Create(PlantillaNotificacion data, Guid currentUserId, Guid clientCompanyId)
        {
            bool booOk = false;

            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                if (clientCompanyId == Guid.Empty) { return false; }

                data.Id = Guid.NewGuid();
                data.Estado = 1;
                data.FechaCreacion = DateTime.Now;
                data.UsuarioCreadorId = currentUserId;

                await _unitOfWork.PlantillaNotificacionRepository.Add(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> Update(PlantillaNotificacion data, Guid currentUserId, Guid clientCompanyId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                if (data.Id == Guid.Empty) { return false; }
                if (data.TipoPlantillaNotificacionId == Guid.Empty) { return false; }

                PlantillaNotificacion current = await _unitOfWork.PlantillaNotificacionRepository.GetById(data.Id);

                if (current == null) { return false; }

                current.Nombre = data.Nombre;
                current.TipoPlantillaNotificacionId = data.TipoPlantillaNotificacionId;
                current.CuerpoPlantilla = data.CuerpoPlantilla;
                current.NotificarEmail = data.NotificarEmail;
                current.NotificarTeams = data.NotificarTeams;

                current.FechaModificacion = DateTime.Now;
                current.UsuarioModificadorId = currentUserId;

                _unitOfWork.PlantillaNotificacionRepository.Update(current);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ExistsName(string name)
        {
            try
            {
                var data = await _unitOfWork.PlantillaNotificacionRepository.GetNotificationTemplate(n => n.Nombre == name);
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

        public async Task<bool> ExistsNameForUpdate(Guid id, string name)
        {
            var current = await _unitOfWork.PlantillaNotificacionRepository.GetById(id);
            if (current == null)
            {
                return false;
            }

            if (current.Nombre.Trim().ToLower() == name.Trim().ToLower())
            {
                return false;
            }

            var data = await _unitOfWork.PlantillaNotificacionRepository.GetNotificationTemplate(
                x => x.Nombre.Trim().ToLower() == name.Trim().ToLower()
            );

            return data != null;
        }

        public async Task<bool> ExistsTipoPlantilla(Guid notificationTemplateTypeId, Guid clientCompanyId)
        {
            try
            {
                var data = await _unitOfWork.PlantillaNotificacionRepository.GetNotificationTemplate(
                    x => x.TipoPlantillaNotificacionId == notificationTemplateTypeId
                         && x.EmpresaClienteId == clientCompanyId
                         && x.Estado != 2 // no cuenta contra plantillas inactivas/dadas de baja
                );
                return data != null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ExistsTipoPlantillaForUpdate(Guid id, Guid notificationTemplateTypeId, Guid clientCompanyId)
        {
            var current = await _unitOfWork.PlantillaNotificacionRepository.GetById(id);
            if (current == null)
            {
                return false;
            }

            // si no cambió el tipo, no hay nada que validar
            if (current.TipoPlantillaNotificacionId == notificationTemplateTypeId)
            {
                return false;
            }

            var data = await _unitOfWork.PlantillaNotificacionRepository.GetNotificationTemplate(
                x => x.TipoPlantillaNotificacionId == notificationTemplateTypeId
                     && x.EmpresaClienteId == clientCompanyId
                     && x.Estado != 2
                     && x.Id != id
            );

            return data != null;
        }
    }
}
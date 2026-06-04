using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Services.HID
{
    public class DipositivosHIDService : IDipositivosHIDService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;

        public DipositivosHIDService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<PagedList<DipositivosHid>> GetAll(DipositivosHIDQueryFilter filters, Guid empresaId)
        {
            PagedList<DipositivosHid> pagedCredentialDevice = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<DipositivosHid> deviceHID;

                if (filters.DatosCompletos == 0)
                {
                    deviceHID = _unitOfWork.DipositivosHIDRepository.GetAll();
                }
                else
                {
                    deviceHID = await _unitOfWork.DipositivosHIDRepository.GetAllDevice();
                }

                if (filters.CodigoInvitacion != null) { deviceHID = deviceHID.Where(x => x.CodigoInvitacion.ToLower().Contains(filters.CodigoInvitacion.ToLower())); }
                if (filters.NombreDispositivo != null) { deviceHID = deviceHID.Where(x => x.NombreDispositivo.ToLower().Contains(filters.NombreDispositivo.ToLower())); }
                if (filters.SdkVersion != null) { deviceHID = deviceHID.Where(x => x.SdkVersion.ToLower().Contains(filters.SdkVersion.ToLower())); }
                if (filters.SistemaOperativo != null) { deviceHID = deviceHID.Where(x => x.SistemaOperativo.ToLower().Contains(filters.SistemaOperativo.ToLower())); }
                if (filters.UsuarioId != null && filters.UsuarioId != Guid.Empty) { deviceHID = deviceHID.Where(x => x.UsuarioId == filters.UsuarioId); }

                if (filters.EmpresaClienteId != null && filters.EmpresaClienteId != Guid.Empty) { deviceHID = deviceHID.Where(x => x.EmpresaClienteId == filters.EmpresaClienteId); }

                if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { deviceHID = deviceHID.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
                if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { deviceHID = deviceHID.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
                if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { deviceHID = deviceHID.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
                if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { deviceHID = deviceHID.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
                if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { deviceHID = deviceHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
                if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { deviceHID = deviceHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
                if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { deviceHID = deviceHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
                if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { deviceHID = deviceHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
                if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { deviceHID = deviceHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
                if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { deviceHID = deviceHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
                if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { deviceHID = deviceHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
                if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { deviceHID = deviceHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
                if (filters.Estado != null && filters.Estado > 0) { deviceHID = deviceHID.Where(x => x.Estado == filters.Estado); }

                pagedCredentialDevice = PagedList<DipositivosHid>.Create(deviceHID, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedCredentialDevice;
        }

        public async Task<DipositivosHid> GetById(Guid deviceHIDId, Guid empresaId)
        {
            try
            {
                DipositivosHid deviceHID = await _unitOfWork.DipositivosHIDRepository.GetById(deviceHIDId);
                return deviceHID;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DipositivosHid?> Create(DipositivosHid deviceHID, Guid currentUserId)
        {
            try
            {
                deviceHID.Id = Guid.NewGuid();
                deviceHID.UsuarioCreadorId = currentUserId;
                deviceHID.FechaCreacion = DateTime.Now;
                deviceHID.Estado = 1;
                await _unitOfWork.DipositivosHIDRepository.Add(deviceHID);
                await _unitOfWork.SaveChangesAsync();

                return deviceHID;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> Update(DipositivosHid deviceHID, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }

                DipositivosHid deviceHIDUpdate = await _unitOfWork.DipositivosHIDRepository.GetById(deviceHID.Id);

                if (deviceHIDUpdate == null) { return false; }

                //deviceHIDUpdate.UsuarioId = deviceHID.UsuarioId;
                deviceHIDUpdate.SistemaOperativo = deviceHID.SistemaOperativo;
                deviceHIDUpdate.NombreDispositivo = deviceHID.NombreDispositivo;
                deviceHIDUpdate.EndpointId = deviceHID.EndpointId;
                deviceHIDUpdate.SdkVersion = deviceHID.SdkVersion;
                deviceHIDUpdate.FechaModificacion = DateTime.Now;
                deviceHIDUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.DipositivosHIDRepository.Update(deviceHIDUpdate);
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
                DipositivosHid deviceHID = await _unitOfWork.DipositivosHIDRepository.GetById(id);
                deviceHID.FechaBaja = DateTime.Now;
                deviceHID.UsuarioBajaId = currentUserId;
                deviceHID.Estado = 2;

                _unitOfWork.DipositivosHIDRepository.Update(deviceHID);
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
                DipositivosHid deviceHID = await _unitOfWork.DipositivosHIDRepository.GetById(id);
                deviceHID.FechaReactivacion = DateTime.Now;
                deviceHID.UsuarioReactivadorId = currentUserId;
                deviceHID.Estado = 1;

                _unitOfWork.DipositivosHIDRepository.Update(deviceHID);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<PagedList<CommonDTO>> GetAllQuery(DipositivosHIDQueryFilter filters)
        {
            PagedList<CommonDTO> pagedDevice = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<DipositivosHid> userHID = filters.DatosCompletos == 2 ? _unitOfWork.DipositivosHIDRepository.GetAll() : await _unitOfWork.DipositivosHIDRepository.GetAllDevice();
                if (filters.Estado != null && filters.Estado > 0) { userHID = userHID.Where(x => x.Estado == filters.Estado); }

                List<CommonDTO> userHIDSelect = new List<CommonDTO>();
                if (filters.TipoQuery == "DeviceByName")
                {
                    userHIDSelect = userHID.Select(
                        s => new CommonDTO()
                        {
                            Id = s.Id,
                            Nombre = s.NombreDispositivo ?? string.Empty
                        })
                        .GroupBy(x => x.Nombre, StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.First())
                        .ToList();
                }

                if (filters.TipoQuery == "DeviceByCodeInvitation")
                {
                    userHIDSelect = userHID.Select(
                        s => new CommonDTO()
                        {
                            Id = s.Id,
                            Nombre = s.CodigoInvitacion ?? string.Empty
                        })
                        .GroupBy(x => x.Nombre, StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.First())
                        .ToList();
                }

                if (filters.TipoQuery == "DeviceBySystem")
                {
                    userHIDSelect = userHID.Select(
                        s => new CommonDTO()
                        {
                            Id = s.Id,
                            Nombre = s.SistemaOperativo ?? string.Empty
                        })
                        .GroupBy(x => x.Nombre, StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.First())
                        .ToList();
                }

                pagedDevice = PagedList<CommonDTO>.Create(userHIDSelect, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedDevice;
        }

        public async Task<bool> UpdateStatus(Guid userId, int status)
        {
            try
            {
                DipositivosHid data = await _unitOfWork.DipositivosHIDRepository.GetDevice(u => u.LicenciaHidUser.UsuarioWalletId == userId);
                if (data == null) { return false; }

                data.Status = (byte?)status;
                data.FechaModificacion = DateTime.Now;

                _unitOfWork.DipositivosHIDRepository.Update(data);
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
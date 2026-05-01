using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Services.HID
{
    public class CredencialHIDService : ICredencialHIDService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;

        public CredencialHIDService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<PagedList<CredencialHid>> GetAll(CredencialHIDQueryFilter filters, Guid empresaId)
        {
            PagedList<CredencialHid> pagedCredentialDevice = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<CredencialHid> credentialHID;

                if (filters.DatosCompletos == 0)
                {
                    credentialHID = _unitOfWork.CredencialHIDRepository.GetAll();
                }
                else
                {
                    credentialHID = await _unitOfWork.CredencialHIDRepository.GetAllCredentialHID();
                }

                if (filters.TipoCredencial != null) { credentialHID = credentialHID.Where(x => x.TipoCredencial.ToLower().Contains(filters.TipoCredencial.ToLower())); }
                if (filters.DispositivoId != null && filters.DispositivoId != Guid.Empty) { credentialHID = credentialHID.Where(x => x.DispositivoId == filters.DispositivoId); }
                if (filters.Usuarioid != null && filters.Usuarioid != Guid.Empty) { credentialHID = credentialHID.Where(x => x.Usuarioid == filters.Usuarioid); }
                if (filters.CredencialValor != null) { credentialHID = credentialHID.Where(x => x.CredencialValor.ToLower().Contains(filters.CredencialValor.ToLower())); }
                if (filters.Validity != null) { credentialHID = credentialHID.Where(x => x.Validity.ToLower().Contains(filters.Validity.ToLower())); }
                if (filters.EmpresaClienteId != null && filters.EmpresaClienteId != Guid.Empty) { credentialHID = credentialHID.Where(x => x.EmpresaClienteId == filters.EmpresaClienteId); }

                if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { credentialHID = credentialHID.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
                if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { credentialHID = credentialHID.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
                if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { credentialHID = credentialHID.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
                if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { credentialHID = credentialHID.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
                if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { credentialHID = credentialHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
                if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { credentialHID = credentialHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
                if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { credentialHID = credentialHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
                if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { credentialHID = credentialHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
                if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { credentialHID = credentialHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
                if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { credentialHID = credentialHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
                if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { credentialHID = credentialHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
                if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { credentialHID = credentialHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
                if (filters.Estado != null && filters.Estado > 0) { credentialHID = credentialHID.Where(x => x.Estado == filters.Estado); }


                pagedCredentialDevice = PagedList<CredencialHid>.Create(credentialHID, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedCredentialDevice;
        }

        public async Task<CredencialHid> GetById(Guid credentialHIDId, Guid empresaId)
        {
            try
            {
                CredencialHid credentialHID = await _unitOfWork.CredencialHIDRepository.GetById(credentialHIDId);
                return credentialHID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Create(CredencialHid credentialHID, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                credentialHID.Id = Guid.NewGuid();
                credentialHID.UsuarioCreadorId = currentUserId;
                credentialHID.FechaCreacion = DateTime.Now;
                credentialHID.Estado = 1;

                await _unitOfWork.CredencialHIDRepository.Add(credentialHID);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;

            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> Update(CredencialHid credentialHID, Guid currentUserId)
        {
            try
            {
                if (credentialHID.Id == Guid.Empty) { return false; }
                if (currentUserId == Guid.Empty) { return false; }

                CredencialHid credentialHIDUpdate = await _unitOfWork.CredencialHIDRepository.GetById(credentialHID.Id);
                if (credentialHIDUpdate == null) { return false; }

                credentialHIDUpdate.TipoCredencial = credentialHID.TipoCredencial;
                credentialHIDUpdate.CredencialValor = credentialHID.CredencialValor;
                credentialHIDUpdate.Validity = credentialHID.Validity;

                credentialHIDUpdate.FechaModificacion = DateTime.Now;
                credentialHIDUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.CredencialHIDRepository.Update(credentialHIDUpdate);
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
                CredencialHid credentialHID = await _unitOfWork.CredencialHIDRepository.GetById(id);
                credentialHID.FechaBaja = DateTime.Now;
                credentialHID.UsuarioBajaId = currentUserId;
                credentialHID.Estado = 2;

                _unitOfWork.CredencialHIDRepository.Update(credentialHID);
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
                CredencialHid credentialHID = await _unitOfWork.CredencialHIDRepository.GetById(id);
                credentialHID.FechaReactivacion = DateTime.Now;
                credentialHID.UsuarioReactivadorId = currentUserId;
                credentialHID.Estado = 1;

                _unitOfWork.CredencialHIDRepository.Update(credentialHID);
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
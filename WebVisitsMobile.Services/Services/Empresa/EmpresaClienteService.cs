using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Models.Configuracion.Configuraciones;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.Interfaces.Empresa;
using WebVisitsMobile.Services.QueryFilters.Empresa;

namespace WebVisitsMobile.Services.Services.Empresa
{
    public class EmpresaClienteService : IEmpresaClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        private readonly IConfiguracionService _configuracionService;
        public EmpresaClienteService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options,
            IConfiguracionService configuracionService
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
            _configuracionService = configuracionService;
        }

        public async Task<PagedList<EmpresaCliente>> GetAll(EmpresaClienteQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

            IEnumerable<EmpresaCliente> data;

            if (filters.DatosCompletos == 0)
            {
                data = _unitOfWork.EmpresaClienteRepository.GetAll();
            }
            else
            {
                data = _unitOfWork.EmpresaClienteRepository.GetAll();
            }

            if (filters.Id != null && filters.Id != Guid.Empty) { data = data.Where(x => x.Id == filters.Id); }
            if (filters.RazonSocial != null) { data = data.Where(x => x.RazonSocial.ToLower().Contains(filters.RazonSocial.ToLower())); }
            if (filters.RFC != null) { data = data.Where(x => x.RFC.ToLower().Contains(filters.RFC.ToLower())); }
            if (filters.TelefonoEmpresa != null) { data = data.Where(x => x.TelefonoEmpresa.ToLower().Contains(filters.TelefonoEmpresa.ToLower())); }
            if (filters.TelefonoMovil != null) { data = data.Where(x => x.TelefonoMovil.ToLower().Contains(filters.TelefonoMovil.ToLower())); }
            if (filters.CorreoElectronico != null) { data = data.Where(x => x.CorreoElectronico.ToLower().Contains(filters.CorreoElectronico.ToLower())); }
            if (filters.UsaCredencialesHID != null && filters.UsaCredencialesHID > 0) { data = data.Where(x => x.UsaCredencialesHID == filters.UsaCredencialesHID); }

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

            var pagedClientCompany = PagedList<EmpresaCliente>.Create(data, filters.PageNumber, filters.PageSize);

            return pagedClientCompany;
        }

        public async Task<EmpresaCliente> GetById(Guid id)
        {
            try
            {
                EmpresaCliente data = await _unitOfWork.EmpresaClienteRepository.GetById(id);
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<EmpresaCliente> GetCompanyClient(Guid id)
        {
            try
            {
                EmpresaCliente data = await _unitOfWork.EmpresaClienteRepository.GetCompanyClient(g => g.Id == id);
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Inactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                EmpresaCliente clientCompany = await GetById(id);
                if (clientCompany == null) { return false; }

                clientCompany.FechaBaja = DateTime.Now;
                clientCompany.UsuarioBajaId = currentUserId;
                clientCompany.Estado = 2;

                _unitOfWork.EmpresaClienteRepository.Update(clientCompany);

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
                EmpresaCliente clientCompany = await GetById(id);
                if (clientCompany == null) { return false; }

                clientCompany.FechaReactivacion = DateTime.Now;
                clientCompany.UsuarioReactivadorId = currentUserId;
                clientCompany.Estado = 1;

                _unitOfWork.EmpresaClienteRepository.Update(clientCompany);

                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> CreateWithHID(EmpresaCliente clientCompany, List<ConfiguracionesReqDTO>? settings, Guid usuarioActualId)
        {
            bool booOk = false;

            try
            {
                if (usuarioActualId == Guid.Empty) { return false; }

                clientCompany.Id = Guid.NewGuid();
                clientCompany.UsuarioCreadorId = usuarioActualId;
                clientCompany.FechaCreacion = DateTime.Now;
                clientCompany.Estado = 1;

                await _unitOfWork.EmpresaClienteRepository.Add(clientCompany);
                await _unitOfWork.SaveChangesAsync();

                if (clientCompany.UsaCredencialesHID == 1)
                {
                    booOk = await _configuracionService.CreateSettingsForCompany(settings, clientCompany.Id, usuarioActualId);
                    var setting = await _configuracionService.GetByTypeSettingAndCompanyId(new Guid("742CE98B-684B-4A76-BA0D-CF62621FC3E7"), clientCompany.Id);
                    if (setting != null)
                    {
                        var taskById = await _unitOfWork.TipoTareaRepository.GetById(new Guid("90E155AB-EEE5-4B24-943C-9407A27F344D"));
                        if (taskById != null)
                        {
                            string valor = setting.Valor1!;
                            if (int.TryParse(valor, out int userId))
                            {
                                var licenseHIDByTask = new LicenseHIDByTask
                                {
                                    EmpresaClienteId = clientCompany.Id,
                                    UserId = userId
                                };

                                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                                {
                                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                    WriteIndented = false
                                };

                                Tarea task = new Tarea();
                                task.TipoTareaId = new Guid("90E155AB-EEE5-4B24-943C-9407A27F344D");
                                task.Fecha = DateTime.Now;
                                task.Pendiente = 1;
                                task.Status = 1;
                                task.ValorEnvio = System.Text.Json.JsonSerializer.Serialize(licenseHIDByTask, jsonOptions);
                                task.ValorRetorno = "";
                                task.EmpresaClienteId = clientCompany.Id;
                                task.Id = Guid.NewGuid();
                                task.UsuarioCreadorId = usuarioActualId;
                                task.FechaCreacion = DateTime.Now;
                                task.Estado = 1;

                                await _unitOfWork.TareaRepository.Add(task);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }
                }
                else
                {
                    booOk = true;
                }
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> Update(EmpresaCliente clientCompany, Guid usuarioActualId)
        {
            try
            {
                EmpresaCliente clientCompanyUpdate = await _unitOfWork.EmpresaClienteRepository.GetById(clientCompany.Id);
                clientCompanyUpdate.RazonSocial = clientCompany.RazonSocial;
                clientCompanyUpdate.RFC = clientCompany.RFC;
                clientCompanyUpdate.TelefonoEmpresa = clientCompany.TelefonoEmpresa;
                clientCompanyUpdate.TelefonoMovil = clientCompany.TelefonoMovil;
                clientCompanyUpdate.CorreoElectronico = clientCompany.CorreoElectronico;
                clientCompanyUpdate.UsaCredencialesHID = clientCompany.UsaCredencialesHID;

                clientCompanyUpdate.FechaModificacion = DateTime.Now;
                clientCompanyUpdate.UsuarioModificadorId = usuarioActualId;

                _unitOfWork.EmpresaClienteRepository.Update(clientCompanyUpdate);
                await _unitOfWork.SaveChangesAsync();

                if (clientCompany.UsaCredencialesHID == 2)
                {
                    return await _configuracionService.DeactivateAllSettingByCompany(clientCompany.Id, usuarioActualId);
                }
                else
                {
                    return await _configuracionService.CreateInitialSettingsForCompany(clientCompany.Id, usuarioActualId);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<EmpresaCliente?> GetByRFC(string rfc)
        {
            EmpresaCliente clientCompany = await _unitOfWork.EmpresaClienteRepository.GetCompanyClient(l => l.RFC == rfc);
            return clientCompany;
        }

        public async Task<EmpresaCliente?> GetByRazonSocial(string socialReason)
        {
            EmpresaCliente clientCompany = await _unitOfWork.EmpresaClienteRepository.GetCompanyClient(l => l.RazonSocial == socialReason);
            return clientCompany;
        }
    }
}
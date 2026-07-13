using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Services.HID
{
    public class LicenciaHIDService : ILicenciaHIDService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguracionService _configuracionService;
        private readonly PaginationOption _paginationOptions;

        public LicenciaHIDService(
            IUnitOfWork unitOfWork,
            IConfiguracionService configuracionService,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _configuracionService = configuracionService;
            _paginationOptions = options.Value;
        }

        public async Task<PagedList<LicenciaHID>> GetAll(LicenciaHIDQueryFilter filters, Guid empresaId)
        {
            PagedList<LicenciaHID> pagedCredentialDevice = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;


                var licenseHID = _unitOfWork.LicenciaHIDRepository.GetAll();

                if (!string.IsNullOrWhiteSpace(filters.NumeroParte))
                {
                    licenseHID = licenseHID.Where(x => x.NumeroParte.Contains(filters.NumeroParte));
                }

                if (!string.IsNullOrWhiteSpace(filters.Nombre))
                {
                    licenseHID = licenseHID.Where(x => x.Nombre.Contains(filters.Nombre));
                }

                if (filters.CantidadDisponible != null)
                {
                    licenseHID = licenseHID.Where(x => x.CantidadDisponible == filters.CantidadDisponible);
                }

                if (filters.CantidadConsumida != null)
                {
                    licenseHID = licenseHID.Where(x => x.CantidadConsumida == filters.CantidadConsumida);
                }

                if (!string.IsNullOrWhiteSpace(filters.EstadoLicencia))
                {
                    licenseHID = licenseHID.Where(x => x.EstadoLicencia.Contains(filters.EstadoLicencia));
                }

                if (!string.IsNullOrWhiteSpace(filters.EstadoPeriodo))
                {
                    licenseHID = licenseHID.Where(x => x.EstadoPeriodo.Contains(filters.EstadoPeriodo));
                }

                if (!string.IsNullOrWhiteSpace(filters.MensajeEstado))
                {
                    licenseHID = licenseHID.Where(x => x.MensajeEstado.Contains(filters.MensajeEstado));
                }

                if (filters.EmpresaClienteId != null && filters.EmpresaClienteId != Guid.Empty) { licenseHID = licenseHID.Where(x => x.EmpresaClienteId == filters.EmpresaClienteId); }

                if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { licenseHID = licenseHID.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
                if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { licenseHID = licenseHID.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
                if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { licenseHID = licenseHID.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
                if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { licenseHID = licenseHID.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }

                //if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { credentialDevice = credentialDevice.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
                //if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { credentialDevice = credentialDevice.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }

                if (filters.FechaInicio != null && filters.FechaFin == null)
                {
                    // Solo se ingresa FechaCreacionDesde → traer registros de ese día
                    var fecha = filters.FechaInicio.Value.Date;
                    var fechaFin = fecha.AddDays(1).AddTicks(-1);

                    licenseHID = licenseHID
                        .Where(x => x.FechaInicio >= fecha && x.FechaInicio <= fechaFin);
                }
                else if (filters.FechaInicio == null && filters.FechaFin != null)
                {
                    var fecha = filters.FechaFin.Value.Date;

                    licenseHID = licenseHID
                        .Where(x => x.FechaFin.Date == fecha);
                }
                else if (filters.FechaInicio != null && filters.FechaFin != null)
                {
                    var inicio = filters.FechaInicio.Value.Date;
                    var fin = filters.FechaFin.Value.Date;

                    licenseHID = licenseHID
                        .Where(x => x.FechaInicio.Date >= inicio && x.FechaInicio.Date <= fin);
                }

                if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { licenseHID = licenseHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
                if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { licenseHID = licenseHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
                if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { licenseHID = licenseHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
                if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { licenseHID = licenseHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
                if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { licenseHID = licenseHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
                if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { licenseHID = licenseHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
                if (filters.Estado != null && filters.Estado > 0) { licenseHID = licenseHID.Where(x => x.Estado == filters.Estado); }


                pagedCredentialDevice = PagedList<LicenciaHID>.Create(licenseHID, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedCredentialDevice;
        }

        public async Task<LicenciaHID> GetById(Guid licenseHIDId, Guid empresaId)
        {
            try
            {
                LicenciaHID licenciaHID = await _unitOfWork.LicenciaHIDRepository.GetById(licenseHIDId);
                return licenciaHID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<LicenciaHID?> GetByEmpresaClienteId(Guid empresaClienteId)
        {
            try
            {
                var licenciaHID = _unitOfWork.LicenciaHIDRepository.GetAll()
                    .FirstOrDefault(x => x.EmpresaClienteId == empresaClienteId);

                return licenciaHID;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Create(LicenciaHID licenciaHID, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                licenciaHID.Id = Guid.NewGuid();

                licenciaHID.CantidadTotal = licenciaHID.CantidadTotal;
                licenciaHID.CantidadDisponible = licenciaHID.CantidadDisponible;
                licenciaHID.CantidadConsumida = licenciaHID.CantidadConsumida;

                licenciaHID.UsuarioCreadorId = currentUserId;
                licenciaHID.FechaCreacion = DateTime.Now;
                licenciaHID.Estado = 1;

                await _unitOfWork.LicenciaHIDRepository.Add(licenciaHID);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;

            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> CreateByTask(LicenciaHID licenseHID, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                if (licenseHID.EmpresaClienteId == null)
                {
                    return false;
                }
                var setting = await _configuracionService.GetByTypeSettingAndCompanyId(new Guid("742CE98B-684B-4A76-BA0D-CF62621FC3E7"), (Guid)licenseHID.EmpresaClienteId);
                if (setting == null)
                {
                    return false;
                }
                string valor = setting.Valor1;
                if (string.IsNullOrWhiteSpace(valor) || !int.TryParse(valor, out int userId))
                {
                    return false;
                }
                var licenseHIDByTask = new LicenseHIDByTask
                {
                    EmpresaClienteId = licenseHID.EmpresaClienteId,
                    UserId = userId
                };

                var taskById = await _unitOfWork.TipoTareaRepository.GetById(new Guid("90E155AB-EEE5-4B24-943C-9407A27F344D"));
                if (taskById != null)
                {
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
                    // task.ReferenciaId = licenseUserHID.Id;
                    task.Id = Guid.NewGuid();
                    task.UsuarioCreadorId = currentUserId;
                    task.FechaCreacion = DateTime.Now;
                    task.Estado = 1;

                    await _unitOfWork.TareaRepository.Add(task);
                    await _unitOfWork.SaveChangesAsync();

                    return true;
                }
                booOk = false;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> CreateByTask(Guid clientCompanyId, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                if (clientCompanyId == Guid.Empty)
                {
                    return false;
                }
                var setting = await _configuracionService.GetByTypeSettingAndCompanyId(new Guid("742CE98B-684B-4A76-BA0D-CF62621FC3E7"), clientCompanyId);
                if (setting == null)
                {
                    return false;
                }
                string valor = setting.Valor1;
                if (string.IsNullOrWhiteSpace(valor) || !int.TryParse(valor, out int userId))
                {
                    return false;
                }
                var licenseHIDByTask = new LicenseHIDByTask
                {
                    EmpresaClienteId = clientCompanyId,
                    UserId = userId
                };

                var taskById = await _unitOfWork.TipoTareaRepository.GetById(new Guid("90E155AB-EEE5-4B24-943C-9407A27F344D"));
                if (taskById != null)
                {
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
                    // task.ReferenciaId = licenseUserHID.Id;
                    task.EmpresaClienteId = clientCompanyId;
                    task.Id = Guid.NewGuid();
                    task.UsuarioCreadorId = currentUserId;
                    task.FechaCreacion = DateTime.Now;
                    task.Estado = 1;

                    await _unitOfWork.TareaRepository.Add(task);
                    await _unitOfWork.SaveChangesAsync();

                    return true;
                }
                booOk = false;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> SynchronizeByTask(LicenciaHID licenseHID, Guid currentUserId)
        {
            try
            {
                if (licenseHID.EmpresaClienteId == null)
                {
                    return false;
                }
                var setting = await _configuracionService.GetByTypeSettingAndCompanyId(new Guid("742CE98B-684B-4A76-BA0D-CF62621FC3E7"), (Guid)licenseHID.EmpresaClienteId);
                if (setting == null)
                {
                    return false;
                }
                string valor = setting.Valor1;
                if (string.IsNullOrWhiteSpace(valor) || !int.TryParse(valor, out int userId))
                {
                    return false;
                }
                if (currentUserId == Guid.Empty) { return false; }
                LicenciaHID licenciaHID = await _unitOfWork.LicenciaHIDRepository.GetById(licenseHID.Id);
                if (licenciaHID == null) { return false; }

                var licenseHIDByTask = new LicenseHIDUpdateByTask
                {
                    Id = licenciaHID.Id,
                    NumeroParte = licenciaHID.NumeroParte,
                    Nombre = licenciaHID.Nombre,
                    EmpresaClienteId = licenseHID.EmpresaClienteId,
                    CantidadTotal = licenciaHID.CantidadTotal,
                    CantidadDisponible = licenciaHID.CantidadDisponible,
                    CantidadConsumida = licenciaHID.CantidadConsumida,
                    FechaInicio = licenciaHID.FechaInicio,
                    FechaFin = licenciaHID.FechaFin,
                    EstadoLicencia = licenciaHID.EstadoLicencia,
                    EstadoPeriodo = licenciaHID.EstadoPeriodo,
                    MensajeEstado = licenciaHID.MensajeEstado,
                    UserId = userId
                };

                var taskById = await _unitOfWork.TipoTareaRepository.GetById(new Guid("0250978C-6734-4496-90CB-4F8EC82750AC"));
                if (taskById != null)
                {
                    var jsonOptions = new System.Text.Json.JsonSerializerOptions
                    {
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false
                    };

                    Tarea task = new Tarea();
                    task.TipoTareaId = new Guid("0250978C-6734-4496-90CB-4F8EC82750AC");
                    task.Fecha = DateTime.Now;
                    task.Pendiente = 1;
                    task.Status = 1;
                    task.ValorEnvio = System.Text.Json.JsonSerializer.Serialize(licenseHIDByTask, jsonOptions);
                    task.ValorRetorno = "";
                    // task.ReferenciaId = licenseUserHID.Id;
                    task.Id = Guid.NewGuid();
                    task.UsuarioCreadorId = currentUserId;
                    task.FechaCreacion = DateTime.Now;
                    task.Estado = 1;

                    await _unitOfWork.TareaRepository.Add(task);
                    await _unitOfWork.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Update(LicenciaHID licenseHID, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                LicenciaHID licenciaHID = await _unitOfWork.LicenciaHIDRepository.GetById(licenseHID.Id);
                if (licenciaHID == null) { return false; }

                licenciaHID.NumeroParte = licenseHID.NumeroParte;
                licenciaHID.Nombre = licenseHID.Nombre;
                licenciaHID.EmpresaClienteId = licenseHID.EmpresaClienteId;
                //licenciaHID.CantidadTotal = licenseHID.CantidadTotal;
                //licenciaHID.CantidadDisponible = licenseHID.CantidadDisponible;
                //licenciaHID.CantidadConsumida = licenseHID.CantidadConsumida;
                licenciaHID.FechaInicio = licenseHID.FechaInicio;
                licenciaHID.FechaFin = licenseHID.FechaFin;
                //licenciaHID.EstadoLicencia = licenseHID.EstadoLicencia;
                //licenciaHID.EstadoPeriodo = licenseHID.EstadoPeriodo;
                licenciaHID.MensajeEstado = licenseHID.MensajeEstado;
                licenciaHID.FechaModificacion = DateTime.Now;
                licenciaHID.UsuarioModificadorId = currentUserId;

                _unitOfWork.LicenciaHIDRepository.Update(licenciaHID);
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
                LicenciaHID licenciaHID = await _unitOfWork.LicenciaHIDRepository.GetById(id);
                licenciaHID.FechaBaja = DateTime.Now;
                licenciaHID.UsuarioBajaId = currentUserId;
                licenciaHID.Estado = 2;

                _unitOfWork.LicenciaHIDRepository.Update(licenciaHID);
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
                LicenciaHID licenciaHID = await _unitOfWork.LicenciaHIDRepository.GetById(id);
                licenciaHID.FechaReactivacion = DateTime.Now;
                licenciaHID.UsuarioReactivadorId = currentUserId;
                licenciaHID.Estado = 1;

                _unitOfWork.LicenciaHIDRepository.Update(licenciaHID);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public IEnumerable<LicenciaHID> GetAllList(LicenciaHIDQueryFilter filters)
        {
            IEnumerable<LicenciaHID> credentialDevice = new List<LicenciaHID>();

            try
            {
                credentialDevice = _unitOfWork.LicenciaHIDRepository.GetAll();

                if (!string.IsNullOrWhiteSpace(filters.NumeroParte))
                {
                    credentialDevice = credentialDevice.Where(x => x.NumeroParte.Contains(filters.NumeroParte));
                }

                if (!string.IsNullOrWhiteSpace(filters.Nombre))
                {
                    credentialDevice = credentialDevice.Where(x => x.Nombre.Contains(filters.Nombre));
                }

                if (filters.CantidadDisponible != null)
                {
                    credentialDevice = credentialDevice.Where(x => x.CantidadDisponible == filters.CantidadDisponible);
                }

                if (filters.CantidadConsumida != null)
                {
                    credentialDevice = credentialDevice.Where(x => x.CantidadConsumida == filters.CantidadConsumida);
                }

                if (!string.IsNullOrWhiteSpace(filters.EstadoLicencia))
                {
                    credentialDevice = credentialDevice.Where(x => x.EstadoLicencia.Contains(filters.EstadoLicencia));
                }

                if (!string.IsNullOrWhiteSpace(filters.EstadoPeriodo))
                {
                    credentialDevice = credentialDevice.Where(x => x.EstadoPeriodo.Contains(filters.EstadoPeriodo));
                }

                if (!string.IsNullOrWhiteSpace(filters.MensajeEstado))
                {
                    credentialDevice = credentialDevice.Where(x => x.MensajeEstado.Contains(filters.MensajeEstado));
                }


                if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { credentialDevice = credentialDevice.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
                if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { credentialDevice = credentialDevice.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
                if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { credentialDevice = credentialDevice.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
                if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { credentialDevice = credentialDevice.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
                if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { credentialDevice = credentialDevice.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
                if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { credentialDevice = credentialDevice.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
                if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { credentialDevice = credentialDevice.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
                if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { credentialDevice = credentialDevice.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
                if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { credentialDevice = credentialDevice.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
                if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { credentialDevice = credentialDevice.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
                if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { credentialDevice = credentialDevice.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
                if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { credentialDevice = credentialDevice.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
                if (filters.Estado != null && filters.Estado > 0) { credentialDevice = credentialDevice.Where(x => x.Estado == filters.Estado); }

            }
            catch (Exception ex)
            {

                return null;
            }

            return credentialDevice;
        }
    }
}
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Models.HID.UserHID;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.Interfaces.Organizacion.Email;
using WebVisitsMobile.Services.Interfaces.Organizacion.Tarea;
using WebVisitsMobile.Services.QueryFilters.HID;
using WebVisitsMobile.Services.QueryFilters.Organizacion.Tarea;
using static WebVisitsMobile.Services.Services.HID.LicenciaUserHIDService;

namespace WebVisitsMobile.Services.Services.HID
{
    public class LicenciaUserHIDService : ILicenciaUserHIDService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        private readonly IConfiguracionService _configuracionService;
        private readonly IDipositivosHIDService _dipositivosHIDService;
        private readonly ICredencialHIDService _credencialHIDService;
        private readonly ITareaService _tareaService;
        private readonly IEmailService _emailService;

        public enum InvitationStatus
        {
            Pending = 1,
            Sent = 2,
            Accepted = 3,
            Expired = 4,
            Acknowledged = 5,
            Cancelled = 6,
            Inactive = 7
        }

        public LicenciaUserHIDService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options,
            IConfiguracionService configuracionService,
            IDipositivosHIDService dipositivosHIDService,
            ICredencialHIDService credencialHIDService,
            ITareaService tareaService,
            IEmailService emailService
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
            _dipositivosHIDService = dipositivosHIDService;
            _credencialHIDService = credencialHIDService;
            _tareaService = tareaService;
            _configuracionService = configuracionService;
            _emailService = emailService;
        }

        public async Task<LicenciaHidUser?> ExistUserWVM(string email, Guid clientCompanyId)
        {
            try
            {
                LicenciaUserHIDQueryFilter filters = new LicenciaUserHIDQueryFilter();
                filters.Email = email;
                filters.Estado = 1;

                var usersWithEmail = await GetAll(filters);

                return usersWithEmail.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedList<LicenciaHidUser>> GetAll(LicenciaUserHIDQueryFilter filters)
        {
            PagedList<LicenciaHidUser> pagedCredentialDevice = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<LicenciaHidUser> userHID;

                if (filters.DatosCompletos == 0)
                {
                    userHID = _unitOfWork.LicenciaUserHIDRepository.GetAll()
                            .OrderBy(u => u.FechaCreacion)
                            .ThenBy(u => u.Estado);
                }
                else
                {
                    userHID = await _unitOfWork.LicenciaUserHIDRepository.GetAllUserHID();
                }

                //if (filters.Nombre != null) { userHID = userHID.Where(x => x.Nombre.ToLower().Contains(filters.Nombre.ToLower())); }
                if (!string.IsNullOrWhiteSpace(filters.Nombre))
                {
                    var palabras = filters.Nombre
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var palabra in palabras)
                    {
                        userHID = userHID.Where(x =>
                            (!string.IsNullOrEmpty(x.Nombre) &&
                             x.Nombre.Contains(palabra, StringComparison.OrdinalIgnoreCase))
                            ||
                            (!string.IsNullOrEmpty(x.Apellidos) &&
                             x.Apellidos.Contains(palabra, StringComparison.OrdinalIgnoreCase))
                        );
                    }
                }
                if (filters.Email != null) { userHID = userHID.Where(x => x.Email.ToLower().Contains(filters.Email.ToLower())); }
                if (!string.IsNullOrWhiteSpace(filters.InvitacionActividad))
                {
                    userHID = userHID
                        .Where(x => !string.IsNullOrWhiteSpace(x.InvitacionActividad) &&
                                    x.InvitacionActividad.Contains(filters.InvitacionActividad, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(filters.Telefono))
                {
                    userHID = userHID.Where(x =>
                        !string.IsNullOrEmpty(x.Telefono) &&
                        x.Telefono.ToLower().Contains(filters.Telefono.ToLower())
                    );
                }
                if (filters.Status != null && filters.Status != 0) { userHID = userHID.Where(x => x.Status == filters.Status); }
                if (filters.EmpresaClienteId != null && filters.EmpresaClienteId != Guid.Empty) { userHID = userHID.Where(x => x.EmpresaClienteId == filters.EmpresaClienteId); }

                if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { userHID = userHID.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
                if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { userHID = userHID.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
                if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { userHID = userHID.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
                if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { userHID = userHID.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
                if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { userHID = userHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
                if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { userHID = userHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
                if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { userHID = userHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
                if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { userHID = userHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
                if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { userHID = userHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
                if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { userHID = userHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
                if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { userHID = userHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
                if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { userHID = userHID.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
                if (filters.Estado != null && filters.Estado > 0) { userHID = userHID.Where(x => x.Estado == filters.Estado); }

                pagedCredentialDevice = PagedList<LicenciaHidUser>.Create(userHID, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedCredentialDevice;
        }

        public async Task<bool> Create(LicenciaHidUser licenseUserHID, Guid currentClientCompanyId, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                licenseUserHID.LicenseCount = 1;
                licenseUserHID.EmpresaClienteId = currentClientCompanyId;
                licenseUserHID.Id = Guid.NewGuid();
                licenseUserHID.Status = 1;
                licenseUserHID.UsuarioCreadorId = currentUserId;
                licenseUserHID.FechaCreacion = DateTime.Now;
                licenseUserHID.Estado = 1;

                if (licenseUserHID.ExternalId == null)
                {
                    licenseUserHID.ExternalId = licenseUserHID.Id;
                }

                await _unitOfWork.LicenciaUserHIDRepository.Add(licenseUserHID);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;

                var taskById = await _unitOfWork.TipoTareaRepository.GetById(new Guid("3D68F904-2A4A-40BD-BB62-09A95B7247F5"));
                if (taskById != null)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles
                    };

                    Tarea task = new Tarea();
                    task.TipoTareaId = new Guid("3D68F904-2A4A-40BD-BB62-09A95B7247F5");
                    task.Fecha = DateTime.Now;
                    task.Pendiente = 1;
                    task.Status = 1;
                    task.ValorEnvio = System.Text.Json.JsonSerializer.Serialize(licenseUserHID, jsonOptions);
                    task.ValorRetorno = "";
                    task.ReferenciaId = licenseUserHID.Id;
                    task.EmpresaClienteId = currentClientCompanyId;
                    task.Id = Guid.NewGuid();
                    task.UsuarioCreadorId = currentUserId;
                    task.FechaCreacion = DateTime.Now;
                    task.Estado = 1;

                    await _unitOfWork.TareaRepository.Add(task);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }
        public async Task<bool> Update(LicenciaHidUser licenseUserHID, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                LicenciaHidUser licenseUserHIDUpdate = await _unitOfWork.LicenciaUserHIDRepository.GetById(licenseUserHID.Id);
                if (licenseUserHIDUpdate == null) { return false; }

                if (licenseUserHID.LicenciaId != Guid.Empty)
                    licenseUserHIDUpdate.LicenciaId = licenseUserHID.LicenciaId;

                if (!string.IsNullOrWhiteSpace(licenseUserHID.Nombre))
                    licenseUserHIDUpdate.Nombre = licenseUserHID.Nombre;

                if (!string.IsNullOrWhiteSpace(licenseUserHID.Email))
                    licenseUserHIDUpdate.Email = licenseUserHID.Email;

                if (licenseUserHID.UserId.HasValue && licenseUserHID.UserId.Value > 0)
                    licenseUserHIDUpdate.UserId = licenseUserHID.UserId;

                licenseUserHIDUpdate.Site = licenseUserHID.Site;
                licenseUserHIDUpdate.Alert = licenseUserHID.Alert;
                licenseUserHIDUpdate.LicenseCount = 1;

                if (!string.IsNullOrWhiteSpace(licenseUserHID.Telefono))
                    licenseUserHIDUpdate.Telefono = licenseUserHID.Telefono;

                if (licenseUserHID.InvitacionFecha.HasValue)
                    licenseUserHIDUpdate.InvitacionFecha = licenseUserHID.InvitacionFecha;

                if (licenseUserHID.InvitacionExpirationDate.HasValue)
                    licenseUserHIDUpdate.InvitacionExpirationDate = licenseUserHID.InvitacionExpirationDate;

                if (!string.IsNullOrWhiteSpace(licenseUserHID.InvitacionActividad))
                    licenseUserHIDUpdate.InvitacionActividad = licenseUserHID.InvitacionActividad;

                if (!string.IsNullOrWhiteSpace(licenseUserHID.InvitacionDetalle))
                    licenseUserHIDUpdate.InvitacionDetalle = licenseUserHID.InvitacionDetalle;

                if (licenseUserHID.InvitacionId.HasValue && licenseUserHID.InvitacionId.Value > 0)
                    licenseUserHIDUpdate.InvitacionId = licenseUserHID.InvitacionId;

                if (licenseUserHID.Status.HasValue && licenseUserHID.Status.Value > 0)
                    licenseUserHIDUpdate.Status = licenseUserHID.Status;

                if (!string.IsNullOrWhiteSpace(licenseUserHID.Apellidos))
                    licenseUserHIDUpdate.Apellidos = licenseUserHID.Apellidos;

                licenseUserHIDUpdate.FechaModificacion = DateTime.Now;
                licenseUserHIDUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.LicenciaUserHIDRepository.Update(licenseUserHIDUpdate);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateWithAttributes(LicenciaHidUser licenseUserHID, Guid clientCompanyId, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                if (licenseUserHID.UserId == 0 || licenseUserHID.UserId == null)
                {
                    return await Update(licenseUserHID, currentUserId);
                }
                if (licenseUserHID.Nombre == null || licenseUserHID.Email == null)
                {
                    return false;
                }
                LicenciaHidUser licenseUserHIDUpdate = await _unitOfWork.LicenciaUserHIDRepository.GetById(licenseUserHID.Id);
                if (licenseUserHIDUpdate == null) { return false; }

                //var appSettingsResult = await _configuracionService.GetAppSettings(clientCompanyId);
                //if (!appSettingsResult.Success || appSettingsResult.Value == null)
                //{
                //    return false;
                //}
                //var settingHID = appSettingsResult.Value;
                //var tokenResponse = await _hIDService.GetTokenHIDAsync(settingHID);
                //if (tokenResponse == null) return false;
                //if (tokenResponse.access_token == null) return false;
                //var attributesDTO = new Models.Request.Organizacion.UserHIDEditarAtributosDTO
                //{
                //    UserId = (int)licenseUserHID.UserId,
                //    UserExternalId = licenseUserHIDUpdate.Id.ToString(),
                //    UserFamilyName = licenseUserHID.Apellidos,
                //    UserGivenName = licenseUserHID.Nombre,
                //    UserEmail = licenseUserHID.Email
                //};
                //var userResponse = await _hIDService.UpdateUserAsync(settingHID, attributesDTO, tokenResponse.access_token);
                //if (userResponse == null) return false;

                licenseUserHIDUpdate.Nombre = licenseUserHID.Nombre;
                licenseUserHIDUpdate.Apellidos = licenseUserHID.Apellidos;
                licenseUserHIDUpdate.Email = licenseUserHID.Email;
                licenseUserHIDUpdate.Site = licenseUserHID.Site;
                licenseUserHIDUpdate.Alert = licenseUserHID.Alert;
                licenseUserHIDUpdate.Telefono = licenseUserHID.Telefono;

                licenseUserHIDUpdate.FechaInicio = licenseUserHID.FechaInicio;
                licenseUserHIDUpdate.FechaFin = licenseUserHID.FechaFin;

                licenseUserHIDUpdate.FechaModificacion = DateTime.Now;
                licenseUserHIDUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.LicenciaUserHIDRepository.Update(licenseUserHIDUpdate);
                await _unitOfWork.SaveChangesAsync();

                // ✅ Crear la tarea solo si todo fue exitoso
                var taskById = await _unitOfWork.TipoTareaRepository.GetById(new Guid("CBF07C75-2E43-40AB-B22A-3EEB44B51559"));
                if (taskById != null)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles
                    };

                    Tarea task = new Tarea
                    {
                        TipoTareaId = taskById.Id,
                        Fecha = DateTime.Now,
                        Pendiente = 1,
                        Status = 1,
                        ValorEnvio = JsonSerializer.Serialize(licenseUserHIDUpdate, jsonOptions),
                        ValorRetorno = "",
                        ReferenciaId = licenseUserHID.Id,
                        EmpresaClienteId = clientCompanyId,
                        Id = Guid.NewGuid(),
                        UsuarioCreadorId = currentUserId,
                        FechaCreacion = DateTime.Now,
                        Estado = 1
                    };

                    await _unitOfWork.TareaRepository.Add(task);
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatus(Guid id, int statusUser, string statusInvitation, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                LicenciaHidUser licenseUserHIDUpdate = await _unitOfWork.LicenciaUserHIDRepository.GetById(id);
                if (licenseUserHIDUpdate == null) { return false; }

                licenseUserHIDUpdate.InvitacionActividad = statusInvitation;
                licenseUserHIDUpdate.Status = statusUser;

                licenseUserHIDUpdate.FechaModificacion = DateTime.Now;
                licenseUserHIDUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.LicenciaUserHIDRepository.Update(licenseUserHIDUpdate);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatus(Guid userHIDId, string newInvitationActivity, int newStatus, Guid currentUserId)
        {
            try
            {
                return await _unitOfWork.LicenciaUserHIDRepository.UpdateStatusOnly(userHIDId, newInvitationActivity, newStatus, currentUserId);
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
                if (currentUserId == Guid.Empty) { return false; }
                LicenciaHidUser licenseUserHID = await _unitOfWork.LicenciaUserHIDRepository.GetById(id);
                if (licenseUserHID == null) { return false; }
                licenseUserHID.FechaBaja = DateTime.Now;
                licenseUserHID.UsuarioBajaId = currentUserId;
                licenseUserHID.Estado = 2;

                _unitOfWork.LicenciaUserHIDRepository.Update(licenseUserHID);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> InactivateById(Guid id, Guid userLowId)
        {
            bool booOk = false;
            try
            {
                if (userLowId == Guid.Empty) { return false; }
                LicenciaHidUser licenseUserHID = await _unitOfWork.LicenciaUserHIDRepository.GetById(id);
                if (licenseUserHID == null) { return false; }
                licenseUserHID.InvitacionActividad = "DELETED";
                licenseUserHID.Status = 7;
                licenseUserHID.FechaBaja = DateTime.Now;
                licenseUserHID.UsuarioBajaId = userLowId;
                licenseUserHID.Estado = 2;

                _unitOfWork.LicenciaUserHIDRepository.Update(licenseUserHID);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> InactivateWithHIDAndTask(Guid id, Guid currentUserId, Guid clientCompanyId)
        {
            if (id == Guid.Empty || currentUserId == Guid.Empty)
                return false;

            TareaQueryFilter tareaQueryFilter = new TareaQueryFilter();
            // tareaQueryFilter.Pendiente = 1;
            tareaQueryFilter.ReferenciaId = id;
            tareaQueryFilter.Estado = 1;
            tareaQueryFilter.PageNumber = 1;
            tareaQueryFilter.PageSize = 1000;

            var taskList = await _tareaService.GetAll(tareaQueryFilter);
            if (taskList == null || !taskList.Any())
            {
                var userExist = await GetById(id);
                if (userExist == null)
                {
                    return true;
                }
                else
                {
                    var userResult = await UpdateStatus(id, 7, "DELETED", currentUserId);
                    if (!userResult)
                        return false;
                    return await Inactivate(id, currentUserId);
                }
            }

            foreach (var tarea in taskList)
            {
                if (tarea == null || tarea.Id == Guid.Empty || !tarea.ReferenciaId.HasValue || tarea.ReferenciaId == Guid.Empty)
                    continue;

                var licenseResult = await Inactivate((Guid)tarea.ReferenciaId, currentUserId);
                if (!licenseResult)
                    return false;

                var taskResult = await _tareaService.Inactivate(tarea.Id, currentUserId);
                if (!taskResult)
                    return false;

                var userResult = await UpdateStatus((Guid)tarea.ReferenciaId, 7, "DELETED", currentUserId);
                if (!userResult)
                    return false;
            }

            return await Inactivate(id, currentUserId);
        }

        public async Task<bool> InactivateWithHID(UserHIDEliminarDTO licenciaUserHIDEliminar, Guid clientCompanyId, Guid currentUserId)
        {
            bool booOk = false;
            try
            {
                //if (currentUserId == Guid.Empty) { return false; }
                //if (licenciaUserHIDEliminar == null || licenciaUserHIDEliminar.Id == Guid.Empty || currentUserId == Guid.Empty)
                //{
                //    return false;
                //}
                //var appSettingsResult = await _configuracionService.GetAppSettings(clientCompanyId);
                //if (!appSettingsResult.Success || appSettingsResult.Value == null)
                //{
                //    return false;
                //}
                //var appSettings = appSettingsResult.Value;
                LicenciaHidUser licenseUserHID = await _unitOfWork.LicenciaUserHIDRepository.GetById(licenciaUserHIDEliminar.Id);
                if (licenseUserHID == null) { return false; }
                if (licenseUserHID.UserId == null || licenseUserHID.UserId <= 0) { return false; }
                //var tokenResponse = await _hIDService.GetTokenHIDAsync(appSettings);
                //if (tokenResponse == null) return false;
                //if (tokenResponse.access_token == null) return false;
                //var requestUserHID = new Models.Request.Organizacion.LicenciaUserHIDEliminarDTO()
                //{
                //    Id = licenseUserHID.Id,
                //    UserId = (int)licenseUserHID.UserId
                //};
                //var deleteUserResponse = await _hIDService.DeleteUserAsync(appSettings, requestUserHID, tokenResponse.access_token);
                //if (deleteUserResponse == false) return false;


                DipositivosHIDQueryFilter dipositivosHIDQueryFilter = new DipositivosHIDQueryFilter();
                dipositivosHIDQueryFilter.PageNumber = 1;
                dipositivosHIDQueryFilter.PageSize = 100000;
                dipositivosHIDQueryFilter.UsuarioId = licenseUserHID.Id;
                dipositivosHIDQueryFilter.Estado = 1;
                var deviceResponse = await _dipositivosHIDService.GetAll(dipositivosHIDQueryFilter, clientCompanyId);
                if (deviceResponse.Count() > 0)
                {
                    foreach (var item in deviceResponse.ToList())
                    {
                        await _dipositivosHIDService.Inactivate(item.Id, currentUserId);
                    }
                }
                CredencialHIDQueryFilter credencialHIDQueryFilter = new CredencialHIDQueryFilter();
                credencialHIDQueryFilter.PageNumber = 1;
                credencialHIDQueryFilter.PageSize = 100000;
                credencialHIDQueryFilter.Usuarioid = licenseUserHID.Id;
                credencialHIDQueryFilter.Estado = 1;
                var credentialResponse = await _credencialHIDService.GetAll(credencialHIDQueryFilter, clientCompanyId);
                if (credentialResponse.Count() > 0)
                {
                    foreach (var item in credentialResponse.ToList())
                    {
                        await _credencialHIDService.Inactivate(item.Id, currentUserId);
                    }
                }
                var statusResponse = await UpdateStatus(licenseUserHID.Id, 7, "DELETED", currentUserId);
                if (statusResponse == false) { return false; }
                licenseUserHID.FechaBaja = DateTime.Now;
                licenseUserHID.UsuarioBajaId = currentUserId;
                licenseUserHID.Estado = 2;
                _unitOfWork.LicenciaUserHIDRepository.Update(licenseUserHID);
                await _unitOfWork.SaveChangesAsync();

                // ✅ Crear la tarea solo si todo fue exitoso
                var taskById = await _unitOfWork.TipoTareaRepository.GetById(new Guid("74E34403-E42C-4AE4-93C1-309ABBE95A24"));
                if (taskById != null)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles
                    };

                    Tarea task = new Tarea
                    {
                        TipoTareaId = taskById.Id,
                        Fecha = DateTime.Now,
                        Pendiente = 1,
                        Status = 1,
                        ValorEnvio = JsonSerializer.Serialize(licenseUserHID, jsonOptions),
                        ValorRetorno = "",
                        ReferenciaId = licenseUserHID.Id,
                        Id = Guid.NewGuid(),
                        UsuarioCreadorId = currentUserId,
                        FechaCreacion = DateTime.Now,
                        Estado = 1
                    };

                    await _unitOfWork.TareaRepository.Add(task);
                    await _unitOfWork.SaveChangesAsync();
                }

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
                LicenciaHidUser licenseUserHID = await _unitOfWork.LicenciaUserHIDRepository.GetById(id);
                licenseUserHID.FechaReactivacion = DateTime.Now;
                licenseUserHID.UsuarioReactivadorId = currentUserId;
                licenseUserHID.Estado = 1;

                _unitOfWork.LicenciaUserHIDRepository.Update(licenseUserHID);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<LicenciaHidUser> GetById(Guid licenseUserHIDId)
        {
            try
            {
                LicenciaHidUser licenseUserHID = await _unitOfWork.LicenciaUserHIDRepository.GetById(licenseUserHIDId);
                return licenseUserHID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<LicenciaHidUser> GetByUserHIDId(int UserHIDId)
        {
            try
            {
                var licenseUserHID = await _unitOfWork.LicenciaUserHIDRepository.GetUserHID(l => l.UserId == (int?)UserHIDId);
                return licenseUserHID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CodigoInvitacionEmailHIDDTO> SendInvitationCodeByEmailHID(CodigoInvitacionHIDDTO invitationRequest, Guid clientCompanyId, Guid currentUserId)
        {
            try
            {
                if (invitationRequest == null)
                {
                    return CodigoInvitacionEmailHIDDTO.Invalid("La solicitud recibida no es válida.", "INVALID_REQUEST", "Error");
                }

                if (invitationRequest.UserId <= 0)
                {
                    return CodigoInvitacionEmailHIDDTO.Invalid(
                        "El identificador de usuario es obligatorio y debe ser válido.",
                        "USERID_REQUIRED",
                        "Error");
                }

                if (string.IsNullOrWhiteSpace(invitationRequest.Email))
                {
                    return CodigoInvitacionEmailHIDDTO.Invalid(
                        "El correo electrónico es obligatorio.",
                        "EMAIL_REQUIRED",
                        "Error");
                }

                var user = await GetById(invitationRequest.Id);
                if (user == null)
                {
                    return CodigoInvitacionEmailHIDDTO.Invalid("El usuario no existe o no fue encontrado.", "USER_NOT_FOUND", "Error");
                }

                var currentStatus = GetInvitationStatus(user.InvitacionActividad);
                if (currentStatus != InvitationStatus.Pending)
                {
                    //return Models.Response.Organizacion.CodigoInvitacionEmailHIDDTO.Invalid(
                    //    $"El estado actual de la invitación no permite esta acción: {currentStatus}",
                    //    "INVALID_STATUS",
                    //    currentStatus.ToString());
                    return CodigoInvitacionEmailHIDDTO.Invalid(
                        $"Este código ya ha sido escaneado/activado y no puede reenviarse",
                        "INVALID_STATUS",
                        currentStatus.ToString());
                }

                var utcNow = DateTime.UtcNow;
                if (!user.InvitacionExpirationDate.HasValue || user.InvitacionExpirationDate.Value <= utcNow)
                {
                    var errorCode = !user.InvitacionExpirationDate.HasValue ? "MISSING_EXPIRATION" : "EXPIRED_CODE";
                    var message = !user.InvitacionExpirationDate.HasValue
                        ? "La invitación no tiene una fecha de expiración configurada."
                        : "El código de invitación ha expirado.";

                    return CodigoInvitacionEmailHIDDTO.Invalid(
                        message,
                        errorCode,
                        InvitationStatus.Expired.ToString());
                }

                if (!ValidateInvitationCode(user.InvitacionDetalle, invitationRequest.InvitacionDetalle))
                {
                    return CodigoInvitacionEmailHIDDTO.Invalid(
                        "El código de invitación proporcionado no coincide.",
                        "INVALID_CODE",
                        currentStatus.ToString());
                }
                bool exito = await _emailService.SendEmailInvitationCodeHID(
    invitationRequest.Email,
    invitationRequest.InvitacionDetalle,
    user.InvitacionExpirationDate.Value,
    clientCompanyId
);

                return CodigoInvitacionEmailHIDDTO.Valid(
                    user.InvitacionExpirationDate.Value,
                    InvitationStatus.Sent.ToString());
            }
            catch (Exception)
            {
                return CodigoInvitacionEmailHIDDTO.Invalid(
                    "Error interno del sistema. Por favor, inténtelo de nuevo más tarde.",
                    "INTERNAL_ERROR",
                    "Critical");
            }
        }

        private InvitationStatus GetInvitationStatus(string status)
        {
            return InvitationStatusMapper.GetStatusFromString(status);
        }

        private bool ValidateInvitationCode(string storedCode, string receivedCode)
        {
            return !string.IsNullOrWhiteSpace(storedCode)
                && storedCode.Equals(receivedCode, StringComparison.Ordinal);
        }

        private async Task<bool> InactivateAllDevicesAndCredentials(CodigoInvitacionHIDDTO invitationCodeHID, Guid currentUserId, Guid clientCompanyId)
        {
            try
            {
                var deviceHID = await GetAndInactivateDevices(invitationCodeHID.Id, currentUserId, clientCompanyId);
                var credentialHID = await GetAndInactivateCredentials(invitationCodeHID.Id, currentUserId, clientCompanyId);
                return deviceHID && credentialHID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> GetAndInactivateDevices(Guid usuarioId, Guid currentUserId, Guid clientCompanyId)
        {
            var filter = new DipositivosHIDQueryFilter()
            {
                PageNumber = 1,
                PageSize = 10000,
                UsuarioId = usuarioId,
                Estado = 1
            };

            var devices = await _dipositivosHIDService.GetAll(filter, clientCompanyId);
            if (devices == null || !devices.Any()) return true;

            var inactivationTasks = devices.Select(async item =>
                await _dipositivosHIDService.Inactivate(item.Id, currentUserId)
            );

            await Task.WhenAll(inactivationTasks);
            return true;
        }

        private async Task<bool> GetAndInactivateCredentials(Guid usuarioId, Guid currentUserId, Guid clientCompanyId)
        {
            var filter = new CredencialHIDQueryFilter()
            {
                PageNumber = 1,
                PageSize = 10000,
                Usuarioid = usuarioId,
                Estado = 1
            };

            var credentials = await _credencialHIDService.GetAll(filter, clientCompanyId);
            if (credentials == null || !credentials.Any()) return true;

            var inactivationTasks = credentials.Select(async item =>
               await _credencialHIDService.Inactivate(item.Id, currentUserId)
            );

            await Task.WhenAll(inactivationTasks);
            return true;
        }

        public async Task<UserHIDExpired> GetExpired(Guid id)
        {
            try
            {
                var licenciaUserRepo = await _unitOfWork.LicenciaUserHIDRepository.GetUserHIDExpired(id);
                return licenciaUserRepo;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consultar el estado de la licencia HID para el usuario con ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<List<UserHIDExpired>> GetAllExpired()
        {
            try
            {
                var licenciaUserRepo = await _unitOfWork.LicenciaUserHIDRepository.GetAllUsersHIDExpired();
                return licenciaUserRepo;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserHIDWithCredentialsDTO?> GetWithCredentials(Guid externalId)
        {
            // Validar que ambos parámetros no sean Guid.Empty
            if (externalId == Guid.Empty)
                return null;

            try
            {
                // Llamar al repositorio
                var licenseUserHID = await _unitOfWork.LicenciaUserHIDRepository
                    .GetUserHIDWithCredential(externalId);

                // Verificar si se obtuvo un usuario válido con credenciales
                if (licenseUserHID?.Credenciales == null || !licenseUserHID.Credenciales.Any())
                    return null;


                var firstCredential = licenseUserHID.Credenciales.FirstOrDefault();

                var dto = new UserHIDWithCredentialsDTO
                {
                    Id = licenseUserHID.Id,
                    LicenciaId = licenseUserHID.LicenciaId,
                    Nombre = licenseUserHID.Nombre,
                    Email = licenseUserHID.Email,
                    UserId = licenseUserHID.UserId,
                    Site = licenseUserHID.Site,
                    Alert = licenseUserHID.Alert,
                    LicenseCount = licenseUserHID.LicenseCount,
                    Telefono = licenseUserHID.Telefono,
                    InvitacionFecha = licenseUserHID.InvitacionFecha,
                    InvitacionExpirationDate = licenseUserHID.InvitacionExpirationDate,
                    InvitacionActividad = licenseUserHID.InvitacionActividad,
                    InvitacionDetalle = licenseUserHID.InvitacionDetalle,
                    InvitacionId = licenseUserHID.InvitacionId,
                    Status = licenseUserHID.Status,
                    Apellidos = licenseUserHID.Apellidos,
                    FechaInicio = licenseUserHID.FechaInicio,
                    FechaFin = licenseUserHID.FechaFin,
                    ExternalId = licenseUserHID.ExternalId,
                    EmpresaClienteId = licenseUserHID.EmpresaClienteId,
                    Credencial = firstCredential == null ? null : new CredentialWithUserHIDDTO
                    {
                        Id = firstCredential.Id,
                        TipoCredencial = firstCredential.TipoCredencial,
                        DispositivoId = firstCredential.DispositivoId,
                        Usuarioid = firstCredential.Usuarioid,
                        CredencialValor = firstCredential.CredencialValor,
                        Validity = firstCredential.Validity,
                        Status = firstCredential.Status,
                        EmpresaClienteId = firstCredential.EmpresaClienteId,
                    }
                };

                return dto;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<PagedList<CommonDTO>> GetAllQuery(LicenciaUserHIDQueryFilter filters)
        {
            PagedList<CommonDTO> pagedCredentialDevice = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<LicenciaHidUser> userHID = filters.DatosCompletos == 2 ? _unitOfWork.LicenciaUserHIDRepository.GetAll() : await _unitOfWork.LicenciaUserHIDRepository.GetAllUserHID();
                if (filters.Estado != null && filters.Estado > 0) { userHID = userHID.Where(x => x.Estado == filters.Estado); }

                List<CommonDTO> userHIDSelect = new List<CommonDTO>();
                if (filters.TipoQuery == "UserByName")
                {
                    userHIDSelect = userHID.Select(
                        s => new CommonDTO()
                        {
                            Id = s.Id,
                            Nombre = string.Join(" ", new[] { s.Nombre, s.Apellidos }.Where(x => !string.IsNullOrWhiteSpace(x)))
                        })
                        .GroupBy(x => x.Nombre, StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.First())
                        .ToList();
                }

                if (filters.TipoQuery == "UserByEmail")
                {
                    userHIDSelect = userHID.Select(
                        s => new CommonDTO()
                        {
                            Id = s.Id,
                            Nombre = s.Email
                        })
                        .GroupBy(x => x.Nombre, StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.First())
                        .ToList();
                }

                if (filters.TipoQuery == "UserByPhone")
                {
                    userHIDSelect = userHID.Select(
                        s => new CommonDTO()
                        {
                            Id = s.Id,
                            Nombre = s.Telefono ?? string.Empty
                        })
                        .GroupBy(x => x.Nombre, StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.First())
                        .ToList();
                }

                pagedCredentialDevice = PagedList<CommonDTO>.Create(userHIDSelect, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pagedCredentialDevice;
        }
    }

    public static class InvitationStatusMapper
    {
        public static InvitationStatus GetStatusFromString(string status)
        {
            return status.ToUpperInvariant() switch
            {
                "PENDING" => InvitationStatus.Pending,
                "SENT" => InvitationStatus.Sent,
                "ACCEPTED" => InvitationStatus.Accepted,
                "EXPIRED" => InvitationStatus.Expired,
                "ACKNOWLEDGED" => InvitationStatus.Acknowledged,
                "CANCELLED" => InvitationStatus.Cancelled,
                _ => InvitationStatus.Pending
            };
        }
    }
}
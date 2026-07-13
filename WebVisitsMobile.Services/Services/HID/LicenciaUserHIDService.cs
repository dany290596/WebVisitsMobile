using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Numerics;
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
using WebVisitsMobile.Models.Organizacion.Tarea.Tarea;
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
        private readonly IDipositivosHIDService _dipositivosHIDService;
        private readonly ICredencialHIDService _credencialHIDService;
        private readonly ITareaService _tareaService;
        private readonly IEmailService _emailService;
        private readonly IHostEnvironment _env;
        private readonly IPlantillaCredencialService _plantillaCredencialService;

        private static readonly object _logLock = new();

        private static void Log(string message)
        {
            var timestamp = DateTime.Now;
            var line = $"[{timestamp:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            Console.WriteLine(line);
            try
            {
                var dir = @"C:\logs";
                Directory.CreateDirectory(dir);
                var file = System.IO.Path.Combine(dir, $"webvisitsMobilecallbacks_{timestamp:yyyy-MM-dd}.txt");
                lock (_logLock)
                    System.IO.File.AppendAllText(file, line + Environment.NewLine);
            }
            catch { }
        }

        private static readonly Guid CREDENCIAL_HID = Guid.Parse("1A2B3C4D-5E6F-7890-ABCD-EF1234567890");
        private static readonly Guid CREDENCIAL_WALLET = Guid.Parse("2B3C4D5E-6F70-8901-BCDE-F12345678901");
        private static readonly Guid HID_ADD = Guid.Parse("3D68F904-2A4A-40BD-BB62-09A95B7247F5");
        private static readonly Guid WALLET_ADD = Guid.Parse("FD82D317-F02C-4A26-86F4-23766E029BC0");
        private static readonly Guid WALLET_TEMPLATE = Guid.Parse("022047D9-2E61-44A5-ADE4-AEB2F69CDC17");
        private static readonly Guid WALLET_CORREO = Guid.Parse("7E468AF2-6A24-4F95-9B99-F542E605855F");

        public enum InvitationStatus
        {
            Pending = 1,
            Sent = 2,
            Accepted = 3,
            Expired = 4,
            Acknowledged = 5,
            Cancelled = 6,
            Inactive = 7,
            Reactivate = 8
        }

        public LicenciaUserHIDService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options,
            IDipositivosHIDService dipositivosHIDService,
            ICredencialHIDService credencialHIDService,
            ITareaService tareaService,
            IEmailService emailService,
            IHostEnvironment env,
            IPlantillaCredencialService plantillaCredencialService
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
            _dipositivosHIDService = dipositivosHIDService;
            _credencialHIDService = credencialHIDService;
            _tareaService = tareaService;
            _emailService = emailService;
            _env = env;
            _plantillaCredencialService = plantillaCredencialService;
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

        /*
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

                if (licenseUserHID.UsuarioHidTipoCredencial.Count > 0)
                {
                    foreach (var x in licenseUserHID.UsuarioHidTipoCredencial)
                    {
                        x.Id = Guid.NewGuid();
                        x.Estado = 1;
                        x.FechaCreacion = DateTime.Now;
                        x.UsuarioCreadorId = currentUserId;

                        if (x.TipoCredencialId == new Guid("1A2B3C4D-5E6F-7890-ABCD-EF1234567890"))
                        {
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
                                task.ValorEnvio = JsonSerializer.Serialize(licenseUserHID, jsonOptions);
                                task.ValorRetorno = "";
                                task.ReferenciaId = licenseUserHID.Id;
                                task.EmpresaClienteId = currentClientCompanyId;
                                task.Id = Guid.NewGuid();
                                task.UsuarioCreadorId = currentUserId;
                                task.FechaCreacion = DateTime.Now;
                                task.Estado = 1;

                                await _unitOfWork.TareaRepository.Add(task);
                            }
                        }
                        if (x.TipoCredencialId == new Guid("2B3C4D5E-6F70-8901-BCDE-F12345678901"))
                        {
                            var taskById = await _unitOfWork.TipoTareaRepository.GetById(new Guid("FD82D317-F02C-4A26-86F4-23766E029BC0"));
                            if (taskById != null)
                            {
                                var jsonOptions = new JsonSerializerOptions
                                {
                                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                    WriteIndented = false,
                                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                                };

                                Tarea task = new Tarea();
                                task.TipoTareaId = new Guid("FD82D317-F02C-4A26-86F4-23766E029BC0");
                                task.Fecha = DateTime.Now;
                                task.Pendiente = 1;
                                task.Status = 1;
                                task.ValorEnvio = JsonSerializer.Serialize(licenseUserHID, jsonOptions);
                                task.ValorRetorno = "";
                                task.ReferenciaId = licenseUserHID.Id;
                                task.EmpresaClienteId = currentClientCompanyId;
                                task.Id = Guid.NewGuid();
                                task.UsuarioCreadorId = currentUserId;
                                task.FechaCreacion = DateTime.Now;
                                task.Estado = 1;

                                await _unitOfWork.TareaRepository.Add(task);
                            }
                        }
                    }
                }

                await _unitOfWork.LicenciaUserHIDRepository.Add(licenseUserHID);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }
        */

        public async Task<bool> Create(LicenciaHidUser licenseUserHID, Guid currentClientCompanyId, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                // 1. Obtener los tipos de credencial seleccionados (evitar duplicados)
                var tiposSeleccionados = licenseUserHID.UsuarioHidTipoCredencial
                    .Select(x => x.TipoCredencialId)
                    .Distinct()
                    .ToList();

                if (tiposSeleccionados.Count == 0)
                {
                    // No se seleccionó ningún tipo – puedes decidir si devolver false o lanzar excepción
                    return false;
                }

                string baseFolder = Path.Combine(_env.ContentRootPath, "FOTOS_USUARIOS_HID");
                Directory.CreateDirectory(baseFolder);

                foreach (var tipoId in tiposSeleccionados)
                {
                    var nuevoId = Guid.NewGuid();
                    var imagenId = Guid.NewGuid();

                    var imagenBase64 = licenseUserHID.Imagen;
                    var nombreImagen = "";

                    if (!string.IsNullOrWhiteSpace(imagenBase64))
                    {
                        //Se crea el nombre de la foto que se compone por el ID de colaborador y la extencion de la foto
                        string imageName = imagenId.ToString() + '.' + licenseUserHID.ExtensionImagen;

                        //Se forma el path donde estara la imagen almacenada en nuestro servidor
                        string imgPath = Path.Combine(baseFolder, imageName);

                        //La imagen base 64 se trasforma a una imagen por bytes
                        byte[] imageBytes = Convert.FromBase64String(imagenBase64);

                        //La foto se escribe dentro de la carpeta seleccionada por  medio del path
                        File.WriteAllBytes(imgPath, imageBytes);

                        //En colaborador foto se guarda el nombre de la foto
                        nombreImagen = imageName;
                    }

                    // Crear una copia limpia del usuario con nuevos identificadores


                    var nuevoUsuario = new LicenciaHidUser
                    {
                        Id = nuevoId,
                        LicenciaId = licenseUserHID.LicenciaId,
                        Nombre = licenseUserHID.Nombre,
                        Email = licenseUserHID.Email,
                        Telefono = licenseUserHID.Telefono,
                        Apellidos = licenseUserHID.Apellidos,
                        FechaInicio = licenseUserHID.FechaInicio?.Date,
                        FechaFin = licenseUserHID.FechaFin,
                        LicenseCount = 1,
                        EmpresaClienteId = currentClientCompanyId,
                        Status = 1,
                        Estado = 1,
                        UsuarioCreadorId = currentUserId,
                        FechaCreacion = DateTime.Now,
                        ExternalId = licenseUserHID.ExternalId ?? nuevoId,  // o asignar el nuevo Id
                        ExtensionImagen = licenseUserHID.ExtensionImagen,
                        Imagen = nombreImagen,

                        PlantillaCredencialId = tipoId == CREDENCIAL_WALLET
                        ? licenseUserHID.PlantillaCredencialId
                        : null,

                        Plataforma = tipoId == CREDENCIAL_WALLET
                        ? licenseUserHID.Plataforma
                        : null
                    };

                    // 3. Crear el enlace único hacia el tipo de credencial
                    var enlace = new UsuarioHidTipoCredencial
                    {
                        Id = Guid.NewGuid(),
                        LicenciaHidUserId = nuevoUsuario.Id,
                        TipoCredencialId = tipoId,
                        Estado = 1,
                        FechaCreacion = DateTime.Now,
                        UsuarioCreadorId = currentUserId
                    };
                    nuevoUsuario.UsuarioHidTipoCredencial.Add(enlace);

                    // 4. Determinar el TipoTarea correspondiente
                    var tiposTarea = new List<Guid>();
                    if (tipoId == CREDENCIAL_HID)
                    {
                        tiposTarea.Add(HID_ADD); // HID - ADD
                    }
                    else if (tipoId == CREDENCIAL_WALLET)
                    {
                        tiposTarea.Add(WALLET_ADD); // Wallet - ADD
                    }

                    // 5. Crear la tarea para este usuario (solo si existe el tipo de tarea)
                    foreach (var tipoTareaId in tiposTarea)
                    {
                        var typeTask = await _unitOfWork.TipoTareaRepository.GetById(tipoTareaId);
                        if (typeTask != null)
                        {
                            if (typeTask.Id == HID_ADD)
                            {
                                var jsonOptions = new JsonSerializerOptions
                                {
                                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                    WriteIndented = false,
                                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                                };

                                //                            var hidAdd = new TareaUsuarioHIDDTO()
                                //                            {
                                //    Id = 
                                //    LicenciaId = 
                                //    Nombre = 
                                //    Email =
                                //    UserId =
                                //    Site = 
                                //    Alert = 
                                //    LicenseCount = 
                                //    Telefono = 
                                //    InvitacionFecha = 
                                //    InvitacionExpirationDate = 
                                //    InvitacionActividad = 
                                //    InvitacionDetalle = 
                                //    InvitacionId = 
                                //    Apellidos = 
                                //};

                                var tarea = new Tarea
                                {
                                    Id = Guid.NewGuid(),
                                    TipoTareaId = tipoTareaId,
                                    Fecha = DateTime.Now,
                                    Pendiente = 1,
                                    Status = 1,
                                    ValorEnvio = JsonSerializer.Serialize(nuevoUsuario, jsonOptions),
                                    ValorRetorno = "",
                                    ReferenciaId = nuevoUsuario.Id,
                                    Marca = 0,
                                    EmpresaClienteId = currentClientCompanyId,
                                    UsuarioCreadorId = currentUserId,
                                    FechaCreacion = DateTime.Now,
                                    Estado = 1
                                };

                                await _unitOfWork.TareaRepository.Add(tarea);
                            }

                            if (typeTask.Id == WALLET_ADD)
                            {
                                var jsonOptions = new JsonSerializerOptions
                                {
                                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                    WriteIndented = false,
                                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                                };

                                var templateAdd = nuevoUsuario.PlantillaCredencialId is Guid plantillaId
                                    ? await _plantillaCredencialService.GetById(plantillaId, currentClientCompanyId)
                                    : null;

                                var walletAdd = new TareaUsuarioHIDWalletDTO()
                                {
                                    Id = nuevoUsuario.Id,
                                    DisplayName = nuevoUsuario.Nombre + " " + nuevoUsuario.Apellidos,
                                    ExternalId = nuevoUsuario.ExternalId.ToString()!,
                                    Emails = nuevoUsuario.Email,
                                    Telefono = nuevoUsuario.Telefono!,
                                    FechaInicio = nuevoUsuario.FechaInicio,
                                    FechaFin = nuevoUsuario.FechaFin,
                                    Plataforma = nuevoUsuario.Plataforma,
                                    Plantilla = templateAdd == null
                                    ? null
                                    : new TareaPlantillaCredencialDTO
                                    {
                                        Id = templateAdd.Id,
                                        BackgroundExternalId = templateAdd.BackgroundExternalId,
                                        LogoExternalId = templateAdd.LogoExternalId,
                                        ExternalId = templateAdd.ExternalId,
                                        AppleId = templateAdd.AppleId
                                    }
                                };

                                var tarea = new Tarea
                                {
                                    Id = Guid.NewGuid(),
                                    TipoTareaId = tipoTareaId,
                                    Fecha = DateTime.Now,
                                    Pendiente = 1,
                                    Status = 1,
                                    ValorEnvio = JsonSerializer.Serialize(walletAdd, jsonOptions),
                                    ValorRetorno = "",
                                    ReferenciaId = nuevoUsuario.Id,
                                    Marca = 0,
                                    EmpresaClienteId = currentClientCompanyId,
                                    UsuarioCreadorId = currentUserId,
                                    FechaCreacion = DateTime.Now,
                                    Estado = 1
                                };

                                await _unitOfWork.TareaRepository.Add(tarea);
                            }
                        }
                    }

                    // 6. Agregar el nuevo usuario al repositorio (el enlace se guarda por cascada)
                    await _unitOfWork.LicenciaUserHIDRepository.Add(nuevoUsuario);
                }

                // 7. Persistir todos los cambios en una sola transacción
                await _unitOfWork.SaveChangesAsync();
                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
                // Considera loguear el error
            }

            return booOk;
        }

        public async Task<bool> CreateTypeCredential(UserHIDTypeCredentialReqDTO licenseUserHID, Guid currentClientCompanyId, Guid currentUserId)
        {
            bool booOk = false;
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


        public async Task<Tarea> ActualizarCredencial(string correo, Guid clientCompanyId, Guid currentUserId)
        {
            Tarea tarea = new Tarea();

            try
            {

                LicenciaHidUser licenciaHidUser = await _unitOfWork.LicenciaUserHIDRepository.GetUserActivoEmail(correo);

                if (licenciaHidUser==null)
                {
                    return null;
                }


                var jsonOptions = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                var templateAdd = licenciaHidUser.PlantillaCredencialId is Guid plantillaId
                    ? await _plantillaCredencialService.GetById(plantillaId, (Guid)licenciaHidUser.EmpresaClienteId)
                    : null;

                var walletAdd = new TareaUsuarioHIDWalletDTO()
                {
                    Id = licenciaHidUser.Id,
                    DisplayName = licenciaHidUser.Nombre + " " + licenciaHidUser.Apellidos,
                    ExternalId = licenciaHidUser.ExternalId.ToString()!,
                    UsuarioWalletId = licenciaHidUser.UsuarioWalletId,
                    Emails = licenciaHidUser.Email,
                    Telefono = licenciaHidUser.Telefono!,
                    FechaInicio = licenciaHidUser.FechaInicio,
                    FechaFin = licenciaHidUser.FechaFin,
                    Plataforma = licenciaHidUser.Plataforma,
                    Plantilla = templateAdd == null
                    ? null
                    : new TareaPlantillaCredencialDTO
                    {
                        Id = templateAdd.Id,
                        BackgroundExternalId = templateAdd.BackgroundExternalId,
                        LogoExternalId = templateAdd.LogoExternalId,
                        ExternalId = templateAdd.ExternalId,
                        AppleId = templateAdd.AppleId
                    }
                };

                var tareaNew = new Tarea
                {
                    Id = Guid.NewGuid(),
                    TipoTareaId = new Guid("17034A93-A868-4B9B-90C1-5FE689E9381B"),
                    Fecha = DateTime.Now,
                    Pendiente = 1,
                    Status = 1,
                    ValorEnvio = JsonSerializer.Serialize(walletAdd, jsonOptions),
                    ValorRetorno = "",
                    ReferenciaId = licenciaHidUser.Id,
                    Marca = 0,
                    EmpresaClienteId = licenciaHidUser.EmpresaClienteId,
                    UsuarioCreadorId = currentUserId,
                    FechaCreacion = DateTime.Now,
                    Estado = 1
                };

                await _unitOfWork.TareaRepository.Add(tareaNew);

                // 7. Persistir todos los cambios en una sola transacción
                await _unitOfWork.SaveChangesAsync();

                tarea = tareaNew;

            }
            catch (Exception)
            {

                return null;
            }

            return tarea;
        }

        public async Task<LicenciaHidUser?> UpdatePartial(LicenciaHidUser licenseUserHID, Guid clientCompanyId, Guid currentUserId)
        {
            try
            {
                LicenciaHidUser licenseUserHIDUpdate = await _unitOfWork.LicenciaUserHIDRepository.GetById(licenseUserHID.Id);
                if (licenseUserHIDUpdate == null) { return null; }

                if (licenseUserHID.UsuarioWalletId != null && licenseUserHID.UsuarioWalletId != Guid.Empty)
                    licenseUserHIDUpdate.UsuarioWalletId = licenseUserHID.UsuarioWalletId;

                if (licenseUserHID.InvitacionFecha.HasValue)
                    licenseUserHIDUpdate.InvitacionFecha = licenseUserHID.InvitacionFecha;

                if (licenseUserHID.InvitacionExpirationDate.HasValue)
                    licenseUserHIDUpdate.InvitacionExpirationDate = licenseUserHID.InvitacionExpirationDate;

                if (!string.IsNullOrWhiteSpace(licenseUserHID.InvitacionDetalle))
                    licenseUserHIDUpdate.InvitacionDetalle = licenseUserHID.InvitacionDetalle;

                if (licenseUserHID.Status.HasValue && licenseUserHID.Status.Value > 0)
                    licenseUserHIDUpdate.Status = licenseUserHID.Status;

                licenseUserHIDUpdate.FechaModificacion = DateTime.Now;
                licenseUserHIDUpdate.UsuarioModificadorId = currentUserId;
                licenseUserHIDUpdate.Status = 2;
                licenseUserHIDUpdate.InvitacionActividad = licenseUserHID.InvitacionActividad;

                _unitOfWork.LicenciaUserHIDRepository.Update(licenseUserHIDUpdate);
                await _unitOfWork.SaveChangesAsync();

                var tipoTarea = await _unitOfWork.TipoTareaRepository.GetById(WALLET_CORREO);
                if (tipoTarea != null)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles
                    };

                    var payload = new TareaWalletUpdateDTO
                    {
                        CorreoElectronico = licenseUserHIDUpdate.Email ?? string.Empty,
                        FechaInicio = licenseUserHIDUpdate.FechaInicio,
                        FechaFin = licenseUserHIDUpdate.FechaFin,
                        Plataforma = licenseUserHIDUpdate.Plataforma,
                        NombreCompleto = string.Join(" ", new[]
                        {
                            licenseUserHIDUpdate.Nombre,
                            licenseUserHIDUpdate.Apellidos
                        }.Where(x => !string.IsNullOrWhiteSpace(x))),
                        CodigoInvitacion = licenseUserHIDUpdate.InvitacionDetalle
                    };

                    var tarea = new Tarea
                    {
                        Id = Guid.NewGuid(),
                        TipoTareaId = WALLET_CORREO,
                        Fecha = DateTime.Now,
                        Pendiente = 1,
                        Status = 1,
                        ValorEnvio = JsonSerializer.Serialize(payload, jsonOptions),
                        ValorRetorno = "",
                        ReferenciaId = licenseUserHIDUpdate.Id,
                        Marca = 0,
                        EmpresaClienteId = clientCompanyId,
                        UsuarioCreadorId = currentUserId,
                        FechaCreacion = DateTime.Now,
                        Estado = 1
                    };

                    await _unitOfWork.TareaRepository.Add(tarea);
                    await _unitOfWork.SaveChangesAsync();
                }

                return licenseUserHIDUpdate;
            }
            catch (Exception ex)
            {
                return null;
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

        public async Task<bool> UpdateStatus(Guid userId, int status)
        {
            try
            {
                // Intento 1 — buscar por UsuarioWalletId
                LicenciaHidUser? data = await _unitOfWork.LicenciaUserHIDRepository.GetUserWalletId(userId);

                // Intento 2 — buscar por ExternalId
                if (data == null)
                {
                    Log($"[HIDOrigo] UpdateStatus ⚠️  No encontrado por UsuarioWalletId, intentando ExternalId...");
                    data = await _unitOfWork.LicenciaUserHIDRepository
                        .GetUserHID(u => u.ExternalId == userId);
                }

                if (data == null)
                {
                    Log($"[HIDOrigo] UpdateStatus ❌ No encontrado con ningún campo para: {userId}");
                    return false;
                }

                data.Status = status;
                data.FechaModificacion = DateTime.Now;
                _unitOfWork.LicenciaUserHIDRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                Log($"[HIDOrigo] UpdateStatus ✅ Status actualizado a {status} para: {userId}");
                return true;
            }
            catch (Exception ex)
            {
                Log($"[HIDOrigo] UpdateStatus ❌ Exception: {ex.Message}");
                return false;
            }
        }

        // Buscar por UserId entero (ej: 165901445)
        public async Task<bool> UpdateStatusByIntId(int userId, int status)
        {
            try
            {
                LicenciaHidUser? data = await _unitOfWork.LicenciaUserHIDRepository
                    .GetUserHID(u => u.UserId == userId);

                if (data == null)
                {
                    Log($"[HIDOrigo] UpdateStatusByIntId ❌ No encontrado para UserId: {userId}");
                    return false;
                }

                data.Status = status;
                data.FechaModificacion = DateTime.Now;

                _unitOfWork.LicenciaUserHIDRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                Log($"[HIDOrigo] UpdateStatusByIntId ✅ Status actualizado a {status} para UserId: {userId}");
                return true;
            }
            catch (Exception ex)
            {
                Log($"[HIDOrigo] UpdateStatusByIntId ❌ Exception: {ex.Message}");
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

        public async Task<bool> InactivateWithWalletAndTask(Guid id, Guid currentUserId, Guid clientCompanyId)
        {
            if (id == Guid.Empty || currentUserId == Guid.Empty)
                return false;

            LicenciaHidUser data = await _unitOfWork.LicenciaUserHIDRepository.GetById(id);
            if (data == null) { return false; }

            var taskById = await _unitOfWork.TipoTareaRepository.GetById(new Guid("8BB6A16A-E148-4952-97A7-76106F048E5D"));
            if (taskById == null)
            {
                return false;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var valor = new TareaWalletInactivateDTO()
            {
                ExternalId = data.UsuarioWalletId
            };

            Tarea task = new Tarea
            {
                TipoTareaId = taskById.Id,
                Fecha = DateTime.Now,
                Pendiente = 1,
                Status = 1,
                ValorEnvio = JsonSerializer.Serialize(valor, jsonOptions),
                ValorRetorno = "",
                ReferenciaId = data.ExternalId,
                Marca = 0,
                EmpresaClienteId = clientCompanyId,
                Id = Guid.NewGuid(),
                UsuarioCreadorId = currentUserId,
                FechaCreacion = DateTime.Now,
                Estado = 1
            };

            TareaQueryFilter tareaQueryFilter = new TareaQueryFilter();
            // tareaQueryFilter.Pendiente = 4; // NO PREOCESADA
            tareaQueryFilter.ReferenciaId = data.ExternalId;
            tareaQueryFilter.TipoTareaId = taskById.Id;
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
                    var userResult = await UpdateStatus(id, 7, "INACTIVATE", currentUserId);
                    if (!userResult)
                        return false;

                    await _unitOfWork.TareaRepository.Add(task);
                    await _unitOfWork.SaveChangesAsync();

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

                var userResult = await UpdateStatus((Guid)tarea.ReferenciaId, 7, "INACTIVATE", currentUserId);
                if (!userResult)
                    return false;
            }

            await _unitOfWork.TareaRepository.Add(task);
            await _unitOfWork.SaveChangesAsync();


            return await Inactivate(id, currentUserId);
        }

        public async Task<bool> ReactivateWithWalletAndTask(Guid id, Guid currentUserId, Guid clientCompanyId)
        {
            if (id == Guid.Empty || currentUserId == Guid.Empty)
                return false;

            LicenciaHidUser data = await _unitOfWork.LicenciaUserHIDRepository.GetById(id);
            if (data == null) { return false; }

            var taskById = await _unitOfWork.TipoTareaRepository.GetById(new Guid("1116C4E2-8B24-4EBD-A938-4123291956F5"));
            if (taskById == null)
            {
                return false;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var valor = new TareaWalletReactivate()
            {
                ExternalId = data.ExternalId
            };

            Tarea task = new Tarea
            {
                TipoTareaId = taskById.Id,
                Fecha = DateTime.Now,
                Pendiente = 1,
                Status = 1,
                ValorEnvio = JsonSerializer.Serialize(valor, jsonOptions),
                ValorRetorno = "",
                ReferenciaId = data.ExternalId,
                Marca = 0,
                EmpresaClienteId = clientCompanyId,
                Id = Guid.NewGuid(),
                UsuarioCreadorId = currentUserId,
                FechaCreacion = DateTime.Now,
                Estado = 1
            };

            TareaQueryFilter tareaQueryFilter = new TareaQueryFilter();
            // tareaQueryFilter.Pendiente = 4; // NO PREOCESADA
            tareaQueryFilter.ReferenciaId = data.ExternalId;
            tareaQueryFilter.TipoTareaId = taskById.Id;
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
                    var userResult = await UpdateStatus(id, 8, "REACTIVATE", currentUserId);
                    if (!userResult)
                        return false;

                    await _unitOfWork.TareaRepository.Add(task);
                    await _unitOfWork.SaveChangesAsync();

                    return await Reactivate(id, currentUserId);
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

                var userResult = await UpdateStatus((Guid)tarea.ReferenciaId, 8, "REACTIVATE", currentUserId);
                if (!userResult)
                    return false;
            }

            await _unitOfWork.TareaRepository.Add(task);
            await _unitOfWork.SaveChangesAsync();


            return await Reactivate(id, currentUserId);
        }

        public async Task<bool> InactivateWithHID(Guid id, Guid clientCompanyId, Guid currentUserId)
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
                LicenciaHidUser licenseUserHID = await _unitOfWork.LicenciaUserHIDRepository.GetById(id);
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
                        Marca = 0,
                        EmpresaClienteId = clientCompanyId,
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
                LicenciaHidUser data = await _unitOfWork.LicenciaUserHIDRepository.GetById(licenseUserHIDId);
                if (data == null) { return null; }
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<LicenciaHidUser?> GetByPhoto(Guid licenseUserHIDId)
        {
            try
            {
                UsuarioHidTipoCredencial dataTC = await _unitOfWork.UsuarioHidTipoCredencialRepository.GetById(licenseUserHIDId);
                if (dataTC == null) { return null; }
                LicenciaHidUser data = await _unitOfWork.LicenciaUserHIDRepository.GetById(dataTC.LicenciaHidUserId);
                // Cargar las imágenes en Base64 para preview
                string baseFolder = Path.Combine(_env.ContentRootPath, "FOTOS_USUARIOS_HID");

                // Imagen de fondo
                if (!string.IsNullOrEmpty(data.Imagen) && data.Imagen != "Sin foto" && data.Imagen != "")
                {
                    string fondoPath = Path.Combine(baseFolder, data.Imagen);
                    data.ImagenBase64 = await GetImageBase64(fondoPath);
                }

                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<string?> GetImageBase64(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
            string extension = Path.GetExtension(filePath).TrimStart('.').ToLower();
            string mimeType = extension switch
            {
                "png" => "image/png",
                "jpg" or "jpeg" => "image/jpeg",
                "webp" => "image/webp",
                "svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
            return $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
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

        public async Task<LicenciaHidUser> GetByIdExpired(Guid licenseUserHIDId)
        {
            try
            {
                LicenciaHidUser licenseUserHID = await _unitOfWork.LicenciaUserHIDRepository.GetUserHID(s => s.Id == licenseUserHIDId);
                return licenseUserHID;
            }
            catch (Exception)
            {
                throw;
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

        public async Task<List<LicenciaHidUser>> GetAllLicenciasExpiradas()
        {
            try
            {
                return await _unitOfWork.LicenciaUserHIDRepository.GetAllLicenciasExpiradas();
            }
            catch (Exception)
            {
                throw;
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

        public async Task<bool> ExisteEmailEnLicenciaHidUser(string email)
        {
            try
            {
                return await _unitOfWork.LicenciaUserHIDRepository.ExisteEmailEnLicenciaHidUser(email);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string?> GetInvitacionDetalleVigenteByEmail(string email)
        {
            try
            {
                var registro = await _unitOfWork.LicenciaUserHIDRepository.GetLicenciaVigenteByEmail(email);
                return registro?.InvitacionDetalle;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<LicenciaHidUser?> GetByExternalId(Guid externalId)
        {
            if (externalId == Guid.Empty)
                return null;

            try
            {
                return await _unitOfWork.LicenciaUserHIDRepository.GetUserHID(u => u.ExternalId == externalId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> TieneCredencialWallet(Guid licenciaHidUserId)
        {
            try
            {
                var credencial = await _unitOfWork.UsuarioHidTipoCredencialRepository
                    .GetUserHidTypeCredential(x =>
                        x.LicenciaHidUserId == licenciaHidUserId &&
                        x.TipoCredencialId == CREDENCIAL_WALLET &&
                        x.Estado == 1);

                return credencial != null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string?> GetCredencialWalletMasReciente(Guid licenciaHidUserId)
        {
            try
            {
                return await _unitOfWork.CredencialHIDRepository.GetCredencialWalletMasReciente(licenciaHidUserId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string?> GetCredencialWalletMasRecienteWatch(Guid licenciaHidUserId)
        {
            try
            {
                return await _unitOfWork.CredencialHIDRepository.GetCredencialWalletMasRecienteWatch(licenciaHidUserId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> TieneCredencialOrigo(Guid licenciaHidUserId)
        {
            try
            {
                var credencial = await _unitOfWork.UsuarioHidTipoCredencialRepository
                    .GetUserHidTypeCredential(x =>
                        x.LicenciaHidUserId == licenciaHidUserId &&
                        x.TipoCredencialId == CREDENCIAL_HID &&
                        x.Estado == 1);

                return credencial != null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string?> GetCredencialOrigoMasReciente(Guid licenciaHidUserId)
        {
            try
            {
                return await _unitOfWork.CredencialHIDRepository.GetCredencialOrigoMasReciente(licenciaHidUserId);
            }
            catch (Exception)
            {
                throw;
            }
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
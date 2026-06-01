using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Models.Configuracion.Configuraciones;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.QueryFilters.Configuracion;

namespace WebVisitsMobile.Services.Services.Configuracion
{
    public class ConfiguracionService : IConfiguracionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        public ConfiguracionService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<PagedList<Configuraciones>> GetAll(ConfiguracionesQueryFilter filters)
        {
            PagedList<Configuraciones> pageSettings = null;
            try
            {

                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<Configuraciones> settings;

                if (filters.DatosCompletos == 0)
                {
                    settings = _unitOfWork.ConfiguracionesRepository.GetAll();
                }
                else
                {
                    settings = await _unitOfWork.ConfiguracionesRepository.GetAllSetting();
                }

                if (filters.EmpresaClienteId != null && filters.EmpresaClienteId != Guid.Empty) { settings = settings.Where(x => x.EmpresaClienteId == filters.EmpresaClienteId); }

                if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { settings = settings.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
                if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { settings = settings.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
                if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { settings = settings.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
                if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { settings = settings.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
                if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { settings = settings.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
                if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { settings = settings.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
                if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { settings = settings.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
                if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { settings = settings.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
                if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { settings = settings.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
                if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { settings = settings.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
                if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { settings = settings.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
                if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { settings = settings.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
                if (filters.Estado != null && filters.Estado > 0) { settings = settings.Where(x => x.Estado == filters.Estado); }

                pageSettings = PagedList<Configuraciones>.Create(settings, filters.PageNumber, filters.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }

            return pageSettings;
        }

        public async Task<Configuraciones> GetById(Guid settingId, Guid clientCompanyId)
        {
            try
            {
                Configuraciones setting = await _unitOfWork.ConfiguracionesRepository.GetSetting(s => s.Id == settingId && s.EmpresaClienteId == clientCompanyId);
                if (setting != null)
                {
                    return setting;
                }
                return new Configuraciones();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Configuraciones> GetById(Guid settingId)
        {
            try
            {
                Configuraciones setting = await _unitOfWork.ConfiguracionesRepository.GetById(settingId);
                if (setting != null)
                {
                    return setting;
                }
                return new Configuraciones();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Configuraciones> GetByTypeSettingAndCompanyId(Guid typeSetting, Guid clientCompanyId)
        {
            try
            {
                Configuraciones setting = await _unitOfWork.ConfiguracionesRepository.GetSetting(s => s.TipoConfiguracion == typeSetting && s.EmpresaClienteId == clientCompanyId);
                return setting;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Create(Configuraciones setting, Guid currentUserId)
        {
            bool booOk = false;

            try
            {
                setting.Id = Guid.NewGuid();
                setting.UsuarioCreadorId = currentUserId;
                setting.FechaCreacion = DateTime.Now;
                setting.Estado = 1;
                await _unitOfWork.ConfiguracionesRepository.Add(setting);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> Update(Configuraciones setting, Guid currentUserId, Guid clientCompanyId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                Configuraciones settingUpdate = await _unitOfWork.ConfiguracionesRepository.GetById(setting.Id);
                if (settingUpdate == null) { return false; }

                settingUpdate.NombreParametro = setting.NombreParametro;
                settingUpdate.ValorGuid = setting.ValorGuid;
                settingUpdate.Valor1 = setting.Valor1;
                settingUpdate.Valor2 = setting.Valor2;
                settingUpdate.Valor3 = setting.Valor3;
                settingUpdate.editable = setting.editable;
                settingUpdate.lectura = setting.lectura;
                settingUpdate.EmpresaClienteId = clientCompanyId;
                settingUpdate.FechaModificacion = DateTime.Now;
                settingUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.ConfiguracionesRepository.Update(settingUpdate);
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
                Configuraciones setting = await _unitOfWork.ConfiguracionesRepository.GetById(id);
                setting.FechaBaja = DateTime.Now;
                setting.UsuarioBajaId = currentUserId;
                setting.Estado = 2;

                _unitOfWork.ConfiguracionesRepository.Update(setting);
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
                Configuraciones setting = await _unitOfWork.ConfiguracionesRepository.GetById(id);
                setting.FechaReactivacion = DateTime.Now;
                setting.UsuarioReactivadorId = currentUserId;
                setting.Estado = 1;

                _unitOfWork.ConfiguracionesRepository.Update(setting);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> Update(List<ConfiguracionesListReqDTO> data)
        {
            try
            {
                // Extraer solo los IDs para la consulta
                var ids = data.Select(c => c.Id).ToList();

                // Obtener las entidades existentes
                var existingEntities = _unitOfWork.ConfiguracionesRepository.GetAll().Where(c => ids.Contains(c.Id)).ToList();

                foreach (var dto in data)
                {
                    var existing = existingEntities.FirstOrDefault(e => e.Id == dto.Id);
                    if (existing != null)
                    {
                        // Actualizar SOLO el campo necesario desde el DTO
                        existing.Valor1 = dto.Valor1;

                        // Marcar como modificado
                        _unitOfWork.ConfiguracionesRepository.Update(existing);
                    }
                }

                int changes = await _unitOfWork.CommitAsync();
                return changes > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Dictionary<Guid, ConfigSettingDTO>> GetFullSetting(Guid clientCompanyId)
        {
            var settings = await _unitOfWork.ConfiguracionesRepository
                 .GetAllSettingQueryable()
                 .Where(c => c.EmpresaClienteId == clientCompanyId && c.Estado == 1)
                 .Select(c => new ConfigSettingDTO
                 {
                     TipoConfiguracion = c.TipoConfiguracion,
                     Nombre = c.NombreParametro,
                     Valor1 = c.Valor1
                 })
                 .ToListAsync();

            return settings
                .GroupBy(s => s.TipoConfiguracion)
                .Select(g => g.First())
                .ToDictionary(s => s.TipoConfiguracion);
        }

        public async Task<ResultDTO<AppSettingDTO>> GetAppSettings(Guid clientCompanyId)
        {
            try
            {
                var appSettings = await GetAppSettingsAsync(clientCompanyId);
                return ResultDTO<AppSettingDTO>.Ok(appSettings);
            }
            catch (KeyNotFoundException ex)
            {
                return ResultDTO<AppSettingDTO>.Fail("Falta una configuración requerida.");
            }
            catch (ArgumentException ex)
            {
                return ResultDTO<AppSettingDTO>.Fail("Error en el formato de configuración.");
            }
            catch (Exception ex)
            {
                return ResultDTO<AppSettingDTO>.Fail("Error inesperado al obtener configuración.");
            }
        }

        public async Task<AppSettingDTO> GetAppSettingsAsync(Guid clientCompanyId)
        {
            var settings = await GetFullSetting(clientCompanyId);
            var stringKeySettings = settings.ToDictionary(k => k.Key, v => v.Value.Valor1);

            return new AppSettingDTO
            {
                CustomerId = GetRequiredSetting(stringKeySettings, "742CE98B-684B-4A76-BA0D-CF62621FC3E7", nameof(AppSettingDTO.CustomerId)),
                ClientId = GetRequiredSetting(stringKeySettings, "BB617929-5F49-4FDC-8C28-62435505B600", nameof(AppSettingDTO.ClientId)),
                ClientSecretOrCertificate = GetRequiredSetting(stringKeySettings, "29625587-4A45-495A-B728-203608694C44", nameof(AppSettingDTO.ClientSecretOrCertificate)),

                IdpAuthenticationUrl = GetRequiredSetting(stringKeySettings, "60ADEBFE-01B5-497A-828B-CF3801F37495", nameof(AppSettingDTO.IdpAuthenticationUrl)),
                ApiUrl = GetRequiredSetting(stringKeySettings, "9B02E35B-A069-4BF5-B9CA-337A59455347", nameof(AppSettingDTO.ApiUrl)),
                CallbackAndEventUrl = GetRequiredSetting(stringKeySettings, "82481E61-4BF5-44CE-B222-3283F7BC02F9", nameof(AppSettingDTO.CallbackAndEventUrl)),
                PremiumReportUrl = GetOptionalSetting(stringKeySettings, "84BA81E1-56C0-4BEE-A57F-D05C13BB544A"),
                CredentialManagementURL = GetRequiredSetting(stringKeySettings, "5006A3E3-1E78-4341-9253-C2189A7C8974", nameof(AppSettingDTO.CredentialManagementURL)),
                UsersURL = GetRequiredSetting(stringKeySettings, "5F9327BE-42D6-46B9-BF0E-DB7176371A20", nameof(AppSettingDTO.UsersURL)),
                EventsURL = GetRequiredSetting(stringKeySettings, "9914DCB1-B370-4FC5-8CA3-D5ADD1605AF9", nameof(AppSettingDTO.EventsURL)),
                TransactionURL = GetRequiredSetting(stringKeySettings, "A90006CA-A3E8-4576-A8B0-25B1C5438D55", nameof(AppSettingDTO.TransactionURL)),

                ContentType = GetRequiredSetting(stringKeySettings, "40E1A0B9-9144-490E-BF75-7663F3447118", nameof(AppSettingDTO.ContentType)),
                AcceptType = GetOptionalSetting(stringKeySettings, "4B6BCEFA-20CA-48B9-92FA-5396C7C94202"),
                ApplicationId = GetRequiredSetting(stringKeySettings, "788F90F3-0CE3-4E96-B4BA-38DA1CFE105B", nameof(AppSettingDTO.ApplicationId)),
                ApplicationVersion = GetRequiredSetting(stringKeySettings, "FF5E7D45-FCED-4169-B4EB-BA70B43F7BB6", nameof(AppSettingDTO.ApplicationVersion)),

                PartNumberField = GetRequiredSetting(stringKeySettings, "C98EE139-92FB-4E71-94B7-AE258DD1929A", nameof(AppSettingDTO.PartNumberField)),

                AutoDetectPartNumber = GetOptionalSetting(stringKeySettings, "D539FF01-17F0-4C29-9E17-668A5591ACE5"),
                SelectPartNumber = GetOptionalSetting(stringKeySettings, "18A0E41D-960E-4F52-9604-D0C773A87F9C"),
                ManualEntryPartNumber = GetOptionalSetting(stringKeySettings, "32DC2E87-E6A4-48D7-AF0E-B967ED2BBF49")
            };
        }

        public async Task<EmailSettingDTO> GetEmailSetting(Guid clientCompanyId)
        {
            var settings = await GetFullSetting(clientCompanyId);
            var stringKeySettings = settings.ToDictionary(k => k.Key, v => v.Value.Valor1);

            return new EmailSettingDTO
            {
                SenderName = GetRequiredSetting(stringKeySettings,
                    "55a17813-0f43-4e2b-8318-0d7c57495150",
                    nameof(EmailSettingDTO.SenderName)),

                Subject = GetRequiredSetting(stringKeySettings,
                    "0057d019-b285-44c3-8ad3-8ab314700c6a",
                    nameof(EmailSettingDTO.Subject)),

                SmtpServer = GetRequiredSetting(stringKeySettings,
                    "6ee5652f-6fcd-4159-9799-59f27bd87804",
                    nameof(EmailSettingDTO.SmtpServer)),

                Port = GetRequiredSetting(stringKeySettings,
                    "4ad8fd8a-40fc-4850-a687-dbb441a9ce8d",
                    nameof(EmailSettingDTO.Port)),

                SenderEmail = GetRequiredSetting(stringKeySettings,
                    "80e03d8f-a17e-4f3a-8bc0-61f737a22023",
                    nameof(EmailSettingDTO.SenderEmail)),

                Password = GetRequiredSetting(stringKeySettings,
                    "001425c3-3806-455e-addd-19656e354587",
                    nameof(EmailSettingDTO.Password))
            };
        }

        private string GetRequiredSetting(Dictionary<Guid, string> settings, string guidString, string settingName)
        {
            if (!Guid.TryParse(guidString, out var guid))
                throw new ArgumentException($"GUID inválido para '{settingName}': '{guidString}'");

            if (!settings.TryGetValue(guid, out var value) || string.IsNullOrWhiteSpace(value))
                throw new KeyNotFoundException($"Falta valor requerido para configuración '{settingName}' con GUID '{guidString}'.");

            return value;
        }

        private string? GetOptionalSetting(Dictionary<Guid, string> settings, string guidString)
        {
            if (!Guid.TryParse(guidString, out var guid))
                return null;

            settings.TryGetValue(guid, out var value);
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public async Task<bool> CreateInitialSettingsForCompany(Guid empresaClienteId, Guid currentUserId)
        {
            if (empresaClienteId == Guid.Empty || currentUserId == Guid.Empty)
                throw new ArgumentException("Los identificadores no pueden estar vacíos.");

            // Verifica si ya existen configuraciones para esta empresa
            var exists = await _unitOfWork.ConfiguracionesRepository
                .GetAllSettingQueryable()
                .AnyAsync(c => c.EmpresaClienteId == empresaClienteId && c.Estado == 1);

            if (exists)
                return false;

            var now = DateTime.UtcNow;
            var configTemplates = await GetConfigurationTemplates();

            var configsToCreate = configTemplates.Select(template => new Configuraciones
            {
                Id = Guid.NewGuid(),
                TipoConfiguracion = template.TipoConfiguracion,
                NombreParametro = template.NombreParametro,
                Valor1 = template.Valor1,
                Valor2 = template.Valor2,
                Valor3 = template.Valor3,
                editable = template.editable,
                lectura = template.lectura,
                EmpresaClienteId = empresaClienteId,
                UsuarioCreadorId = currentUserId,
                FechaCreacion = now,
                Estado = 1
            }).ToList();

            await _unitOfWork.ConfiguracionesRepository.AddRangeSetting(configsToCreate);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreateSettingsForCompany(List<ConfiguracionesReqDTO> settings, Guid empresaClienteId, Guid currentUserId)
        {
            if (empresaClienteId == Guid.Empty)
                return false;

            if (currentUserId == Guid.Empty)
                return false;

            if (settings == null || settings.Count == 0)
                return false;

            var now = DateTime.UtcNow;

            var list = settings.Select(c => new Configuraciones
            {
                Id = Guid.NewGuid(),
                TipoConfiguracion = c.TipoConfiguracion,
                NombreParametro = c.NombreParametro,
                Valor1 = c.Valor1,
                Valor2 = c.Valor2,
                Valor3 = c.Valor3,
                editable = c.editable,
                lectura = c.lectura,
                EmpresaClienteId = empresaClienteId,
                UsuarioCreadorId = currentUserId,
                FechaCreacion = now,
                Estado = 1
            }).ToList();

            await _unitOfWork.ConfiguracionesRepository.AddRangeSetting(list);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public Task<List<Configuraciones>> GetConfigurationTemplates()
        {
            return Task.FromResult(new List<Configuraciones>
            {
                // @CN01
                new() { TipoConfiguracion = Guid.Parse("742CE98B-684B-4A76-BA0D-CF62621FC3E7"), NombreParametro = "Customer ID", Valor1 = "", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("BB617929-5F49-4FDC-8C28-62435505B600"), NombreParametro = "Client ID", Valor1 = "", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("29625587-4A45-495A-B728-203608694C44"), NombreParametro = "Client secret/Client certificate", Valor1 = "", editable = 1, lectura = 0 },

                // @CN02
                new() { TipoConfiguracion = Guid.Parse("60ADEBFE-01B5-497A-828B-CF3801F37495"), NombreParametro = "IDP authentication URL", Valor1 = "https://api.cert.origo.hidglobal.com", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("9B02E35B-A069-4BF5-B9CA-337A59455347"), NombreParametro = "API URL", Valor1 = "", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("82481E61-4BF5-44CE-B222-3283F7BC02F9"), NombreParametro = "Callback and Event URL", Valor1 = "", Valor2 = "If callback is implemented", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("84BA81E1-56C0-4BEE-A57F-D05C13BB544A"), NombreParametro = "Premium Report URL", Valor1 = "", Valor2 = "If premium reports API is used", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("5006A3E3-1E78-4341-9253-C2189A7C8974"), NombreParametro = "Credential Management URL", Valor1 = "", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("5F9327BE-42D6-46B9-BF0E-DB7176371A20"), NombreParametro = "Users URL", Valor1 = "", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("9914DCB1-B370-4FC5-8CA3-D5ADD1605AF9"), NombreParametro = "Events URL", Valor1 = "", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("A90006CA-A3E8-4576-A8B0-25B1C5438D55"), NombreParametro = "Transaction URL", Valor1 = "", editable = 1, lectura = 0 },

                // @CN03
                new() { TipoConfiguracion = Guid.Parse("40E1A0B9-9144-490E-BF75-7663F3447118"), NombreParametro = "Content Type", Valor1 = "application/vnd.assaabloy.ma.credential-management-2.2+json", Valor2 = "Header requirement", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("4B6BCEFA-20CA-48B9-92FA-5396C7C94202"), NombreParametro = "Accept Type", Valor1 = "##MANDATORY##", Valor2 = "For .NET compatibility", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("788F90F3-0CE3-4E96-B4BA-38DA1CFE105B"), NombreParametro = "Application ID", Valor1 = "HID-CRCDEMEXICO-DEV", Valor2 = "Format: HID-PARTNERNAME-SOLUTIONNAME", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("FF5E7D45-FCED-4169-B4EB-BA70B43F7BB6"), NombreParametro = "Application Version", Valor1 = "##MANDATORY##", Valor2 = "Versioning format", editable = 1, lectura = 0 },

                // @CN04
                new() { TipoConfiguracion = Guid.Parse("C98EE139-92FB-4E71-94B7-AE258DD1929A"), NombreParametro = "Part number field", Valor1 = "MID-SUB-CRD_FTPN_644745", Valor2 = "Replaces hardcoded value", editable = 1, lectura = 0 },

                // @CN05
                new() { TipoConfiguracion = Guid.Parse("D539FF01-17F0-4C29-9E17-668A5591ACE5"), NombreParametro = "Auto detect Part number", Valor1 = "4924_644745", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("18A0E41D-960E-4F52-9604-D0C773A87F9C"), NombreParametro = "Select Part number", Valor1 = "MID-SUB-CRD_FTPN_644745", editable = 1, lectura = 0 },
                new() { TipoConfiguracion = Guid.Parse("32DC2E87-E6A4-48D7-AF0E-B967ED2BBF49"), NombreParametro = "Manual entry Part number", Valor1 = "Enter value", Valor2 = "HID Origo compatible", editable = 1, lectura = 0 }
            });
        }

        public async Task<bool> DeleteAllSettingsByCompany(Guid empresaClienteId)
        {
            try
            {
                // Obtener todas las configuraciones de la empresa
                var settings = await _unitOfWork.ConfiguracionesRepository
                    .GetAllSettingQueryable()
                    .Where(c => c.EmpresaClienteId == empresaClienteId)
                    .ToListAsync();

                if (settings == null || !settings.Any())
                    return true; // No hay configuraciones para eliminar

                // Eliminar todas las configuraciones encontradas
                _unitOfWork.ConfiguracionesRepository.DeleteRange(settings);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Considera agregar logging del error aquí
                return false;
            }
        }

        public async Task<bool> ReactivateAllSettingsByCompany(Guid clientCompanyId, Guid currentUserId)
        {
            try
            {
                var now = DateTime.UtcNow;

                var settings = await _unitOfWork.ConfiguracionesRepository
                    .GetAllSettingQueryable()
                    .Where(c => c.EmpresaClienteId == clientCompanyId && c.Estado == 1)
                    .ToListAsync();

                if (settings.Count == 0)
                {
                    return true;
                }

                foreach (var setting in settings)
                {
                    setting.Estado = 2; // Inactivo
                    setting.FechaBaja = now;
                    setting.UsuarioBajaId = currentUserId;
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<SettingsGroup>> GetSettingsGroupByCompany()
        {
            try
            {
                var settings = await _unitOfWork.ConfiguracionesRepository.GetSettingGroupByCompany();
                return settings.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeactivateAllSettingByCompany(Guid clientCompanyId, Guid currentUserId)
        {
            try
            {
                var now = DateTime.UtcNow;

                var settings = await _unitOfWork.ConfiguracionesRepository
                    .GetAllSettingQueryable()
                    .Where(c => c.EmpresaClienteId == clientCompanyId && c.Estado == 1)
                    .ToListAsync();

                if (settings.Count == 0)
                {
                    return true;
                }

                foreach (var setting in settings)
                {
                    setting.Estado = 2; // Inactivo
                    setting.FechaBaja = now;
                    setting.UsuarioBajaId = currentUserId;
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<SettingAccountEmail?> GetSettingOfAccountEmail()
        {
            try
            {
                var settings = await _unitOfWork.ConfiguracionesRepository.GetSettingOfAccountEmail();
                if (settings == null)
                {
                    return null;
                }
                return settings;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Models.HID.CredencialHID;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Services.HID
{
    public class WebhookEventService : IWebhookEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHIDService _hIDService;
        private readonly IConfiguracionService _configuracionService;
        private readonly IDipositivosHIDService _dipositivosHIDService;
        private readonly ICredencialHIDService _credencialHIDService;
        private readonly ILicenciaUserHIDService _licenciaUserHIDService;
        public WebhookEventService(
            IUnitOfWork unitOfWork,
            IHIDService hIDService,
            IConfiguracionService configuracionService,
            IDipositivosHIDService dipositivosHIDService,
            ICredencialHIDService credencialHIDService,
            ILicenciaUserHIDService licenciaUserHIDService
            )
        {
            _unitOfWork = unitOfWork;
            _hIDService = hIDService;
            _configuracionService = configuracionService;
            _dipositivosHIDService = dipositivosHIDService;
            _credencialHIDService = credencialHIDService;
            _licenciaUserHIDService = licenciaUserHIDService;
        }

        public async Task<bool> PostWebhookEvents(int userHIDId, string invitationCode, Guid companyId, Guid userCreatorId)
        {
            if (userHIDId <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(invitationCode))
                return false;

            if (companyId == Guid.Empty || userCreatorId == Guid.Empty)
                return false;

            try
            {
                var appSettingsResult = await _configuracionService.GetAppSettings(companyId);
                if (!appSettingsResult.Success || appSettingsResult.Value == null)
                {
                    return false;
                }
                var settingData = appSettingsResult.Value;
                var tokenData = await _hIDService.GetTokenHIDAsync(settingData);

                if (tokenData == null) return false;
                if (tokenData.access_token == null) return false;

                var userHIDData = await _hIDService.GetAllUserAsync(settingData, userHIDId, tokenData.access_token);
                if (userHIDData != null)
                {
                    if (userHIDData.CredentialContainers != null)
                    {
                        if (userHIDData.CredentialContainers.Count() > 0)
                        {
                            var filterCredentialContainers = userHIDData.CredentialContainers.Where(W => W.Status == "ACTIVE").ToList();
                            foreach (var credentialContainerHID in filterCredentialContainers)
                            {
                                var dipositivoHIDRequest = new DipositivosHid()
                                {
                                    UsuarioId = new Guid(userHIDData.ExternalId),
                                    SistemaOperativo = credentialContainerHID.OsVersion,
                                    NombreDispositivo = credentialContainerHID.Model,
                                    CodigoInvitacion = invitationCode,
                                    EndpointId = credentialContainerHID.Id.ToString(),
                                    SdkVersion = credentialContainerHID.ApplicationVersion,
                                    EmpresaClienteId = companyId,
                                    DeviceInfoLastUpdated = DateTime.Now,
                                    Status = 1,
                                    UsuarioCreadorId = userCreatorId
                                };

                                var dispositivoHIDResult = await _dipositivosHIDService.Create(dipositivoHIDRequest, userCreatorId);
                                if (dispositivoHIDResult != null)
                                {
                                    //if (credentialContainerHID.Credentials != null)
                                    //{
                                    //    if (credentialContainerHID.Credentials.Count() > 0)
                                    //    {
                                    //        var filterCredentials = credentialContainerHID.Credentials.Where(W => W.Status == "ISSUED").ToList();
                                    //        var credentialCreationTasks = filterCredentials.Select(async credentialHID =>
                                    //        {
                                    //            var credencialHIDCrear = new CredencialHid
                                    //            {
                                    //                TipoCredencial = credentialHID.CredentialType,
                                    //                DispositivoId = dispositivoHIDResult.Id,
                                    //                Usuarioid = new Guid(userHIDData.ExternalId),
                                    //                CredencialValor = credentialHID.CardNumber,
                                    //                Validity = credentialHID.PartNumber,
                                    //                Status = 1,
                                    //                UsuarioCreadorId = userCreatorId
                                    //            };

                                    //            var response = await _credencialHIDService.CreateCredentialHID(credencialHIDCrear, userCreatorId);

                                    //            if (!(response))
                                    //            {
                                    //            }

                                    //            return response;
                                    //        }).ToList();

                                    //        bool[] creationResults = await Task.WhenAll(credentialCreationTasks);
                                    //        bool allCreationsSuccessful = creationResults.All(result => result);

                                    //        if (allCreationsSuccessful)
                                    //        {
                                    //            var updateResult = await _licenciaUserHIDService.UpdateStatusAsync(new Guid(userHIDData.ExternalId), userHIDData.UserInvitations[0].Status, 3, userCreatorId);

                                    //            if (updateResult)
                                    //            {
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //        }
                                    //    }
                                    //}
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateCredentialHID(int userHIDId, CredencialHIDSeriDTO credential, Guid companyId, Guid userCreatorId)
        {
            var getUser = await _licenciaUserHIDService.GetByUserHIDId(userHIDId);
            if (getUser == null) return false;

            DipositivosHIDQueryFilter filters = new DipositivosHIDQueryFilter
            {
                UsuarioId = getUser.Id,
                PageNumber = 1,
                PageSize = 100
            };

            var getDevices = await _dipositivosHIDService.GetAll(filters, companyId);

            foreach (var item in getDevices)
            {
                try
                {
                    var credencialHIDCrear = new CredencialHid
                    {
                        TipoCredencial = "ICLASSSEOS",
                        DispositivoId = item.Id,
                        Usuarioid = getUser.Id,
                        CredencialValor = credential.CardNumber.ToString(),
                        EmpresaClienteId = companyId,
                        Status = 1,
                        UsuarioCreadorId = userCreatorId
                    };
                    await _credencialHIDService.Create(credencialHIDCrear, userCreatorId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creando credencial para dispositivo {item.Id}: {ex.Message}");
                }
            }

            getUser.InvitacionActividad = "ACKNOWLEDGED";
            getUser.Status = 3;
            getUser.FechaModificacion = DateTime.Now;
            getUser.UsuarioModificadorId = userCreatorId;

            _unitOfWork.LicenciaUserHIDRepository.Update(getUser);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
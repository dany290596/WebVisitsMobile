using System.ComponentModel.DataAnnotations.Schema;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Models.HID.HIDOrigoCallback;
using WebVisitsMobile.Services.Interfaces.HID;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebVisitsMobile.Services.Services.HID
{
    public class HIDOrigoEventService : IHIDOrigoEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDipositivosHIDService _dipositivosHIDService;
        private readonly ICredencialHIDService _credencialHIDService;
        private readonly ILicenciaUserHIDService _licenciaUserHIDService;

        private static readonly object _logLock = new();

        private static bool _logDirReady;

        private static void Log(string message)
        {
            var timestamp = DateTime.Now;
            var line = $"[{timestamp:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            Console.WriteLine(line);
            try
            {
                if (!_logDirReady)
                {
                    Directory.CreateDirectory(@"C:\logs");
                    _logDirReady = true;
                }
                var file = System.IO.Path.Combine(@"C:\logs", $"webvisitsMobilecallbacks_{timestamp:yyyy-MM-dd}.txt");
                lock (_logLock)
                    System.IO.File.AppendAllText(file, line + Environment.NewLine);
            }
            catch { }
        }

        public HIDOrigoEventService(
            IUnitOfWork unitOfWork,
            IDipositivosHIDService dipositivosHIDService,
            ICredencialHIDService credencialHIDService,
            ILicenciaUserHIDService licenciaUserHIDService)
        {
            _unitOfWork = unitOfWork;
            _dipositivosHIDService = dipositivosHIDService;
            _credencialHIDService = credencialHIDService;
            _licenciaUserHIDService = licenciaUserHIDService;
        }

        // ═════════════════════════════════════════════════════════════════════
        // HELPER PRINCIPAL
        // Intenta actualizar el status con cada ID disponible en orden.
        // Soporta IDs tipo GUID y tipo entero (int).
        // Si uno devuelve true (registro encontrado y actualizado) sale inmediato.
        // Si ninguno funciona retorna false.
        //
        // updateGuidFn  → función que recibe un GUID  (ej: UpdateStatus(guid, status))
        // updateIntFn   → función opcional que recibe un int (ej: UpdateStatusByIntId(int, status))
        //                 Solo aplica para Usuarios — credenciales y dispositivos siempre usan GUID
        // ═════════════════════════════════════════════════════════════════════
        private async Task<bool> TryUpdateStatus(
            Func<Guid, Task<bool>> updateGuidFn,
            string methodName,
            Func<int, Task<bool>>? updateIntFn = null,
            params string?[] ids)
        {
            foreach (var id in ids)
            {
                // Validación 1 — nulo o vacío
                if (string.IsNullOrWhiteSpace(id))
                {
                    Log($"[HIDOrigo] {methodName} ⏭️  ID nulo/vacío, saltando...");
                    continue;
                }

                // Validación 2 — es un entero (ej: "165901445")
                if (long.TryParse(id, out long intValue))
                {
                    // Si no hay función para enteros, saltar
                    if (updateIntFn == null)
                    {
                        Log($"[HIDOrigo] {methodName} ⏭️  '{id}' es un entero pero no hay handler para int, saltando...");
                        continue;
                    }

                    Log($"[HIDOrigo] {methodName} → Intentando con int: {intValue}");
                    try
                    {
                        var result = await updateIntFn((int)intValue);
                        if (result)
                        {
                            Log($"[HIDOrigo] {methodName} ✅ Actualizado con int: {intValue}");
                            return true;
                        }
                        Log($"[HIDOrigo] {methodName} ⚠️  No encontrado con int {intValue}, intentando siguiente...");
                    }
                    catch (Exception ex)
                    {
                        Log($"[HIDOrigo] {methodName} ❌ Error con int {intValue}: {ex.Message}");
                    }
                    continue;
                }

                // Validación 3 — intentar parsear como GUID
                if (!Guid.TryParse(id, out Guid guid))
                {
                    Log($"[HIDOrigo] {methodName} ⏭️  '{id}' no es GUID ni entero válido, saltando...");
                    continue;
                }

                // ✅ Es un GUID válido — intentar actualizar
                Log($"[HIDOrigo] {methodName} → Intentando con GUID: {guid}");
                try
                {
                    var result = await updateGuidFn(guid);
                    if (result)
                    {
                        Log($"[HIDOrigo] {methodName} ✅ Actualizado con GUID: {guid}");
                        return true;
                    }
                    Log($"[HIDOrigo] {methodName} ⚠️  No encontrado con GUID {guid}, intentando siguiente...");
                }
                catch (Exception ex)
                {
                    Log($"[HIDOrigo] {methodName} ❌ Error con GUID {guid}: {ex.Message}");
                }
            }

            Log($"[HIDOrigo] {methodName} ❌ No se encontró registro con ningún ID disponible");
            return false;
        }

        // ═════════════════════════════════════════════════════════════════════
        // USUARIOS
        // UserId puede ser int ("165901445") o GUID
        // Por eso se pasa updateIntFn = UpdateStatusByIntId
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Se dispara cuando HID crea un nuevo usuario en la organización.
        /// Status: 20
        /// </summary>
        public async Task<bool> OnUserCreated(UserEventDTO data)
        {
            Log($"[HIDOrigo] OnUserCreated | UserId: {data.UserId} | Id: {data.Id} | ExternalId: {data.ExternalId}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _licenciaUserHIDService.UpdateStatus(guid, 2),
                methodName: nameof(OnUserCreated),
                updateIntFn: intId => _licenciaUserHIDService.UpdateStatusByIntId(intId, 2),
                data.UserId, data.Id, data.ExternalId);
        }

        /// <summary>
        /// Se dispara cuando se modifican los datos de un usuario en HID.
        /// Status: 21
        /// </summary>
        public async Task<bool> OnUserUpdated(UserEventDTO data)
        {
            Log($"[HIDOrigo] OnUserUpdated | UserId: {data.UserId} | Id: {data.Id} | ExternalId: {data.ExternalId}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _licenciaUserHIDService.UpdateStatus(guid, 3),
                methodName: nameof(OnUserUpdated),
                updateIntFn: intId => _licenciaUserHIDService.UpdateStatusByIntId(intId, 3),
                data.UserId, data.Id, data.ExternalId);
        }

        /// <summary>
        /// Se dispara cuando se inicia el proceso de eliminación de un usuario.
        /// Status: 22
        /// </summary>
        public async Task<bool> OnUserDeleteInitiated(UserEventDTO data)
        {
            Log($"[HIDOrigo] OnUserDeleteInitiated | UserId: {data.UserId} | Id: {data.Id} | ExternalId: {data.ExternalId}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _licenciaUserHIDService.UpdateStatus(guid, 7),
                methodName: nameof(OnUserDeleteInitiated),
                updateIntFn: intId => _licenciaUserHIDService.UpdateStatusByIntId(intId, 7),
                data.UserId, data.Id, data.ExternalId);
        }

        /// <summary>
        /// Se dispara cuando un usuario ha sido eliminado definitivamente de HID.
        /// Status: 23
        /// </summary>
        public async Task<bool> OnUserDeleted(UserEventDTO data)
        {
            Log($"[HIDOrigo] OnUserDeleted | UserId: {data.UserId} | Id: {data.Id} | ExternalId: {data.ExternalId}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _licenciaUserHIDService.UpdateStatus(guid, 7),
                methodName: nameof(OnUserDeleted),
                updateIntFn: intId => _licenciaUserHIDService.UpdateStatusByIntId(intId, 7),
                data.UserId, data.Id, data.ExternalId);
        }

        // ═════════════════════════════════════════════════════════════════════
        // CREDENCIALES
        // UserId y CredentialId siempre son GUID en eventos de credenciales
        // Por eso updateIntFn = null (no se necesita)
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Se dispara cuando una credencial ha sido reservada para un usuario (pre-emisión).
        /// Status: 30
        /// </summary>
        public async Task<bool> OnCredentialReserved(CredentialEventDTO data)
        {
            Log($"[HIDOrigo] OnCredentialReserved | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 2),
                methodName: nameof(OnCredentialReserved),
                updateIntFn: null,
                data.UserId, data.Id);
        }

        /// <summary>
        /// Se dispara cuando una credencial ha sido emitida y entregada al dispositivo.
        /// Status: 31
        /// </summary>
        public async Task<bool> OnCredentialIssued(CredentialEventDTO data)
        {
            Log($"[HIDOrigo] OnCredentialIssued | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 3),
                methodName: nameof(OnCredentialIssued),
                updateIntFn: null,
                data.UserId, data.Id);
        }

        /// <summary>
        /// Se dispara mientras se procesa la revocación de una credencial.
        /// Status: 32
        /// </summary>
        public async Task<bool> OnCredentialRevoking(CredentialEventDTO data)
        {
            Log($"[HIDOrigo] OnCredentialRevoking | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 7),
                methodName: nameof(OnCredentialRevoking),
                updateIntFn: null,
                data.UserId, data.Id);
        }

        /// <summary>
        /// Se dispara cuando una credencial ha sido revocada definitivamente.
        /// Status: 33
        /// </summary>
        public async Task<bool> OnCredentialRevoked(CredentialEventDTO data)
        {
            Log($"[HIDOrigo] OnCredentialRevoked | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 7),
                methodName: nameof(OnCredentialRevoked),
                updateIntFn: null,
                data.UserId, data.Id);
        }

        /// <summary>
        /// Se dispara cuando una credencial es desvinculada de un dispositivo sin ser revocada.
        /// Status: 34
        /// </summary>
        public async Task<bool> OnCredentialUnbound(CredentialEventDTO data)
        {
            Log($"[HIDOrigo] OnCredentialUnbound | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 7),
                methodName: nameof(OnCredentialUnbound),
                updateIntFn: null,
                data.UserId, data.Id);
        }

        /// <summary>
        /// Se dispara cuando falla el proceso de creación de una credencial.
        /// Status: 35
        /// </summary>
        public async Task<bool> OnCredentialCreationFailure(CredentialEventDTO data)
        {
            Log($"[HIDOrigo] OnCredentialCreationFailure | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 7),
                methodName: nameof(OnCredentialCreationFailure),
                updateIntFn: null,
                data.UserId, data.Id);
        }

        // ═════════════════════════════════════════════════════════════════════
        // DISPOSITIVOS
        // UserId y ContainerId siempre son GUID en eventos de dispositivos
        // Por eso updateIntFn = null (no se necesita)
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Se dispara cuando un dispositivo queda registrado y activo en HID (CREDENTIALCONTAINER_PERSONALIZED).
        /// Status: 40
        /// </summary>
        public async Task<bool> OnDevicePersonalized(EndpointEventDTO data)
        {
            Log($"[HIDOrigo] OnDevicePersonalized | UserId: {data.UserId} | ContainerId: {data.Id} | Modelo: {data.Model} | Platform: {data.Platform?.Type}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _dipositivosHIDService.UpdateStatus(guid, 2),
                methodName: nameof(OnDevicePersonalized),
                updateIntFn: null,
                data.UserId, data.Id, data.ExternalId);
        }

        /// <summary>
        /// Se dispara cuando un dispositivo queda inactivo en HID (CREDENTIALCONTAINER_INACTIVE).
        /// Status: 41
        /// </summary>
        public async Task<bool> OnDeviceInactive(EndpointEventDTO data)
        {
            Log($"[HIDOrigo] OnDeviceInactive | UserId: {data.UserId} | ContainerId: {data.Id} | Modelo: {data.Model} | Platform: {data.Platform?.Type}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _dipositivosHIDService.UpdateStatus(guid, 41),
                methodName: nameof(OnDeviceInactive),
                updateIntFn: null,
                data.UserId, data.Id, data.ExternalId);
        }

        public async Task<bool> OnUserUpdatedPass(PassEventDTO pass)
        {
            Log($"[HIDOrigo] OnUserUpdatedPass ▶️  UserId: {pass.UserId} | Status: {pass.Status} | DeviceType: {pass.DeviceType}");
            try
            {
                // ── 1. Validar UserId ──────────────────────────────────────────
                if (!Guid.TryParse(pass.UserId, out var userGuid))
                {
                    Log($"[HIDOrigo] OnUserUpdatedPass ❌ UserId inválido o nulo: '{pass.UserId}'");
                    return false;
                }

                var licenseUser = await _unitOfWork.LicenciaUserHIDRepository.GetUserWalletId(userGuid);
                if (licenseUser == null)
                {
                    Log($"[HIDOrigo] OnUserUpdatedPass ❌ No se encontró usuario con WalletId: {userGuid}");
                    return false;
                }
                Log($"[HIDOrigo] OnUserUpdatedPass 👤 Usuario encontrado: {licenseUser.Id} | {licenseUser.Nombre} {licenseUser.Apellidos}");


                // ── 3. Crear credencial ────────────────────────────────────────
                var firstCredential = pass.Credentials?.FirstOrDefault();
                if (firstCredential == null)
                {
                    Log($"[HIDOrigo] OnUserUpdatedPass ⚠️  No hay credenciales en el payload — dispositivo creado sin credencial");
                    return true;
                }

                if (!Guid.TryParse(pass.Id, out var passGuid))
                {
                    Log($"[HIDOrigo] OnUserUpdatedPass ❌ PassId inválido: '{pass.Id}'");
                    return false;
                }

                var credencialHid = new CredencialHid
                {
                    Usuarioid = licenseUser.Id,
                    CredencialValor = firstCredential.CredentialIdentifiers?.CardNumber?.ToString() ?? "",
                    Status = 3,
                    EmpresaClienteId = licenseUser.EmpresaClienteId,
                    ExternalId = passGuid,
                    TipoCredencial = "Wallet Watch"
                };

                var credential = await _credencialHIDService.Create(credencialHid, new Guid("739B4C8F-4DB1-4475-84D4-7644DCE00620"));
                if (credential == null)
                {
                    Log($"[HIDOrigo] OnUserUpdatedPass ❌ Falló la creación de la credencial para usuario: {licenseUser.Id}");
                    return false;
                }
                Log($"[HIDOrigo] OnUserUpdatedPass 🔑 Credencial creada:  CardNumber: {credencialHid.CredencialValor}");
            }
            catch (Exception ex)
            {
                Log($"[HIDOrigo] OnUserUpdatedPass ❌ Exception: {ex.Message}");
                return false;
            }

            return true;
        }



 


        public async Task<bool> OnDeviceCreate(EndpointEventDTO data)
        {
            Log($"[HIDOrigo] OnDeviceCreate ▶️  UserId: {data.UserId} | ContainerId: {data.Id}");
            try
            {
                // ── 1. Validar UserId ──────────────────────────────────────────
                if (!Guid.TryParse(data.UserId, out var userGuid))
                {
                    Log($"[HIDOrigo] OnDeviceCreate ❌ UserId inválido o nulo: '{data.UserId}'");
                    return false;
                }

                var licenseUser = await _unitOfWork.LicenciaUserHIDRepository.GetUserWalletId(userGuid);


                if (licenseUser == null)
                {
                    Log($"[HIDOrigo] OnDeviceCreate ❌ No se encontró usuario con WalletId: {userGuid}");
                    return false;
                }
                Log($"[HIDOrigo] OnDeviceCreate 👤 Usuario encontrado: {licenseUser.Id} | {licenseUser.Nombre} {licenseUser.Apellidos}");


                string dispositivoNombre = "";

                if (data.DeviceProperties?.Model== null)
                {
                    dispositivoNombre = data.DeviceProperties.DeviceType;
                }
                else {
                    dispositivoNombre = data.DeviceProperties?.Model;
                }

                // ── 2. Crear dispositivo ───────────────────────────────────────
                var dipositivosHid = new DipositivosHid
                {
                    UsuarioId = licenseUser.Id,
                    SistemaOperativo = data.DeviceProperties?.Manufacturer,
                    NombreDispositivo = dispositivoNombre,
                    CodigoInvitacion="",
                    EndpointId="",
                    SdkVersion="",
                    DeviceDefault=1,
                    DeviceInfoLastUpdated=DateTime.Now,
                    Status = 3,
                    EmpresaClienteId = licenseUser.EmpresaClienteId,
                };

                var device = await _dipositivosHIDService.Create(dipositivosHid, new Guid("739B4C8F-4DB1-4475-84D4-7644DCE00620"));
                if (device == null)
                {
                    Log($"[HIDOrigo] OnDeviceCreate ❌ Falló la creación del dispositivo para usuario: {licenseUser.Id}");
                    return false;
                }
                Log($"[HIDOrigo] OnDeviceCreate 📱 Dispositivo creado: {device.Id} | Modelo: {dipositivosHid.NombreDispositivo} | SO: {dipositivosHid.SistemaOperativo}");

                // ── 3. Buscar credencial por PassIds ──────────────────────────
                CredencialHid? credencial = null;
                if (data.PassIds != null)
                {
                    foreach (var item in data.PassIds)
                    {
                        if (!Guid.TryParse(item, out var passGuid))
                        {
                            Log($"[HIDOrigo] OnDeviceCreate ⚠️  PassId inválido, saltando: '{item}'");
                            continue;
                        }

                        credencial = await _unitOfWork.CredencialHIDRepository.GetCredentialHIDExternalId(passGuid);
                        if (credencial != null)
                        {
                            Log($"[HIDOrigo] OnDeviceCreate 🔑 Credencial encontrada: {credencial.Id} | PassId: {item}");
                            break;
                        }
                    }
                }

                if (credencial == null)
                {
                    Log($"[HIDOrigo] OnDeviceCreate ⚠️  No se encontró credencial para ningún PassId — dispositivo creado sin vincular credencial");
                    return true;
                }

                // ── 4. Vincular credencial al dispositivo ──────────────────────
                credencial.DispositivoId = device.Id;
                await _credencialHIDService.Update(credencial, new Guid("739B4C8F-4DB1-4475-84D4-7644DCE00620"));
                Log($"[HIDOrigo] OnDeviceCreate ✅ Credencial {credencial.Id} vinculada al dispositivo {device.Id}");


                licenseUser.InvitacionActividad= "ACKNOWLEDGED";
                licenseUser.Status = 3;

                await _licenciaUserHIDService.Update(licenseUser, new Guid("739B4C8F-4DB1-4475-84D4-7644DCE00620"));

                Log($"[HIDOrigo] licenseUser ✅ Actualizar {licenseUser.Id} wallet {licenseUser.UsuarioWalletId}");
            }
            catch (Exception ex)
            {
                Log($"[HIDOrigo] OnDeviceCreate ❌ Exception: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task<bool> ActualizarStatusPass(PassEventDTO data, int Status)
        {

            bool resp = false;
            try
            {

                CredencialHid credencialHid= await _unitOfWork.CredencialHIDRepository.GetCredentialHIDExternalId(new Guid(data.Id));


                credencialHid.Status = Status;

                await _credencialHIDService.Update(credencialHid, new Guid("739B4C8F-4DB1-4475-84D4-7644DCE00620"));


                resp = true;
            }
            catch (Exception ex)
            {

                resp = false;
            }

            return resp;
        }
    }
}



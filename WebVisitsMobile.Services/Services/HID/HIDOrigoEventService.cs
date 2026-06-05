using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Models.HID.HIDOrigoCallback;
using WebVisitsMobile.Services.Interfaces.HID;

namespace WebVisitsMobile.Services.Services.HID
{
    public class HIDOrigoEventService : IHIDOrigoEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDipositivosHIDService _dipositivosHIDService;
        private readonly ICredencialHIDService _credencialHIDService;
        private readonly ILicenciaUserHIDService _licenciaUserHIDService;

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
                    Console.WriteLine($"[HIDOrigo] {methodName} ⏭️  ID nulo/vacío, saltando...");
                    continue;
                }

                // Validación 2 — es un entero (ej: "165901445")
                if (long.TryParse(id, out long intValue))
                {
                    // Si no hay función para enteros, saltar
                    if (updateIntFn == null)
                    {
                        Console.WriteLine($"[HIDOrigo] {methodName} ⏭️  '{id}' es un entero pero no hay handler para int, saltando...");
                        continue;
                    }

                    Console.WriteLine($"[HIDOrigo] {methodName} → Intentando con int: {intValue}");
                    try
                    {
                        var result = await updateIntFn((int)intValue);
                        if (result)
                        {
                            Console.WriteLine($"[HIDOrigo] {methodName} ✅ Actualizado con int: {intValue}");
                            return true;
                        }
                        Console.WriteLine($"[HIDOrigo] {methodName} ⚠️  No encontrado con int {intValue}, intentando siguiente...");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[HIDOrigo] {methodName} ❌ Error con int {intValue}: {ex.Message}");
                    }
                    continue;
                }

                // Validación 3 — intentar parsear como GUID
                if (!Guid.TryParse(id, out Guid guid))
                {
                    Console.WriteLine($"[HIDOrigo] {methodName} ⏭️  '{id}' no es GUID ni entero válido, saltando...");
                    continue;
                }

                // ✅ Es un GUID válido — intentar actualizar
                Console.WriteLine($"[HIDOrigo] {methodName} → Intentando con GUID: {guid}");
                try
                {
                    var result = await updateGuidFn(guid);
                    if (result)
                    {
                        Console.WriteLine($"[HIDOrigo] {methodName} ✅ Actualizado con GUID: {guid}");
                        return true;
                    }
                    Console.WriteLine($"[HIDOrigo] {methodName} ⚠️  No encontrado con GUID {guid}, intentando siguiente...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[HIDOrigo] {methodName} ❌ Error con GUID {guid}: {ex.Message}");
                }
            }

            Console.WriteLine($"[HIDOrigo] {methodName} ❌ No se encontró registro con ningún ID disponible");
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
            Console.WriteLine($"[HIDOrigo] OnUserCreated | UserId: {data.UserId} | Id: {data.Id} | ExternalId: {data.ExternalId}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _licenciaUserHIDService.UpdateStatus(guid, 20),
                methodName: nameof(OnUserCreated),
                updateIntFn: intId => _licenciaUserHIDService.UpdateStatusByIntId(intId, 20),
                data.UserId, data.Id, data.ExternalId);
        }

        /// <summary>
        /// Se dispara cuando se modifican los datos de un usuario en HID.
        /// Status: 21
        /// </summary>
        public async Task<bool> OnUserUpdated(UserEventDTO data)
        {
            Console.WriteLine($"[HIDOrigo] OnUserUpdated | UserId: {data.UserId} | Id: {data.Id} | ExternalId: {data.ExternalId}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _licenciaUserHIDService.UpdateStatus(guid, 21),
                methodName: nameof(OnUserUpdated),
                updateIntFn: intId => _licenciaUserHIDService.UpdateStatusByIntId(intId, 21),
                data.UserId, data.Id, data.ExternalId);
        }

        /// <summary>
        /// Se dispara cuando se inicia el proceso de eliminación de un usuario.
        /// Status: 22
        /// </summary>
        public async Task<bool> OnUserDeleteInitiated(UserEventDTO data)
        {
            Console.WriteLine($"[HIDOrigo] OnUserDeleteInitiated | UserId: {data.UserId} | Id: {data.Id} | ExternalId: {data.ExternalId}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _licenciaUserHIDService.UpdateStatus(guid, 22),
                methodName: nameof(OnUserDeleteInitiated),
                updateIntFn: intId => _licenciaUserHIDService.UpdateStatusByIntId(intId, 22),
                data.UserId, data.Id, data.ExternalId);
        }

        /// <summary>
        /// Se dispara cuando un usuario ha sido eliminado definitivamente de HID.
        /// Status: 23
        /// </summary>
        public async Task<bool> OnUserDeleted(UserEventDTO data)
        {
            Console.WriteLine($"[HIDOrigo] OnUserDeleted | UserId: {data.UserId} | Id: {data.Id} | ExternalId: {data.ExternalId}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _licenciaUserHIDService.UpdateStatus(guid, 23),
                methodName: nameof(OnUserDeleted),
                updateIntFn: intId => _licenciaUserHIDService.UpdateStatusByIntId(intId, 23),
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
            Console.WriteLine($"[HIDOrigo] OnCredentialReserved | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 30),
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
            Console.WriteLine($"[HIDOrigo] OnCredentialIssued | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 31),
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
            Console.WriteLine($"[HIDOrigo] OnCredentialRevoking | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 32),
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
            Console.WriteLine($"[HIDOrigo] OnCredentialRevoked | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 33),
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
            Console.WriteLine($"[HIDOrigo] OnCredentialUnbound | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 34),
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
            Console.WriteLine($"[HIDOrigo] OnCredentialCreationFailure | UserId: {data.UserId} | CredentialId: {data.Id} | CardNumber: {data.CardNumber}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _credencialHIDService.UpdateStatus(guid, 35),
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
            Console.WriteLine($"[HIDOrigo] OnDevicePersonalized | UserId: {data.UserId} | ContainerId: {data.Id} | Modelo: {data.Model} | Platform: {data.Platform?.Type}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _dipositivosHIDService.UpdateStatus(guid, 40),
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
            Console.WriteLine($"[HIDOrigo] OnDeviceInactive | UserId: {data.UserId} | ContainerId: {data.Id} | Modelo: {data.Model} | Platform: {data.Platform?.Type}");
            return await TryUpdateStatus(
                updateGuidFn: guid => _dipositivosHIDService.UpdateStatus(guid, 41),
                methodName: nameof(OnDeviceInactive),
                updateIntFn: null,
                data.UserId, data.Id, data.ExternalId);
        }
    }
}
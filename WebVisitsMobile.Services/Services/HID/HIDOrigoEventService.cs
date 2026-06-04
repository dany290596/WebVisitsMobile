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
            ILicenciaUserHIDService licenciaUserHIDService
            )
        {
            _unitOfWork = unitOfWork;
            _dipositivosHIDService = dipositivosHIDService;
            _credencialHIDService = credencialHIDService;
            _licenciaUserHIDService = licenciaUserHIDService;
        }

        // ═════════════════════════════════════════════════════════════════════
        // USUARIOS
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Se dispara cuando HID crea un nuevo usuario en la organización.
        /// Usar para: sincronizar el alta del usuario en nuestro sistema.
        /// Data disponible: UserId, Firstname, Lastname, Email, OrganizationId
        /// </summary>
        public async Task<bool> OnUserCreatedAsync(UserEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: sincronizar usuario nuevo en nuestro sistema
            // Ejemplo: await _licenciaUserHIDService.SyncNewUserAsync(data.UserId, data.Email);
            if (data.UserId != null)
            {
                return await _licenciaUserHIDService.UpdateStatus(new Guid(data.UserId), 20);
            }

            return false;
        }

        /// <summary>
        /// Se dispara cuando se modifican los datos de un usuario en HID.
        /// Usar para: actualizar nombre, email u otros datos en nuestro sistema.
        /// Data disponible: UserId, Firstname, Lastname, Email, OrganizationId
        /// </summary>
        public async Task<bool> OnUserUpdatedAsync(UserEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: actualizar datos del usuario en nuestro sistema
            // Ejemplo: await _licenciaUserHIDService.UpdateUserDataAsync(data.UserId, data.Firstname, data.Email);
            if (data.UserId != null)
            {
                return await _licenciaUserHIDService.UpdateStatus(new Guid(data.UserId), 21);
            }

            return false;
        }

        /// <summary>
        /// Se dispara cuando se inicia el proceso de eliminación de un usuario.
        /// Usar para: preparar la revocación de passes y credenciales antes de que se complete.
        /// Data disponible: UserId, Firstname, Email, OrganizationId
        /// </summary>
        public async Task<bool> OnUserDeleteInitiatedAsync(UserEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: marcar usuario como "en proceso de eliminación" en nuestro sistema
            // Ejemplo: await _licenciaUserHIDService.MarkPendingDeleteAsync(data.UserId);
            if (data.UserId != null)
            {
                return await _licenciaUserHIDService.UpdateStatus(new Guid(data.UserId), 22);
            }

            return false;
        }

        /// <summary>
        /// Se dispara cuando un usuario ha sido eliminado definitivamente de HID.
        /// Usar para: desactivar o eliminar el usuario en nuestro sistema.
        /// Data disponible: UserId, Firstname, Email, OrganizationId
        /// </summary>
        public async Task<bool> OnUserDeletedAsync(UserEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: eliminar o desactivar usuario en nuestro sistema
            // Ejemplo: await _licenciaUserHIDService.DeactivateUserAsync(data.UserId);
            if (data.UserId != null)
            {
                return await _licenciaUserHIDService.UpdateStatus(new Guid(data.UserId), 23);
            }

            return false;
        }

        // ═════════════════════════════════════════════════════════════════════
        // CREDENCIALES
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Se dispara cuando una credencial ha sido reservada para un usuario (pre-emisión).
        /// Usar para: registrar en nuestro sistema que hay una credencial en proceso.
        /// Data disponible: Id (credentialId), UserId, CardNumber, PartNumber, CredentialTemplateId
        /// </summary>
        public async Task<bool> OnCredentialReservedAsync(CredentialEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: registrar credencial como reservada en nuestro sistema
            // Ejemplo: await _credencialHIDService.RegisterReservedAsync(data.Id, data.UserId, data.CardNumber);
            if (data.UserId != null)
            {
                return await _credencialHIDService.UpdateStatus(new Guid(data.UserId), 30);
            }
            return false;
        }

        /// <summary>
        /// Se dispara cuando una credencial ha sido emitida y entregada al dispositivo.
        /// Usar para: activar la credencial en nuestro sistema — el usuario ya puede acceder.
        /// Data disponible: Id (credentialId), UserId, CardNumber, PartNumber, CredentialTemplateId
        /// </summary>
        public async Task<bool> OnCredentialIssuedAsync(CredentialEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: activar credencial en nuestro sistema
            // Ejemplo: await _credencialHIDService.ActivateAsync(data.Id, data.UserId, data.CardNumber);
            if (data.UserId != null)
            {
                return await _credencialHIDService.UpdateStatus(new Guid(data.UserId), 31);
            }

            return false;
        }

        /// <summary>
        /// Se dispara mientras se procesa la revocación de una credencial (estado intermedio).
        /// Usar para: marcar credencial como "en proceso de revocación" en nuestro sistema.
        /// Data disponible: Id (credentialId), UserId, CardNumber, PartNumber
        /// </summary>
        public async Task<bool> OnCredentialRevokingAsync(CredentialEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: marcar credencial como en proceso de revocación
            // Ejemplo: await _credencialHIDService.MarkRevokingAsync(data.Id);
            if (data.UserId != null)
            {
                return await _credencialHIDService.UpdateStatus(new Guid(data.UserId), 32);
            }

            return false;
        }

        /// <summary>
        /// Se dispara cuando una credencial ha sido revocada definitivamente.
        /// Usar para: revocar la credencial en nuestro sistema — el usuario pierde acceso.
        /// Data disponible: Id (credentialId), UserId, CardNumber, PartNumber
        /// </summary>
        public async Task<bool> OnCredentialRevokedAsync(CredentialEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: revocar credencial en nuestro sistema
            // Ejemplo: await _credencialHIDService.RevokeAsync(data.Id, data.UserId);
            if (data.UserId != null)
            {
                return await _credencialHIDService.UpdateStatus(new Guid(data.UserId), 33);
            }

            return false;
        }

        /// <summary>
        /// Se dispara cuando una credencial es desvinculada de un dispositivo sin ser revocada.
        /// Usar para: actualizar el estado del dispositivo asociado a esa credencial.
        /// Data disponible: Id (credentialId), UserId, CardNumber
        /// </summary>
        public async Task<bool> OnCredentialUnboundAsync(CredentialEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: desvincular credencial del dispositivo en nuestro sistema
            // Ejemplo: await _credencialHIDService.UnbindAsync(data.Id, data.UserId);
            if (data.UserId != null)
            {
                return await _credencialHIDService.UpdateStatus(new Guid(data.UserId), 34);
            }

            return false;
        }

        /// <summary>
        /// Se dispara cuando falla el proceso de creación de una credencial.
        /// Usar para: notificar el fallo al admin o al usuario, reintentar si aplica.
        /// Data disponible: Id (credentialId), UserId, CardNumber, CredentialTemplateId
        /// </summary>
        public async Task<bool> OnCredentialCreationFailureAsync(CredentialEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: notificar fallo de creación de credencial
            // Ejemplo: await _notificationService.NotifyCredentialFailureAsync(data.UserId, data.Id);
            if (data.UserId != null)
            {
                return await _credencialHIDService.UpdateStatus(new Guid(data.UserId), 35);
            }

            return false;
        }

        // ═════════════════════════════════════════════════════════════════════
        // DISPOSITIVOS
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Se dispara cuando un dispositivo queda registrado y activo en HID (CREDENTIALCONTAINER_PERSONALIZED).
        /// Usar para: registrar el dispositivo en nuestro sistema con modelo, SO y credenciales activas.
        /// Data disponible: Id (containerId), UserId, ExternalId, Model, DeviceType, Manufacturer, Platform (GOOGLE_WALLET / APPLE_WALLET)
        /// </summary>
        public async Task<bool> OnDevicePersonalizedAsync(EndpointEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: registrar dispositivo activo en nuestro sistema
            // Ejemplo: await _dipositivosHIDService.RegisterActiveAsync(data.Id, data.UserId, data.Model, data.DeviceType, data.Platform?.Type);
            if (data.UserId != null)
            {
                return await _dipositivosHIDService.UpdateStatus(new Guid(data.UserId), 40);
            }

            return false;
        }

        /// <summary>
        /// Se dispara cuando un dispositivo queda inactivo en HID (CREDENTIALCONTAINER_INACTIVE).
        /// Ocurre cuando se revoca el pass, se elimina el dispositivo, o el usuario borra el pass del wallet.
        /// Usar para: marcar el dispositivo como inactivo en nuestro sistema.
        /// Data disponible: Id (containerId), UserId, ExternalId, Model, DeviceType, Manufacturer, Platform
        /// </summary>
        public async Task<bool> OnDeviceInactiveAsync(EndpointEventDTO data)
        {
            Console.WriteLine(data);

            // TODO: marcar dispositivo como inactivo en nuestro sistema
            // Ejemplo: await _dipositivosHIDService.MarkInactiveAsync(data.Id, data.UserId);
            if (data.UserId != null)
            {
                return await _dipositivosHIDService.UpdateStatus(new Guid(data.UserId), 40);
            }

            return false;
        }
    }
}
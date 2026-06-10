using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Models.HID.DipositivosHID;
using WebVisitsMobile.Models.HID.UserHID;

namespace WebVisitsMobile.Models.HID.CredencialHID
{
    public class CredencialHIDRespDTO : BaseEntityDTO
    {
        public string? TipoCredencial { get; set; }
        public Guid? DispositivoId { get; set; }
        public Guid? Usuarioid { get; set; }
        public string? CredencialValor { get; set; }
        public string? Validity { get; set; }
        public int? Status { get; set; }

        public Guid? ExternalId { get; set; }

        public DipositivosHIDRespDTO? DipositivosHID { get; set; }
        public UserHIDRespDTO? LicenciaHidUser { get; set; }

        /// <summary>
        /// Descripción amigable del estado de la credencial.
        /// Soporta estados legacy y eventos HID Origo.
        /// </summary>
        public string StatusDescripcion => Status switch
        {
            // Legacy
            1 => "Emitida",
            2 => "Activa",
            3 => "Suspendida",
            4 => "Revocada",
            5 => "Expirada",
            6 => "Eliminada",

            // HID Origo
            30 => "Reservada",
            31 => "Emitida",
            32 => "Revocando",
            33 => "Revocada",
            34 => "Desvinculada",
            35 => "Error de creación",

            null => "Sin estado",
            _ => "Desconocido"
        };

        /// <summary>
        /// Color representativo del estado de la credencial.
        /// </summary>
        public string StatusColor => Status switch
        {
            // Legacy
            1 => "#1E90FF", // Azul - Emitida
            2 => "#32CD32", // Verde - Activa
            3 => "#FFA500", // Naranja - Suspendida
            4 => "#FF0000", // Rojo - Revocada
            5 => "#8B4513", // Café - Expirada
            6 => "#696969", // Gris oscuro - Eliminada

            // HID Origo
            30 => "#1E90FF", // Azul - Reservada
            31 => "#32CD32", // Verde - Emitida
            32 => "#FFA500", // Naranja - Revocando
            33 => "#FF0000", // Rojo - Revocada
            34 => "#9370DB", // Morado - Desvinculada
            35 => "#DC143C", // Rojo intenso - Error de creación

            null => "#2F2F2F", // Gris - Sin estado
            _ => "#2F2F2F"     // Gris - Desconocido
        };
    }
}
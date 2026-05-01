using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Models.HID.LicenciaHID;

namespace WebVisitsMobile.Models.HID.UserHID
{
    public class UserHIDRespDTO : BaseEntityDTO
    {
        public Guid? LicenciaId { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public int? UserId { get; set; }
        public string? Site { get; set; }
        public string? Alert { get; set; }
        public int? LicenseCount { get; set; }
        public string? Telefono { get; set; }
        public DateTime? InvitacionFecha { get; set; }
        public DateTime? InvitacionExpirationDate { get; set; }
        public string? InvitacionActividad { get; set; }
        public string? InvitacionDetalle { get; set; }
        public int? InvitacionId { get; set; }
        public int? Status { get; set; }
        public string StatusDescripcion => Status switch
        {
            1 => "Pendiente",
            2 => "En proceso",
            3 => "Completado",
            7 => "Inactivo",
            null => "Sin estado",
            _ => "Estado desconocido"
        };
        public string StatusColor => Status switch
        {
            1 => "#FFA500", // Pendiente - Naranja
            2 => "#1E90FF", // En proceso - Azul
            3 => "#32CD32", // Completado - Verde
            7 => "#FF0000", // Inactivo - Rojo
            null => "#2f2f2f", // Sin estado - Gris claro
            _ => "#2f2f2f" // Estado desconocido - Gris brillante
        };
        public string StatusHIDColor => InvitacionActividad switch
        {
            "PENDING" => "#FFA500",      // Naranja - Pendiente
            "CANCELLED" => "#FF0000",    // Rojo - Cancelado
            "ACKNOWLEDGED" => "#32CD32", // Verde - Reconocido
            "DELETED" => "#FF0000",      // Gris oscuro - Eliminado
            null => "#2f2f2f", // Sin estado - Gris claro
            _ => "#2f2f2f" // Estado desconocido - Gris brillante
        };
        public string DescripcionEstadoInvitacion => InvitacionActividad switch
        {
            "PENDING" => "Pendiente",
            "CANCELLED" => "Cancelado",
            "ACKNOWLEDGED" => "Reconocido",
            "DELETED" => "Eliminado",
            null => "Sin estado",
            _ => "Estado desconocido"
        };
        public string? Apellidos { get; set; }
        public string NombreCompleto
        {
            get
            {
                var nombreLimpio = string.IsNullOrWhiteSpace(Nombre) || Nombre == "undefined" ? "" : Nombre.Trim();
                var apellidosLimpio = string.IsNullOrWhiteSpace(Apellidos) || Apellidos == "undefined" ? "" : Apellidos.Trim();

                if (string.IsNullOrEmpty(nombreLimpio) && string.IsNullOrEmpty(apellidosLimpio))
                {
                    return "N/A";
                }

                return $"{nombreLimpio} {apellidosLimpio}".Trim();
            }
        }
        public virtual LicenciaHIDRespDTO? LicenciaHID { get; set; } = null!;
    }
}
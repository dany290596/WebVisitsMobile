using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Models.HID.UserHID;

namespace WebVisitsMobile.Models.HID.DipositivosHID
{
    public class DipositivosHIDRespDTO : BaseEntityDTO
    {
        public Guid? UsuarioId { get; set; }
        public string? SistemaOperativo { get; set; }
        public string? NombreDispositivo { get; set; }
        public string? CodigoInvitacion { get; set; }
        public string? EndpointId { get; set; }
        public string? SdkVersion { get; set; }
        public DateTime? DeviceInfoLastUpdated { get; set; }
        public byte? DeviceDefault { get; set; }
        public byte? Status { get; set; }

        public UserHIDRespDTO? LicenciaHidUser { get; set; }
    }
}
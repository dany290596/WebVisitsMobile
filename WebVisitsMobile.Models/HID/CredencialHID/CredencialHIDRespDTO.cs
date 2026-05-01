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

        public DipositivosHIDRespDTO? DipositivosHID { get; set; }
        public UserHIDRespDTO? LicenciaUserHID { get; set; }
    }
}
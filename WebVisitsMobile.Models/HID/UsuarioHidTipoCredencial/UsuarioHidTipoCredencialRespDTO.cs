using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Models.HID.TipoCredencial;
using WebVisitsMobile.Models.HID.UserHID;

namespace WebVisitsMobile.Models.HID.UsuarioHidTipoCredencial
{
    public class UsuarioHidTipoCredencialRespDTO : BaseEntityDTO
    {
        public Guid LicenciaHidUserId { get; set; }
        public Guid TipoCredencialId { get; set; }

        public virtual UserHIDRespDTO LicenciaHidUser { get; set; }
        public virtual TipoCredencialRespDTO TipoCredencial { get; set; }
    }
}
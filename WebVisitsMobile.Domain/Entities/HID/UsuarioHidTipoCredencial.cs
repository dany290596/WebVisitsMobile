using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.HID
{
    public class UsuarioHidTipoCredencial : BaseEntity
    {
        public Guid LicenciaHidUserId { get; set; }
        public Guid TipoCredencialId { get; set; }

        public virtual LicenciaHidUser LicenciaHidUser { get; set; }
        public virtual TipoCredencial TipoCredencial { get; set; }
    }
}
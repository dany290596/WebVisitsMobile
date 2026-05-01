using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Administracion.Sesion
{
    public class TipoUsuario : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public byte TieneSesion { get; set; }
    }
}
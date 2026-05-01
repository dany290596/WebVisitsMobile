using WebVisitsMobile.Models.Common;

namespace WebVisitsMobile.Models.Administracion.Sesion.TipoUsuario
{
    public class TipoUsuarioRespDTO : BaseEntityDTO
    {
        public string Nombre { get; set; } = null!;
        public byte TieneSesion { get; set; }
    }
}
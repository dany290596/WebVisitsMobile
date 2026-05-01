using WebVisitsMobile.Models.Administracion.Modulo.Modulo;
using WebVisitsMobile.Models.Common;

namespace WebVisitsMobile.Models.Administracion.Seccion.Seccion
{
    public class SeccionRespDTO : BaseEntityDTO
    {
        public string Nombre { get; set; } = null!;
        public Guid ModuloId { get; set; }
        public byte Orden { get; set; }
        public string Path { get; set; }
        public virtual ModuloRespDTO Modulo { get; set; } = null!;
    }
}
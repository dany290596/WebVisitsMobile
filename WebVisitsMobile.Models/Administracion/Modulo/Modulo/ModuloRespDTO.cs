using WebVisitsMobile.Models.Common;

namespace WebVisitsMobile.Models.Administracion.Modulo.Modulo
{
    public class ModuloRespDTO : BaseEntityDTO
    {
        public string Nombre { get; set; } = null!;
        public Guid AplicacionId { get; set; }
        public byte Orden { get; set; }
        public string? Imagen { get; set; }
    }
}
using WebVisitsMobile.Models.Common;

namespace WebVisitsMobile.Models.Administracion.Perfil.PerfilPermisoSeccion
{
    public class PerfilPermisoSeccionRespDTO : BaseEntityDTO
    {
        public Guid PerfilId { get; set; }
        public Guid SeccionId { get; set; }
        public byte Permiso { get; set; }
        public byte Vence { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }
}
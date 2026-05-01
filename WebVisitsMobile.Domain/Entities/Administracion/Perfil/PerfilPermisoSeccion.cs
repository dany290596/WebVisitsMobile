using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Administracion.Perfil
{
    public class PerfilPermisoSeccion : BaseEntity
    {
        public Guid PerfilId { get; set; }
        public Guid SeccionId { get; set; }
        public byte Permiso { get; set; }
        public byte Vence { get; set; }
        public DateTime? FechaVencimiento { get; set; }


        public virtual Perfil Perfil { get; set; } = null!;
        public virtual Seccion.Seccion Seccion { get; set; } = null!;
    }
}
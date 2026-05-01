using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Administracion.Sesion
{
    public class Sesion : BaseEntity
    {
        public Guid? UsuarioId { get; set; }
        public Guid? PerfilId { get; set; }
        public string? DireccionIp { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public virtual Perfil.Perfil? Perfil { get; set; }
        public virtual Usuario? Usuario { get; set; }
    }
}
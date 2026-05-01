using WebVisitsMobile.Domain.Entities.Administracion.Seccion;

namespace WebVisitsMobile.Domain.Entities.Administracion.Modulo
{
    public class SeccionesPorModulo
    {
        public Guid ModuloId { get; set; }
        public string ModuloNombre { get; set; }
        public string ModuloImagen { get; set; }
        public List<Secciones> Secciones { get; set; } = new();
    }
}
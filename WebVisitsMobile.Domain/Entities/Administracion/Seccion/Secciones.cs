namespace WebVisitsMobile.Domain.Entities.Administracion.Seccion
{
    public class Secciones
    {
        public Guid? PerfilPermisoSeccionId { get; set; }
        public Guid SeccionId { get; set; }
        public string Nombre { get; set; }
        public byte Orden { get; set; }
        public string Path { get; set; }
        public byte? Permiso { get; set; }
        public byte Vence { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }
}
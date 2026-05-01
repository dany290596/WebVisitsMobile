namespace WebVisitsMobile.Models.Administracion.Perfil.PerfilPermisoSeccion
{
    public class PerfilPermisoSeccionReqDTO
    {
        public Guid PerfilId { get; set; }
        public Guid SeccionId { get; set; }
        public byte Permiso { get; set; }
        public byte Vence { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }
}
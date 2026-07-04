namespace WebVisitsMobile.Domain.Entities.Administracion.Perfil
{
    public class PerfilPermisoSeccionInfo
    {
        public string Modulo { get; set; }
        public string Seccion { get; set; }
        public string Path { get; set; }
        public byte Permiso { get; set; }
        public string PermisoDescripcion { get; set; }
    }
}
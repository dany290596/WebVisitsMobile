namespace WebVisitsMobile.Models.Administracion.Seccion.Seccion
{
    public class SeccionReqDTO
    {
        public string Nombre { get; set; } = null!;
        public Guid ModuloId { get; set; }
        public byte Orden { get; set; }
        public string Path { get; set; }
    }
}
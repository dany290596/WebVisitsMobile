namespace WebVisitsMobile.Models.Administracion.Aplicacion.Aplicacion
{
    public class AplicacionReqDTO
    {
        public string Nombre { get; set; } = null!;
        public string? Imagen { get; set; }
        public byte Orden { get; set; }
    }
}
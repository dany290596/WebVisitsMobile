namespace WebVisitsMobile.Models.Administracion.Modulo.Modulo
{
    public class ModuloReqDTO
    {
        public string Nombre { get; set; } = null!;
        public Guid AplicacionId { get; set; }
        public byte Orden { get; set; }
        public string? Imagen { get; set; }
    }
}
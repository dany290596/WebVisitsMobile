namespace WebVisitsMobile.Domain.Entities.Administracion.Sesion
{
    public class NuevaContasena
    {
        public string? Correo { get; set; } = null!;
        public string? Codigo { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
    }
}
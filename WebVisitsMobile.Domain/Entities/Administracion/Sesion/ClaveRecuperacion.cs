namespace WebVisitsMobile.Domain.Entities.Administracion.Sesion
{
    public class ClaveRecuperacion
    {
        public string? Clave { get; set; } = null!;
        public DateTime? FechaVigencia { get; set; }
    }
}
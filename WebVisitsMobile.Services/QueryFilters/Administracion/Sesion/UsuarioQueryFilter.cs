using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Administracion.Sesion
{
    public class UsuarioQueryFilter : BaseQueryFilter
    {
        public string? Correo { get; set; } = null!;
        public string? Contrasena { get; set; } = null!;
        public Guid? PerfilId { get; set; }
        public Guid? EmpresaId { get; set; }
        public Guid? TipoUsuarioId { get; set; }
        public Guid? IdAsociado { get; set; }
        public byte? Vence { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }
}
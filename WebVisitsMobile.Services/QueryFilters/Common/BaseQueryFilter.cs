namespace WebVisitsMobile.Services.QueryFilters.Common
{
    public class BaseQueryFilter
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int DatosCompletos { get; set; } = 0;
        public string? TipoQuery { get; set; }
        public Guid? Id { get; set; }
        public Guid? UsuarioCreadorId { get; set; }
        public Guid? UsuarioModificadorId { get; set; }
        public Guid? UsuarioBajaId { get; set; }
        public Guid? UsuarioReactivadorId { get; set; }
        public DateTime? FechaCreacionDesde { get; set; }
        public DateTime? FechaCreacionHasta { get; set; }
        public DateTime? FechaModificacionDesde { get; set; }
        public DateTime? FechaModificacionHasta { get; set; }
        public DateTime? FechaBajaDesde { get; set; }
        public DateTime? FechaBajaHasta { get; set; }
        public DateTime? FechaReactivacionDesde { get; set; }
        public DateTime? FechaReactivacionHasta { get; set; }
        public byte? Estado { get; set; }
    }
}
using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Configuracion
{
    public class ConfiguracionesQueryFilter : BaseQueryFilter
    {
        public string? NombreParametro { get; set; }
        public Guid? ValorGuid { get; set; }
        public string? Valor1 { get; set; }
        public string? Valor2 { get; set; }
        public string? Valor3 { get; set; }
        public byte? editable { get; set; }
        public byte? lectura { get; set; }
        public Guid? EmpresaClienteId { get; set; }
    }
}
namespace WebVisitsMobile.Models.Configuracion.Configuraciones
{
    public class ConfiguracionesReqDTO
    {
        public Guid? Id { get; set; }
        public string NombreParametro { get; set; }
        public Guid? ValorGuid { get; set; }
        public string? Valor1 { get; set; }
        public string? Valor2 { get; set; }
        public string? Valor3 { get; set; }
        public byte? editable { get; set; }
        public byte? lectura { get; set; }
        public Guid EmpresaClienteId { get; set; }
        public Guid TipoConfiguracion { get; set; }
        public Guid UsuarioCreadorId { get; set; }
    }
}
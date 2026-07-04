namespace WebVisitsMobile.Domain.Entities.Configuracion
{
    public class SettingEncrypted
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string? Valor1 { get; set; }
        public Guid EmpresaClienteId { get; set; }
        public Guid TipoConfiguracion { get; set; }
    }
}
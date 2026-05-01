namespace WebVisitsMobile.Domain.Entities.Configuracion
{
    public class SettingsGroup
    {
        public Guid EmpresaClienteId { get; set; }
        public string EmpresaClienteNombre { get; set; }
        public AppSetting Settings { get; set; }
    }
}
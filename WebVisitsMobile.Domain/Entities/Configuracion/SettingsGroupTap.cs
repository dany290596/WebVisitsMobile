namespace WebVisitsMobile.Domain.Entities.Configuracion
{
    public class SettingsGroupTap
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public List<ConfigSetting> Items { get; set; }
    }

    public class ConfigSetting
    {
        public Guid TipoConfiguracion { get; set; }
        public string Nombre { get; set; }
        public string Valor1 { get; set; }
    }
}
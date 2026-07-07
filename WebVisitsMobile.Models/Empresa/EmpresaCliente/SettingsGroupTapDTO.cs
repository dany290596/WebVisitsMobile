namespace WebVisitsMobile.Models.Empresa.EmpresaCliente
{
    public class SettingsGroupTapDTO
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public List<ConfigSettingDTO> Items { get; set; }
    }

    public class ConfigSettingDTO
    {
        public Guid TipoConfiguracion { get; set; }
        public Guid? ValorGuid { get; set; }
        public string Nombre { get; set; }
        public string Valor1 { get; set; }
    }
}
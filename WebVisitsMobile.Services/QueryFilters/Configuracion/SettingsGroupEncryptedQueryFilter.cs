namespace WebVisitsMobile.Services.QueryFilters.Configuracion
{
    public class SettingsGroupEncryptedQueryFilter
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int DatosCompletos { get; set; } = 0;

        public Guid EmpresaClienteId { get; set; }
        public byte UsaCredencialesHID { get; set; }
        public byte UsaCredencialesWallet { get; set; }
    }
}
using WebVisitsMobile.Services.QueryFilters.Common;

namespace WebVisitsMobile.Services.QueryFilters.Empresa
{
    public class EmpresaClienteQueryFilter : BaseQueryFilter
    {
        public string? RazonSocial { get; set; }
        public string? RFC { get; set; }
        public string? TelefonoEmpresa { get; set; }
        public string? TelefonoMovil { get; set; }
        public string? CorreoElectronico { get; set; }
        public byte? UsaCredencialesHID { get; set; }

        public Guid? PaisId { get; set; }
        public Guid? EstadoId { get; set; }
        public Guid? CiudadId { get; set; }
    }
}
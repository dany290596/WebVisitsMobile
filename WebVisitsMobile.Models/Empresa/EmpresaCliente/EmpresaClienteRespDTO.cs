using WebVisitsMobile.Models.Common;

namespace WebVisitsMobile.Models.Empresa.EmpresaCliente
{
    public class EmpresaClienteRespDTO : BaseEntityDTO
    {
        public string? RazonSocial { get; set; } = null;
        public string? RFC { get; set; } = null;
        public string? TelefonoEmpresa { get; set; } = null;
        public string? TelefonoMovil { get; set; } = null;
        public string? CorreoElectronico { get; set; } = null;
        public Guid? PaisId { get; set; }
        public Guid? EstadoId { get; set; }
        public Guid? CiudadId { get; set; }
        public byte UsaCredencialesHID { get; set; }
        public string? PaisNombre { get; set; } = null;
        public byte PaisEstado { get; set; }
        public string? EstadoNombre { get; set; }
        public string? CiudadNombre { get; set; } = null;
        public byte CiudadEstado { get; set; }
    }
}
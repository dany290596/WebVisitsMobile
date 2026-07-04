using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Models.Ubicacion.Ciudad;
using WebVisitsMobile.Models.Ubicacion.Pais;
using WebVisitsMobile.Models.Ubicacion.PaisEstado;

namespace WebVisitsMobile.Models.Empresa.EmpresaCliente
{
    public class EmpresaClienteRespDTO : BaseEntityDTO
    {
        public string? RazonSocial { get; set; } = null;
        public string? RFC { get; set; } = null;
        public string? TelefonoEmpresa { get; set; } = null;
        public string? TelefonoMovil { get; set; } = null;
        public string? CorreoElectronico { get; set; } = null;

        public byte UsaCredencialesHID { get; set; }
        public byte UsaCredencialesWallet { get; set; }

        public string? PaisNombre { get; set; } = null;
        public string? EstadoNombre { get; set; }
        public string? CiudadNombre { get; set; } = null;
        public byte CiudadEstado { get; set; }

        public Guid? PaisId { get; set; }
        public Guid? EstadoId { get; set; }
        public Guid? CiudadId { get; set; }

        public PaisRespDTO? Pais { get; set; }
        public PaisEstadoRespDTO? PaisEstado { get; set; }
        public CiudadRespDTO? Ciudad { get; set; }
    }
}
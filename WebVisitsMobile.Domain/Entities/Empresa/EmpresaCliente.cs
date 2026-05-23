using System.ComponentModel.DataAnnotations.Schema;
using WebVisitsMobile.Domain.Entities.Common;
using WebVisitsMobile.Domain.Entities.Ubicacion;

namespace WebVisitsMobile.Domain.Entities.Empresa
{
    public class EmpresaCliente : BaseEntity
    {
        public string RazonSocial { get; set; }
        public string RFC { get; set; }
        public string TelefonoEmpresa { get; set; }
        public string? TelefonoMovil { get; set; }
        public string CorreoElectronico { get; set; }
        public byte UsaCredencialesHID { get; set; }

        [ForeignKey("PaisId")]
        public Guid? PaisId { get; set; }

        [ForeignKey("EstadoId")]
        public Guid? EstadoId { get; set; }

        [ForeignKey("CiudadId")]
        public Guid? CiudadId { get; set; }


        [ForeignKey(nameof(PaisId))]
        public Pais? Pais { get; set; }

        [ForeignKey(nameof(EstadoId))]
        public PaisEstado? PaisEstado { get; set; }

        [ForeignKey(nameof(CiudadId))]
        public Ciudad? Ciudad { get; set; }
    }
}
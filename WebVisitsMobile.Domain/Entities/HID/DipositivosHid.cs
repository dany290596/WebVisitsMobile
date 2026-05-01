using System.ComponentModel.DataAnnotations.Schema;
using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.HID
{
    public class DipositivosHid : BaseEntity
    {
        [ForeignKey("LicenciaHidUser")]
        public Guid? UsuarioId { get; set; }
        public string? SistemaOperativo { get; set; }
        public string? NombreDispositivo { get; set; }
        public string? CodigoInvitacion { get; set; }
        public string? EndpointId { get; set; }
        public string? SdkVersion { get; set; }
        public DateTime? DeviceInfoLastUpdated { get; set; }
        public byte? DeviceDefault { get; set; }
        public byte? Status { get; set; }
        public Guid? EmpresaClienteId { get; set; }

        public virtual LicenciaHidUser LicenciaHidUser { get; set; } = null!;
    }
}
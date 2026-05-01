using System.ComponentModel.DataAnnotations.Schema;
using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.HID
{
    public class CredencialHid : BaseEntity
    {
        public string? TipoCredencial { get; set; }
        [ForeignKey("DipositivosHid")]
        public Guid? DispositivoId { get; set; }
        [ForeignKey("LicenciaHidUser")]
        public Guid? Usuarioid { get; set; }
        public string? CredencialValor { get; set; }
        public string? Validity { get; set; }
        public int? Status { get; set; }
        public Guid? EmpresaClienteId { get; set; }

        public virtual DipositivosHid DipositivosHid { get; set; } = null!;
        public virtual LicenciaHidUser LicenciaHidUser { get; set; } = null!;
    }
}
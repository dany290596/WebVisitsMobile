using System.ComponentModel.DataAnnotations.Schema;
using WebVisitsMobile.Domain.Entities.Common;
using WebVisitsMobile.Domain.Entities.Empresa;

namespace WebVisitsMobile.Domain.Entities.Organizacion.Tarea
{
    public class Tarea : BaseEntity
    {
        public Guid TipoTareaId { get; set; }
        public DateTime Fecha { get; set; }
        public byte Pendiente { get; set; }
        public byte Status { get; set; }
        public string? ValorEnvio { get; set; }
        public string? ValorRetorno { get; set; }
        public Guid? ReferenciaId { get; set; }
        public int? Marca { get; set; }
        public Guid? EmpresaClienteId { get; set; }

        [ForeignKey("TipoTareaId")]
        public virtual TipoTarea TipoTarea { get; set; } = null!;

        public virtual EmpresaCliente EmpresaCliente { get; set; } = null!;
    }
}
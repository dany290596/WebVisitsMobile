using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Models.Organizacion.Tarea.TipoTarea;

namespace WebVisitsMobile.Models.Organizacion.Tarea.Tarea
{
    public class TareaRespDTO : BaseEntityDTO
    {
        public Guid TipoTareaId { get; set; }
        public DateTime Fecha { get; set; }
        public int Pendiente { get; set; }
        public byte Status { get; set; }
        public string? ValorEnvio { get; set; }
        public string? ValorRetorno { get; set; }
        public Guid? ReferenciaId { get; set; }
        public int? Marca { get; set; }
        public Guid? EmpresaClienteId { get; set; }

        public virtual TipoTareaRespDTO TipoTarea { get; set; } = null!;
    }
}
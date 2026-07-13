using System.ComponentModel.DataAnnotations.Schema;
using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Domain.Entities.Empresa
{
    public class Sucursal : BaseEntity
    {
        public string Nombre { get; set; }
        public string RFC { get; set; }

        [ForeignKey("EmpresaCliente")]
        public Guid? EmpresaClienteId { get; set; }
        public virtual EmpresaCliente? EmpresaCliente { get; set; }
    }
}

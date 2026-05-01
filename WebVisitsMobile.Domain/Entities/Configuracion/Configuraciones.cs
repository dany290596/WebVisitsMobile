using System.ComponentModel.DataAnnotations.Schema;
using WebVisitsMobile.Domain.Entities.Common;
using WebVisitsMobile.Domain.Entities.Empresa;

namespace WebVisitsMobile.Domain.Entities.Configuracion
{
    public class Configuraciones : BaseEntity
    {
        public string NombreParametro { get; set; }
        public Guid? ValorGuid { get; set; }
        public string? Valor1 { get; set; }
        public string? Valor2 { get; set; }
        public string? Valor3 { get; set; }
        public byte? editable { get; set; }
        public byte? lectura { get; set; }
        [ForeignKey("EmpresaCliente")]
        public Guid EmpresaClienteId { get; set; }
        public Guid TipoConfiguracion { get; set; }
        public virtual EmpresaCliente EmpresaCliente { get; set; } = null!;
    }
}
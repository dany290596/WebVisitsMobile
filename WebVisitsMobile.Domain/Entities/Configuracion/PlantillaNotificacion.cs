using System.ComponentModel.DataAnnotations.Schema;
using WebVisitsMobile.Domain.Entities.Common;
using WebVisitsMobile.Domain.Entities.Empresa;

namespace WebVisitsMobile.Domain.Entities.Configuracion
{
    public class PlantillaNotificacion : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string? CuerpoPlantilla { get; set; }
        public byte? NotificarEmail { get; set; }
        public byte? NotificarTeams { get; set; }
        public Guid? Identificador { get; set; }

        [ForeignKey("TipoPlantillaNotificacion")]
        public Guid TipoPlantillaNotificacionId { get; set; }

        [ForeignKey("EmpresaCliente")]
        public Guid EmpresaClienteId { get; set; }

        public virtual TipoPlantillaNotificacion TipoPlantillaNotificacion { get; set; } = null!;
        public virtual EmpresaCliente EmpresaCliente { get; set; } = null!;
    }
}
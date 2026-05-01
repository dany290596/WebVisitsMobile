using System.ComponentModel.DataAnnotations.Schema;
using WebVisitsMobile.Domain.Entities.Common;
using WebVisitsMobile.Domain.Entities.Empresa;

namespace WebVisitsMobile.Domain.Entities.Administracion.Sesion
{
    public class Usuario : BaseEntity
    {
        public string Correo { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
        public Guid IdAsociado { get; set; }
        public byte Vence { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        [ForeignKey(nameof(PerfilId))]
        public Guid PerfilId { get; set; }
        [ForeignKey(nameof(TipoUsuarioId))]
        public Guid TipoUsuarioId { get; set; }
        [ForeignKey(nameof(EmpresaClienteId))]
        public Guid? EmpresaClienteId { get; set; }

        public virtual Perfil.Perfil Perfil { get; set; } = null!;
        public virtual TipoUsuario TipoUsuario { get; set; } = null!;
        public virtual EmpresaCliente? EmpresaCliente { get; set; }
    }
}
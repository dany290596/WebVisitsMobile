using WebVisitsMobile.Models.Administracion.Perfil.Perfil;
using WebVisitsMobile.Models.Administracion.Sesion.TipoUsuario;
using WebVisitsMobile.Models.Common;

namespace WebVisitsMobile.Models.Administracion.Sesion.Usuario
{
    public class UsuarioRespDTO : BaseEntityDTO
    {
        public string Correo { get; set; } = null!;
        public string PerfilNombre { get; set; }
        public byte PerfilEstado { get; set; }
        public string TipoUsuarioNombre { get; set; }
        public byte TipoUsuarioEstado { get; set; }
        public Guid IdAsociado { get; set; }
        public byte Vence { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public Guid PerfilId { get; set; }
        public Guid TipoUsuarioId { get; set; }
        public Guid? EmpresaClienteId { get; set; }
        public string? Clave { get; set; } = null!;

        public virtual PerfilRespDTO Perfil { get; set; } = null!;
        public virtual TipoUsuarioRespDTO TipoUsuario { get; set; } = null!;
    }
}
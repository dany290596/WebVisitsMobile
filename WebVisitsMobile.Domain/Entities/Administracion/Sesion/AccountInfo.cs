using WebVisitsMobile.Domain.Entities.Administracion.Perfil;
using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Domain.Entities.Administracion.Sesion
{
    public class AccountInfo
    {
        public Guid UsuarioId { get; set; }
        public string Correo { get; set; }
        public string Perfil { get; set; }
        public string TipoUsuario { get; set; }
        public byte? Estado { get; set; }

        // Empresa directa del usuario (si tiene)
        public EmpresaCliente? EmpresaCliente { get; set; }

        // Licencias HID asignadas al usuario
        public LicenciaHID Licencia { get; set; } = new();

        public List<PerfilPermisoSeccionInfo> Permisos { get; set; } = new();
    }
}
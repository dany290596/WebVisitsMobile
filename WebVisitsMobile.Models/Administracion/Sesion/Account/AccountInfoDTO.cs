using WebVisitsMobile.Models.Empresa.EmpresaCliente;
using WebVisitsMobile.Models.HID.LicenciaHID;

namespace WebVisitsMobile.Models.Administracion.Sesion.Account
{
    public class AccountInfoDTO
    {
        public Guid UsuarioId { get; set; }
        public string Correo { get; set; }
        public string Perfil { get; set; }
        public string TipoUsuario { get; set; }
        public int Estado { get; set; }

        // Empresa directa del usuario (si tiene)
        public EmpresaClienteRespDTO? EmpresaCliente { get; set; }

        // Licencias HID asignadas al usuario
        public LicenciaHIDRespDTO Licencia { get; set; } = new();
    }
}
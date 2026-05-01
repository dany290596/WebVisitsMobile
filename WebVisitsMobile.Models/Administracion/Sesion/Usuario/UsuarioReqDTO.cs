namespace WebVisitsMobile.Models.Administracion.Sesion.Usuario
{
    public class UsuarioReqDTO
    {
        public string Correo { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
        //public Guid IdAsociado { get; set; }
        public byte Vence { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public Guid PerfilId { get; set; }
        public Guid TipoUsuarioId { get; set; }
        public Guid? EmpresaClienteId { get; set; }
    }
}
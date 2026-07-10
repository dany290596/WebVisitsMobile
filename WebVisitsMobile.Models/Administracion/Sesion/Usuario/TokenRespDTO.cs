namespace WebVisitsMobile.Models.Administracion.Sesion.Usuario
{
    public class TokenRespDTO
    {
        public Guid UsuarioId { get; set; }
        public string Email { get; set; }
        public Guid PerfilId { get; set; }
        public string PerfilName { get; set; }
        public Guid EmpresaId { get; set; }
        public Guid SesionId { get; set; }
        public Guid AsociadoId { get; set; }
        public Guid TipoUsuarioId { get; set; }
        public string TipoUsuarioName { get; set; }
    }
}
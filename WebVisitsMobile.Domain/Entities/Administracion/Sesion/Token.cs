namespace WebVisitsMobile.Domain.Entities.Administracion.Sesion
{
    public class Token
    {
        public Guid UsuarioId { get; set; }
        public string Email { get; set; }
        public Guid PerfilId { get; set; }
        public Guid EmpresaId { get; set; }
        public Guid SesionId { get; set; }
        public Guid AsociadoId { get; set; }
        public Guid TipoUsuarioId { get; set; }
    }
}
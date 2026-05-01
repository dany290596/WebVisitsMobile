namespace WebVisitsMobile.Models.Administracion.Sesion.TipoUsuario
{
    public class TipoUsuarioReqDTO
    {
        public string Nombre { get; set; } = null!;
        public byte TieneSesion { get; set; }
    }
}
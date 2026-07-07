namespace WebVisitsMobile.Models.Configuracion.CorreoEmpresa
{
    public class CorreoEmpresaSmtpDTO
    {
        public string? Correo { get; set; }
        public string? Servidor { get; set; }
        public int? Puerto { get; set; }
        public string? Usuario { get; set; }
        public string? Password { get; set; }
        public bool? Ssl { get; set; }
        public bool? Tls12 { get; set; }
        public bool? Tls13 { get; set; }
    }
}

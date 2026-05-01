namespace WebVisitsMobile.Models.Configuracion.Configuraciones
{
    public class EmailSettingDTO
    {
        public string SenderName { get; set; }
        public string Subject { get; set; }
        public string SmtpServer { get; set; }
        public string Port { get; set; }
        public string SenderEmail { get; set; }
        public string Password { get; set; }
    }
}
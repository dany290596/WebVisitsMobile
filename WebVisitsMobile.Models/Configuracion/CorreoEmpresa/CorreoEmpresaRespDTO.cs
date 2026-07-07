namespace WebVisitsMobile.Models.Configuracion.CorreoEmpresa
{
    public class CorreoEmpresaRespDTO
    {
        public Guid EmpresaId { get; set; }
        public string TipoAutenticacion { get; set; }
        public CorreoEmpresaSmtpDTO Smtp { get; set; } = new CorreoEmpresaSmtpDTO();
        public CorreoEmpresaOAuthDTO OAuth { get; set; } = new CorreoEmpresaOAuthDTO();
    }
}

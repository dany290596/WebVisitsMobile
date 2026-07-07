using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Models.Configuracion.CorreoEmpresa
{
    public class CorreoEmpresaReqDTO
    {
        [Required]
        public Guid EmpresaId { get; set; }

        [Required]
        public string TipoAutenticacion { get; set; }

        public CorreoEmpresaSmtpDTO? Smtp { get; set; }
        public CorreoEmpresaOAuthDTO? OAuth { get; set; }
    }
}

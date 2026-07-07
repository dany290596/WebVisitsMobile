using System.ComponentModel.DataAnnotations;

namespace WebVisitsMobile.Models.Configuracion.CorreoEmpresa
{
    public class CorreoEmpresaUpdateReqDTO
    {
        [Required]
        public Guid EmpresaId { get; set; }

        public string? TipoAutenticacion { get; set; }

        public CorreoEmpresaSmtpDTO? Smtp { get; set; }
        public CorreoEmpresaOAuthDTO? OAuth { get; set; }
    }
}

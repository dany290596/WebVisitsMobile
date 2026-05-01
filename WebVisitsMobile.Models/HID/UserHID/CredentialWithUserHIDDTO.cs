namespace WebVisitsMobile.Models.HID.UserHID
{
    public class CredentialWithUserHIDDTO
    {
        public Guid Id { get; set; }
        public string? TipoCredencial { get; set; }
        public Guid? DispositivoId { get; set; }
        public Guid? Usuarioid { get; set; }
        public string? CredencialValor { get; set; }
        public string? Validity { get; set; }
        public int? Status { get; set; }
        public Guid? EmpresaClienteId { get; set; }
    }
}
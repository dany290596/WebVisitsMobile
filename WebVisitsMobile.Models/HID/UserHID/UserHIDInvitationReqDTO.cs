namespace WebVisitsMobile.Models.HID.UserHID
{
    public class UserHIDInvitationReqDTO
    {
        public Guid? UsuarioWalletId { get; set; }
        public DateTime? InvitacionFecha { get; set; }
        public DateTime? InvitacionExpirationDate { get; set; }
        public string? InvitacionDetalle { get; set; }
        public int? Status { get; set; }
    }
}
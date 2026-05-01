namespace WebVisitsMobile.Models.HID.UserHID
{
    public class CodigoInvitacionHIDDTO
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string InvitacionDetalle { get; set; }
        public int InvitacionId { get; set; }
    }
}
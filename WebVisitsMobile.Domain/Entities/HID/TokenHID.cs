namespace WebVisitsMobile.Domain.Entities.HID
{
    public class TokenHID
    {
        public string access_token { get; set; }
        public string id_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
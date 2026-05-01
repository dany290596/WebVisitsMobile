namespace WebVisitsMobile.Models.HID.UserHID
{
    public class CodigoInvitacionEmailHIDDTO
    {
        public bool IsValid { get; init; }
        public string Message { get; init; }
        public string ErrorCode { get; init; }
        public DateTime? ExpirationDate { get; init; }
        public string Status { get; init; }

        public static CodigoInvitacionEmailHIDDTO Valid(DateTime expirationDate, string status)
        {
            var daysRemaining = (expirationDate.Date - DateTime.UtcNow.Date).Days;

            string message = daysRemaining switch
            {
                > 1 => $"El código de invitación es válido y permanecerá activo durante {daysRemaining} días, hasta el {expirationDate:dd/MM/yyyy}.",
                1 => $"El código de invitación es válido y expirará mañana ({expirationDate:dd/MM/yyyy}).",
                0 => $"El código de invitación es válido hasta hoy ({expirationDate:dd/MM/yyyy}).",
                _ => $"El código de invitación es válido hasta el {expirationDate:dd/MM/yyyy}."
            };

            return new CodigoInvitacionEmailHIDDTO
            {
                IsValid = true,
                Message = message,
                ExpirationDate = expirationDate,
                Status = status
            };
        }

        public static CodigoInvitacionEmailHIDDTO Invalid(string message, string errorCode, string status)
        {
            return new CodigoInvitacionEmailHIDDTO
            {
                IsValid = false,
                Message = FormatearMensaje(message),
                ErrorCode = errorCode,
                Status = status
            };
        }

        private static string FormatearMensaje(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return message;

            var trimmed = message.Trim();
            var capitalized = char.ToUpper(trimmed[0]) + trimmed.Substring(1);

            return capitalized.EndsWith(".") ? capitalized : capitalized + ".";
        }
    }
}
namespace WebVisitsMobile.Domain.Entities.HID
{
    public class UserHIDExpired
    {
        public Guid Id { get; set; }
        public int? UserId { get; set; }
        public string Email { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public bool EstaCaducada { get; set; }
        public int DiasCaducados { get; set; }      // Solo si ya caducó
        public int DiasRestantes { get; set; }      // Solo si aún no caduca
        public string EstadoLicencia { get; set; } = "Desconocido";
    }
}
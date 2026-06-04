namespace WebVisitsMobile.Models.HID.UserHID
{
    /// <summary>
    /// Proyección reducida de LicenciaHidUser para el endpoint de licencias expiradas con plataforma asignada.
    /// </summary>
    public class LicenciaHidUserExpiradaRespDTO
    {
        public Guid Id { get; set; }
        public Guid? EmpresaClienteId { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Plataforma { get; set; }
        public Guid? ExternalId { get; set; }
        public Guid? UsuarioWalletId { get; set; }

    }
}

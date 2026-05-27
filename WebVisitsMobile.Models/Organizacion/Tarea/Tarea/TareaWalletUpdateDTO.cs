using System.Text.Json.Serialization;

namespace WebVisitsMobile.Models.Organizacion.Tarea.Tarea
{
    public class TareaWalletUpdateDTO
    {
        [JsonPropertyName("correoElectronico")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [JsonPropertyName("fechaInicio")]
        public DateTime? FechaInicio { get; set; }

        [JsonPropertyName("fechaFin")]
        public DateTime? FechaFin { get; set; }

        /// <summary>1 = Apple, 2 = Android</summary>
        [JsonPropertyName("plataforma")]
        public int? Plataforma { get; set; }

        [JsonPropertyName("nombreCompleto")]
        public string NombreCompleto { get; set; } = string.Empty;

        [JsonPropertyName("codigoInvitacion")]
        public string? CodigoInvitacion { get; set; }
    }
}

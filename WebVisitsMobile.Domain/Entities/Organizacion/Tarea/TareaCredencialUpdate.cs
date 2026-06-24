using System.Text.Json.Serialization;

namespace WebVisitsMobile.Domain.Entities.Organizacion.Tarea
{
    public class TareaCredencialUpdate
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; set; }

        [JsonPropertyName("DisplayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("ExternalId")]
        public Guid? ExternalId { get; set; }

        [JsonPropertyName("UsuarioWalletId")]
        public Guid? UsuarioWalletId { get; set; }


        [JsonPropertyName("Emails")]
        public string? Emails { get; set; }

        [JsonPropertyName("Telefono")]
        public string? Telefono { get; set; }

        [JsonPropertyName("FechaInicio")]
        public DateTime? FechaInicio { get; set; }

        [JsonPropertyName("FechaFin")]
        public DateTime? FechaFin { get; set; }

        [JsonPropertyName("Plataforma")]
        public int? Plataforma { get; set; }

        [JsonPropertyName("Plantilla")]
        public TareaPlantillaCredencial? Plantilla { get; set; }
    }
}

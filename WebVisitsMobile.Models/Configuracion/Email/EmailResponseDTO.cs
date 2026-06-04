namespace WebVisitsMobile.Models.Configuracion.Email
{
    public class EmailResponseDTO
    {
        /// <summary>ID del registro en Configuraciones.</summary>
        public Guid Id { get; set; }

        /// <summary>HTML de la plantilla con placeholders sin reemplazar.</summary>
        public string TemplateHtml { get; set; } = string.Empty;

        /// <summary>Fecha de la última actualización.</summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
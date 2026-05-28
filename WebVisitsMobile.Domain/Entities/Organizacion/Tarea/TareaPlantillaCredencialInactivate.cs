namespace WebVisitsMobile.Domain.Entities.Organizacion.Tarea
{
    /// <summary>
    /// Proyección de respuesta para las tareas del tipo
    /// "PlantillaCredencial Inactivate" (TipoTareaId = C8FC0425-E7C9-4CBD-9CD9-081FB72F549F).
    /// ValorEnvio almacena el ExternalId de la PlantillaCredencial inactivada.
    /// </summary>
    public class TareaPlantillaCredencialInactivate
    {
        public Guid        Id           { get; set; }
        public Guid        TipoTareaId  { get; set; }
        public string?       ValorEnvio   { get; set; }   // ExternalId de PlantillaCredencial
        public byte        Status       { get; set; }
        public int         Pendiente    { get; set; }
        public int Marca { get; set; }
        public Guid EmpresaClienteId { get; set; }

        public DateTime    FechaCreacion{ get; set; }
    }
}

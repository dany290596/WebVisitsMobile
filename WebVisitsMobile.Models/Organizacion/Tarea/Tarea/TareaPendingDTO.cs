namespace WebVisitsMobile.Models.Organizacion.Tarea.Tarea
{
    public class TareaPendingDTO
    {
        public byte Pendiente { get; set; }
        public string? ValorRetorno { get; set; }
        public int? Marca { get; set; }
        public Guid UsuarioModificadorId { get; set; }
    }
}
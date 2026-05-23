namespace WebVisitsMobile.Domain.Entities.Organizacion.Tarea
{
    public class TareaHID<T>
    {
        public Guid Id { get; set; }
        public Guid TipoTareaId { get; set; }
        public DateTime Fecha { get; set; }
        public int Pendiente { get; set; }
        public byte Status { get; set; }
        public string? ValorRetorno { get; set; }
        public Guid? ReferenciaId { get; set; }
        public int? Marca { get; set; }
        public Guid? EmpresaClienteId { get; set; }

        public T ValorEnvio { get; set; }
    }
}
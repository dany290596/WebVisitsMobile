namespace WebVisitsMobile.Models.Common
{
    public class BaseEntityDTO
    {
        public Guid Id { get; set; }
        public Guid? UsuarioCreadorId { get; set; }
        public Guid? UsuarioModificadorId { get; set; }
        public Guid? UsuarioBajaId { get; set; }
        public Guid? UsuarioReactivadorId { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime? FechaBaja { get; set; }
        public DateTime? FechaReactivacion { get; set; }
        public byte? Estado { get; set; }

        public string EstadoDescripcion => Estado switch
        {
            1 => "Activo",
            2 => "Inactivo",
            null => "Sin estado",
            _ => "Desconocido"
        };

        public string EstadoColor => Estado switch
        {
            1 => "#32CD32", // Verde - Activo
            2 => "#FF0000", // Rojo - Inactivo
            null => "#2f2f2f", // Gris - Sin estado
            _ => "#2f2f2f"     // Gris - Desconocido
        };
    }
}
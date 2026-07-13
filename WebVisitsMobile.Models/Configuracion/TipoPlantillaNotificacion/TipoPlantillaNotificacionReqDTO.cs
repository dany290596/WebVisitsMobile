namespace WebVisitsMobile.Models.Configuracion.TipoPlantillaNotificacion
{
    public class TipoPlantillaNotificacionReqDTO
    {
        public string Nombre { get; set; } = null!;
        public Guid EmpresaClienteId { get; set; }
    }
}
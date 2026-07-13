namespace WebVisitsMobile.Models.Empresa.Sucursal
{
    public class SucursalReqDTO
    {
        public string Nombre { get; set; }
        public string RFC { get; set; }
        public Guid? EmpresaClienteId { get; set; }
    }
}

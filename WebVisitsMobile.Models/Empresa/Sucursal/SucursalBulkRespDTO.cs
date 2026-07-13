namespace WebVisitsMobile.Models.Empresa.Sucursal
{
    public class SucursalBulkRespDTO
    {
        public int TotalRecibidos { get; set; }
        public int TotalCreados { get; set; }
        public int TotalOmitidos { get; set; }
        public List<Guid> IdsOmitidos { get; set; } = new List<Guid>();
        public List<SucursalRespDTO> Creados { get; set; } = new List<SucursalRespDTO>();
    }
}

using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Services.QueryFilters.Empresa;

namespace WebVisitsMobile.Services.Interfaces.Empresa
{
    public interface ISucursalService
    {
        Task<Sucursal?> Create(Sucursal data, Guid currentUserId);
        Task<(List<Sucursal> Creados, List<Guid> IdsOmitidos)> CreateBulk(List<Sucursal> data, Guid currentUserId);
        Task<bool> Update(Sucursal data, Guid currentUserId);
        Task<Sucursal?> VincularEmpresaCliente(Guid sucursalId, Guid empresaClienteId, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<PagedList<Sucursal>> GetAll(SucursalQueryFilter filters);
        Task<Sucursal?> GetById(Guid id);
    }
}

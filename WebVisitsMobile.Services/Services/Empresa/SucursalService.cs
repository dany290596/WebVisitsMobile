using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Empresa;
using WebVisitsMobile.Services.QueryFilters.Empresa;

namespace WebVisitsMobile.Services.Services.Empresa
{
    public class SucursalService : ISucursalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;

        public SucursalService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<PagedList<Sucursal>> GetAll(SucursalQueryFilter filters)
        {
            PagedList<Sucursal> pagedData = null;

            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<Sucursal> data = _unitOfWork.SucursalRepository.GetAll();

                if (!string.IsNullOrWhiteSpace(filters.Nombre)) { data = data.Where(x => x.Nombre.ToLower().Contains(filters.Nombre.ToLower())); }
                if (!string.IsNullOrWhiteSpace(filters.RFC)) { data = data.Where(x => x.RFC.ToLower().Contains(filters.RFC.ToLower())); }
                if (filters.EmpresaClienteId != null && filters.EmpresaClienteId != Guid.Empty) { data = data.Where(x => x.EmpresaClienteId == filters.EmpresaClienteId); }

                if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { data = data.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
                if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { data = data.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
                if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { data = data.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
                if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { data = data.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
                if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
                if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
                if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
                if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
                if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
                if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
                if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
                if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
                if (filters.Estado != null && filters.Estado > 0) { data = data.Where(x => x.Estado == filters.Estado); }

                data = data.OrderBy(x => x.Nombre);
                pagedData = PagedList<Sucursal>.Create(data, filters.PageNumber, filters.PageSize);
            }
            catch (Exception)
            {
                return null;
            }

            return pagedData;
        }

        public async Task<Sucursal?> GetById(Guid id)
        {
            try
            {
                return await _unitOfWork.SucursalRepository.GetById(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Sucursal?> Create(Sucursal data, Guid currentUserId)
        {
            try
            {
                data.Id = Guid.NewGuid();
                data.UsuarioCreadorId = currentUserId;
                data.FechaCreacion = DateTime.Now;
                data.Estado = 1;

                await _unitOfWork.SucursalRepository.Add(data);
                await _unitOfWork.SaveChangesAsync();

                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<(List<Sucursal> Creados, List<Guid> IdsOmitidos)> CreateBulk(List<Sucursal> data, Guid currentUserId)
        {
            var creados = new List<Sucursal>();
            var idsOmitidos = new List<Guid>();

            if (data == null || data.Count == 0)
                return (creados, idsOmitidos);

            var idsSolicitados = data.Select(x => x.Id).ToList();
            var idsExistentes = _unitOfWork.SucursalRepository.GetAll()
                .Where(x => idsSolicitados.Contains(x.Id))
                .Select(x => x.Id)
                .ToHashSet();

            var now = DateTime.Now;
            var vistos = new HashSet<Guid>();

            foreach (var item in data)
            {
                if (item.Id == Guid.Empty || idsExistentes.Contains(item.Id) || !vistos.Add(item.Id))
                {
                    idsOmitidos.Add(item.Id);
                    continue;
                }

                item.UsuarioCreadorId = currentUserId;
                item.FechaCreacion = now;
                item.Estado = 1;

                await _unitOfWork.SucursalRepository.Add(item);
                creados.Add(item);
            }

            if (creados.Any())
                await _unitOfWork.SaveChangesAsync();

            return (creados, idsOmitidos);
        }

        public async Task<bool> Update(Sucursal data, Guid currentUserId)
        {
            try
            {
                if (currentUserId == Guid.Empty) { return false; }

                Sucursal dataUpdate = await _unitOfWork.SucursalRepository.GetById(data.Id);
                if (dataUpdate == null) { return false; }

                dataUpdate.Nombre = data.Nombre;
                dataUpdate.RFC = data.RFC;
                dataUpdate.EmpresaClienteId = data.EmpresaClienteId;

                dataUpdate.FechaModificacion = DateTime.Now;
                dataUpdate.UsuarioModificadorId = currentUserId;

                _unitOfWork.SucursalRepository.Update(dataUpdate);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Sucursal?> VincularEmpresaCliente(Guid sucursalId, Guid empresaClienteId, Guid currentUserId)
        {
            try
            {
                Sucursal data = await _unitOfWork.SucursalRepository.GetById(sucursalId);
                if (data == null) { return null; }

                data.EmpresaClienteId = empresaClienteId;
                data.FechaModificacion = DateTime.Now;
                data.UsuarioModificadorId = currentUserId;

                _unitOfWork.SucursalRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Inactivate(Guid id, Guid currentUserId)
        {
            try
            {
                Sucursal data = await _unitOfWork.SucursalRepository.GetById(id);
                if (data == null) { return false; }

                data.FechaBaja = DateTime.Now;
                data.UsuarioBajaId = currentUserId;
                data.Estado = 2;

                _unitOfWork.SucursalRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Reactivate(Guid id, Guid currentUserId)
        {
            try
            {
                Sucursal data = await _unitOfWork.SucursalRepository.GetById(id);
                if (data == null) { return false; }

                data.FechaReactivacion = DateTime.Now;
                data.UsuarioReactivadorId = currentUserId;
                data.Estado = 1;

                _unitOfWork.SucursalRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

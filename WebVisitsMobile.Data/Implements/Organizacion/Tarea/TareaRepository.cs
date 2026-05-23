using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Organizacion.Tarea;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;

namespace WebVisitsMobile.Data.Implements.Organizacion.Tarea
{
    public class TareaRepository : Repository<Domain.Entities.Organizacion.Tarea.Tarea>, ITareaRepository
    {
        public TareaRepository(WebVisitsMobileContext context) : base(context) { }

        public async Task<IEnumerable<Domain.Entities.Organizacion.Tarea.Tarea>> GetAllTask()
        {
            return _context.Tarea
                .Include(i => i.TipoTarea)
                .AsEnumerable();
        }

        public async Task<Domain.Entities.Organizacion.Tarea.Tarea> GetTask(Expression<Func<Domain.Entities.Organizacion.Tarea.Tarea, bool>> predicate)
        {
            return _context.Tarea
                .Include(l => l.TipoTarea)
                .AsNoTracking()
                .FirstOrDefault(predicate);
        }

        public async Task<IEnumerable<TareaHID<T>>> GetAllByUserWallet<T>(Guid typeTaskId)
        {
            var tareas = await _context.Tarea
                .Include(t => t.TipoTarea)
                .Where(t => t.Pendiente == 1 && t.Status == 1 && t.TipoTarea.Id == typeTaskId)
                .ToListAsync();

            return tareas.Select(t => new TareaHID<T>
            {
                Id = t.Id,
                TipoTareaId = t.TipoTareaId,
                Fecha = t.Fecha,
                Pendiente = t.Pendiente,
                Status = t.Status,
                ValorRetorno = t.ValorRetorno,
                ReferenciaId = t.ReferenciaId,
                Marca = t.Marca,
                EmpresaClienteId = t.EmpresaClienteId,
                ValorEnvio = string.IsNullOrWhiteSpace(t.ValorEnvio)
                     ? default
                     : JsonSerializer.Deserialize<T>(t.ValorEnvio) ?? default
            });

            //if (typeTaskId == new Guid("8BB6A16A-E148-4952-97A7-76106F048E5D"))
            //{
            //    return tareas.Select(t => new TareaHID<T>
            //    {
            //        Id = t.Id,
            //        TipoTareaId = t.TipoTareaId,
            //        Fecha = t.Fecha,
            //        Pendiente = t.Pendiente,
            //        Status = t.Status,
            //        ValorRetorno = t.ValorRetorno,
            //        ReferenciaId = t.ReferenciaId,
            //        Marca = t.Marca,
            //        EmpresaClienteId = t.EmpresaClienteId,
            //        ValorEnvio = string.IsNullOrWhiteSpace(t.ValorEnvio)
            //          ? null
            //          : JsonSerializer.Deserialize<TareaWalletInactivate>(t.ValorEnvio)
            //    });
            //}

            //if (typeTaskId == new Guid("FD82D317-F02C-4A26-86F4-23766E029BC0")) {
            //    return tareas.Select(t => new TareaHID<T>
            //    {
            //        Id = t.Id,
            //        TipoTareaId = t.TipoTareaId,
            //        Fecha = t.Fecha,
            //        Pendiente = t.Pendiente,
            //        Status = t.Status,
            //        ValorRetorno = t.ValorRetorno,
            //        ReferenciaId = t.ReferenciaId,
            //        Marca = t.Marca,
            //        EmpresaClienteId = t.EmpresaClienteId,
            //        ValorEnvio = string.IsNullOrWhiteSpace(t.ValorEnvio)
            //            ? null
            //            : JsonSerializer.Deserialize<TareaUsuarioHIDWallet>(t.ValorEnvio)
            //    });
            //}

            //return tareas.Select(t => new TareaHID<T>
            //{
            //    Id = t.Id,
            //    TipoTareaId = t.TipoTareaId,
            //    Fecha = t.Fecha,
            //    Pendiente = t.Pendiente,
            //    Status = t.Status,
            //    ValorRetorno = t.ValorRetorno,
            //    ReferenciaId = t.ReferenciaId,
            //    Marca = t.Marca,
            //    EmpresaClienteId = t.EmpresaClienteId,
            //    ValorEnvio = ""
            //});
        }

        public async Task<IEnumerable<TareaHID<TareaPlantilla>>> GetAllByTemplate()
        {
            var tareas = await _context.Tarea
                .Include(t => t.TipoTarea)
                .Where(t => t.Pendiente == 1 && t.Status == 1 && t.TipoTarea.Id == new Guid("022047D9-2E61-44A5-ADE4-AEB2F69CDC17"))
                .ToListAsync();

            return tareas.Select(t => new TareaHID<TareaPlantilla>
            {
                Id = t.Id,
                TipoTareaId = t.TipoTareaId,
                Fecha = t.Fecha,
                Pendiente = t.Pendiente,
                Status = t.Status,
                ValorRetorno = t.ValorRetorno,
                ReferenciaId = t.ReferenciaId,
                Marca = t.Marca,
                EmpresaClienteId = t.EmpresaClienteId,
                ValorEnvio = string.IsNullOrWhiteSpace(t.ValorEnvio)
                    ? null
                    : JsonSerializer.Deserialize<TareaPlantilla>(t.ValorEnvio)
            });
        }
    }
}
using Microsoft.EntityFrameworkCore;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Data.Implements.Common
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly WebVisitsMobileContext _context;
        protected readonly DbSet<T> _entities;

        public Repository(WebVisitsMobileContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _entities.AsEnumerable();
        }

        public async Task<T> GetById(Guid id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task Add(T t)
        {
            await _entities.AddAsync(t);
        }

        public void Update(T t)
        {
            _context.Update(t);
        }

        public async Task Delete(Guid id)
        {
            T t = await GetById(id);
            if (t != null)
                _context.Remove(t);
        }

        public async Task Delete(T entity)
        {
            if (entity == null) return;

            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
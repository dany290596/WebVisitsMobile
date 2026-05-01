using WebVisitsMobile.Domain.Entities.Common;

namespace WebVisitsMobile.Data.Interfaces.Common
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        Task<T> GetById(Guid id);
        Task Add(T t);
        void Update(T t);
        Task Delete(Guid id);
        Task Delete(T entity);
    }
}
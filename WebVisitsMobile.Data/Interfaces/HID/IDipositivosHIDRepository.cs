using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Interfaces.HID
{
    public interface IDipositivosHIDRepository : IRepository<DipositivosHid>
    {
        Task<DipositivosHid> GetDevice(Expression<Func<DipositivosHid, bool>> predicate);
        Task<IEnumerable<DipositivosHid>> GetAllDevice();
    }
}
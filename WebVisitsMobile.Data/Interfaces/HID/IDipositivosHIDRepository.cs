using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Interfaces.HID
{
    public interface IDipositivosHIDRepository : IRepository<DipositivosHid>
    {
        Task<IEnumerable<DipositivosHid>> GetAllDevice();
    }
}
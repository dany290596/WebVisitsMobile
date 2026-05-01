using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Interfaces.HID
{
    public interface ICredencialHIDRepository : IRepository<CredencialHid>
    {
        Task<IEnumerable<CredencialHid>> GetAllCredentialHID();
    }
}
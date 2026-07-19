using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Interfaces.HID
{
    public interface ILicenciaHIDRepository : IRepository<LicenciaHID>
    {
        Task<IEnumerable<LicenciaHID>> GetAllLicense();
    }
}
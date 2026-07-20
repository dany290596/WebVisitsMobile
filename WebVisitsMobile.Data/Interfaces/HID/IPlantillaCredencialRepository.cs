using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Interfaces.HID
{
    public interface IPlantillaCredencialRepository : IRepository<PlantillaCredencial>
    {
        Task<PlantillaCredencial> GetCredentialTemplate(Expression<Func<PlantillaCredencial, bool>> predicate);
        Task<IEnumerable<PlantillaCredencial>> GetAllCredentialTemplate();
    }
}
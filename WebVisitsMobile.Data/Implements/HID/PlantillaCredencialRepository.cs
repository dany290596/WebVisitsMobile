using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.HID;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Implements.HID
{
    public class PlantillaCredencialRepository : Repository<PlantillaCredencial>, IPlantillaCredencialRepository
    {
        public PlantillaCredencialRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
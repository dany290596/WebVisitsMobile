using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.HID;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Implements.HID
{
    public class TipoCredencialRepository : Repository<TipoCredencial>, ITipoCredencialRepository
    {
        public TipoCredencialRepository(WebVisitsMobileContext context) : base(context) { }
    }
}
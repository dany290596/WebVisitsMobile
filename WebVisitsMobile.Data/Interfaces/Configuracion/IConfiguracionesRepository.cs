using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Configuracion;

namespace WebVisitsMobile.Data.Interfaces.Configuracion
{
    public interface IConfiguracionesRepository : IRepository<Configuraciones>
    {
        Task<IEnumerable<Configuraciones>> GetAllSetting();
        Task<Configuraciones> GetSetting(Expression<Func<Configuraciones, bool>> predicate);
        Task AddRangeSetting(IEnumerable<Configuraciones> settings);
        Task<List<SettingsGroup>> GetSettingGroupByCompany();
        void DeleteRange(IEnumerable<Configuraciones> settings);
        IQueryable<Configuraciones> GetAllSettingQueryable();
    }
}
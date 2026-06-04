using System.Linq.Expressions;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;

namespace WebVisitsMobile.Data.Interfaces.HID
{
    public interface ILicenciaUserHIDRepository : IRepository<LicenciaHidUser>
    {
        Task<IEnumerable<LicenciaHidUser>> GetAllUserHID();
        Task<bool> UpdateStatusOnly(Guid userHIDId, string newInvitationActivity, int newStatus, Guid currentUserId);
        Task<List<UserHIDExpired>> GetAllUsersHIDExpired();
        Task<UserHIDExpired> GetUserHIDExpired(Guid id);
        Task<LicenciaHidUser?> GetUserHIDWithCredential(Guid? externalId);
        Task<LicenciaHidUser> GetUserHID(Expression<Func<LicenciaHidUser, bool>> predicate);
        Task<List<LicenciaHidUser>> GetAllLicenciasExpiradas();
        Task<bool> ExisteEmailEnLicenciaHidUser(string email);
        Task<LicenciaHidUser?> GetLicenciaVigenteByEmail(string email);
    }
}
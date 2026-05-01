using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Models.Common;
using WebVisitsMobile.Models.HID.UserHID;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface ILicenciaUserHIDService
    {
        Task<bool> Create(LicenciaHidUser licenseUserHID, Guid currentClientCompanyId, Guid currentUserId);
        Task<bool> Update(LicenciaHidUser licenseUserHID, Guid currentUserId);
        Task<bool> UpdateWithAttributes(LicenciaHidUser licenseUserHID, Guid clientCompanyId, Guid currentUserId);
        Task<bool> UpdateStatus(Guid userHIDId, string newInvitationActivity, int newStatus, Guid currentUserId);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<bool> InactivateById(Guid id, Guid userLowId);
        Task<bool> InactivateWithHIDAndTask(Guid id, Guid currentUserId, Guid clientCompanyId);
        Task<bool> InactivateWithHID(UserHIDEliminarDTO data, Guid clientCompanyId, Guid currentUserId);
        Task<PagedList<LicenciaHidUser>> GetAll(LicenciaUserHIDQueryFilter filters);
        Task<PagedList<CommonDTO>> GetAllQuery(LicenciaUserHIDQueryFilter filters);
        Task<LicenciaHidUser> GetById(Guid licenseUserHIDId);
        Task<LicenciaHidUser> GetByUserHIDId(int UserHIDId);
        Task<CodigoInvitacionEmailHIDDTO> SendInvitationCodeByEmailHID(CodigoInvitacionHIDDTO invitationRequest, Guid clientCompanyId, Guid currentUserId);
        Task<LicenciaHidUser?> ExistUserWVM(string email, Guid clientCompanyId);
        Task<UserHIDExpired> GetExpired(Guid id);
        Task<List<UserHIDExpired>> GetAllExpired();
        Task<UserHIDWithCredentialsDTO?> GetWithCredentials(Guid externalId);
    }
}
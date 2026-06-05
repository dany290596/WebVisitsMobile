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
        Task<LicenciaHidUser?> UpdatePartial(LicenciaHidUser licenseUserHID, Guid clientCompanyId, Guid currentUserId);
        Task<bool> UpdateWithAttributes(LicenciaHidUser licenseUserHID, Guid clientCompanyId, Guid currentUserId);
        Task<bool> UpdateStatus(Guid userHIDId, string newInvitationActivity, int newStatus, Guid currentUserId);
        Task<bool> UpdateStatus(Guid userId, int status);
        Task<bool> UpdateStatusByIntId(int userId, int status);
        Task<bool> Reactivate(Guid id, Guid currentUserId);
        Task<bool> Inactivate(Guid id, Guid currentUserId);
        Task<bool> InactivateById(Guid id, Guid userLowId);
        Task<bool> InactivateWithHIDAndTask(Guid id, Guid currentUserId, Guid clientCompanyId);
        Task<bool> InactivateWithWalletAndTask(Guid id, Guid currentUserId, Guid clientCompanyId);
        Task<bool> ReactivateWithWalletAndTask(Guid id, Guid currentUserId, Guid clientCompanyId);
        Task<bool> InactivateWithHID(Guid id, Guid clientCompanyId, Guid currentUserId);
        Task<PagedList<LicenciaHidUser>> GetAll(LicenciaUserHIDQueryFilter filters);
        Task<PagedList<CommonDTO>> GetAllQuery(LicenciaUserHIDQueryFilter filters);
        Task<LicenciaHidUser> GetById(Guid licenseUserHIDId);
        Task<LicenciaHidUser> GetByPhoto(Guid licenseUserHIDId);
        Task<LicenciaHidUser> GetByUserHIDId(int UserHIDId);
        Task<CodigoInvitacionEmailHIDDTO> SendInvitationCodeByEmailHID(CodigoInvitacionHIDDTO invitationRequest, Guid clientCompanyId, Guid currentUserId);
        Task<LicenciaHidUser?> ExistUserWVM(string email, Guid clientCompanyId);
        Task<UserHIDExpired> GetExpired(Guid id);
        Task<List<UserHIDExpired>> GetAllExpired();
        Task<UserHIDWithCredentialsDTO?> GetWithCredentials(Guid externalId);
        Task<LicenciaHidUser> GetByIdExpired(Guid licenseUserHIDId);
        Task<bool> CreateTypeCredential(UserHIDTypeCredentialReqDTO licenseUserHID, Guid currentClientCompanyId, Guid currentUserId);
        Task<List<LicenciaHidUser>> GetAllLicenciasExpiradas();
        Task<bool> ExisteEmailEnLicenciaHidUser(string email);
        Task<string?> GetInvitacionDetalleVigenteByEmail(string email);
        Task<LicenciaHidUser?> GetByExternalId(Guid externalId);
        Task<bool> TieneCredencialWallet(Guid licenciaHidUserId);
        Task<string?> GetCredencialWalletMasReciente(Guid licenciaHidUserId);
    }
}
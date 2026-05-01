using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Models.Configuracion.Configuraciones;
using WebVisitsMobile.Models.HID.UserHID;

namespace WebVisitsMobile.Services.Interfaces.HID
{
    public interface IHIDService
    {
        Task<TokenHID?> GetTokenHIDAsync(AppSettingDTO settingHID);
        Task<UserMapperHIDDTO?> GetAllUserAsync(AppSettingDTO settingHID, int userHIDId, string tokenHID);
        Task<bool> DeleteCredentialAsync(AppSettingDTO settingHID, int credentialHIDId, string tokenHID);
        Task<bool> DeleteInvitationAsync(AppSettingDTO settingHID, int invitationHIDId, string tokenHID);
        Task<InvitacionYCredencialHIDDTO?> AddInvitationAsync(AppSettingDTO settingHID, int userHIDId, string tokenHID);
        Task<bool> SendInvitationCodeByEmailAsync(AppSettingDTO settingHID, int invitationHIDId, string tokenHID);
        Task<bool> DeleteUserAsync(AppSettingDTO settingHID, UserHIDEliminarDTO licenciaUserHIDEliminar, string tokenHID);
        Task<CustomerDTO?> GetCustomerAsync(AppSettingDTO settingHID, string tokenHID);
        Task<SDKVersionDTO?> GetSdkVersionAsync(AppSettingDTO settingHID, string tokenHID);
        Task<UserHIDAtributosDTO?> UpdateUserAsync(AppSettingDTO settingHID, UserHIDEditarAtributosDTO attributesDTO, string tokenHID);
        Task<int> GetUserByEmailAsync(AppSettingDTO configuracionHID, string email, string tokenHID);
        Task<UserMapperRespHIDDTO?> CreateUserAsync(AppSettingDTO settingHID, LicenciaHidUser userHID, string tokenHID);
    }
}
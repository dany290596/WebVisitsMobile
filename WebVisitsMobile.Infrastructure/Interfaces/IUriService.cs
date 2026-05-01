using WebVisitsMobile.Services.QueryFilters.Administracion.Aplicacion;
using WebVisitsMobile.Services.QueryFilters.Administracion.Modulo;
using WebVisitsMobile.Services.QueryFilters.Administracion.Perfil;
using WebVisitsMobile.Services.QueryFilters.Administracion.Seccion;
using WebVisitsMobile.Services.QueryFilters.Administracion.Sesion;
using WebVisitsMobile.Services.QueryFilters.Configuracion;
using WebVisitsMobile.Services.QueryFilters.Empresa;
using WebVisitsMobile.Services.QueryFilters.HID;
using WebVisitsMobile.Services.QueryFilters.Organizacion.Tarea;

namespace WebVisitsMobile.Infrastructure.Interfaces
{
    public interface IUriService
    {
        Uri GetUserPaginationUri(UsuarioQueryFilter filter, string actionUrl);
        Uri GetProfilePaginationUri(PerfilQueryFilter filter, string actionUrl);
        Uri GetClientCompanyPaginationUri(EmpresaClienteQueryFilter filter, string actionUrl);
        Uri GetLicenseHIDPaginationUri(LicenciaHIDQueryFilter filter, string actionUrl);
        Uri GetDeviceHIDPaginationUri(DipositivosHIDQueryFilter filter, string actionUrl);
        Uri GetCredentialHIDPaginationUri(CredencialHIDQueryFilter filter, string actionUrl);
        Uri GetLicenseUserHIDPaginationUri(LicenciaUserHIDQueryFilter filter, string actionUrl);
        Uri GetTaskPaginationUri(TareaQueryFilter filter, string actionUrl);
        Uri GetTaskTypePaginationUri(TipoTareaQueryFilter filter, string actionUrl);
        Uri GetSettingPaginationUri(ConfiguracionesQueryFilter filter, string actionUrl);
        Uri GetApplicationPaginationUri(AplicacionQueryFilter filter, string actionUrl);
        Uri GetModulePaginationUri(ModuloQueryFilter filter, string actionUrl);
        Uri GetSectionPaginationUri(SeccionQueryFilter filter, string actionUrl);
        Uri GetTypeUserPaginationUri(TipoUsuarioQueryFilter filter, string actionUrl);
    }
}
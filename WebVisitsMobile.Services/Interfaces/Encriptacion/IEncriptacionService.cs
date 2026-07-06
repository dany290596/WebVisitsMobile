using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Models.Encriptacion;

namespace WebVisitsMobile.Services.Interfaces.Encriptacion
{
    public interface IEncriptacionService
    {
        Task<DesebcriptarDTO> EncriptarCadena(string cadena);
        Task<IntentosRecuperacion> DesencriptarIntentos(DesebcriptarDTO datos);
        Task<ClaveRecuperacion> DesencriptarClaveRecuperacion(DesebcriptarDTO datos);
        Task<List<SettingsGroupTap>> DesencriptarCredential(DesebcriptarDTO datos);
    }
}
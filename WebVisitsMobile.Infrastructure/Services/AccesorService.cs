using Microsoft.AspNetCore.Http;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Infrastructure.Interfaces;

namespace WebVisitsMobile.Infrastructure.Services
{
    public class AccesorService : IAccesorService
    {
        IHttpContextAccessor _accesor;

        public AccesorService(IHttpContextAccessor accesor)
        {
            _accesor = accesor;

        }

        public Guid GetClaimUsuarioId()
        {
            Guid gId = Guid.Empty;

            try
            {
                string usuarioId = _accesor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "UsuarioId").Value;
            }
            catch (Exception ex)
            {
                throw;
            }

            return gId;
        }

        public Token GetTokenData()
        {
            try
            {
                var UsuarioId = _accesor?.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type.EndsWith("UsuarioId"))?.Value;

                if (UsuarioId == null)
                    return null;


                var CorreoElectronico = _accesor?.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type.EndsWith("Correo"))?.Value;
                var PerfilId = _accesor?.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type.EndsWith("PerfilId"))?.Value;
                var PerfilName = _accesor?.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type.EndsWith("PerfilName"))?.Value;
                var EmpresaId = _accesor?.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type.EndsWith("EmpresaId"))?.Value;
                var SesionId = _accesor?.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type.EndsWith("SesionId"))?.Value;
                var AsociadoId = _accesor?.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type.EndsWith("AsociadoId"))?.Value;
                var TipoUsuarioId = _accesor?.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type.EndsWith("TipoUsuarioId"))?.Value;
                var TipoUsuarioName = _accesor?.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type.EndsWith("TipoUsuarioName"))?.Value;

                // Crear la instancia de TokenData
                Token tokenData = new Token();

                // Asignar valores verificando que no sean nulos
                tokenData.UsuarioId = UsuarioId != null ? new Guid(UsuarioId) : Guid.Empty;
                tokenData.Email = CorreoElectronico ?? string.Empty;
                tokenData.PerfilId = PerfilId != null ? new Guid(PerfilId) : Guid.Empty;
                tokenData.PerfilName = PerfilName ?? string.Empty;
                tokenData.EmpresaId = EmpresaId != null ? new Guid(EmpresaId) : Guid.Empty;
                tokenData.SesionId = SesionId != null ? new Guid(SesionId) : Guid.Empty;
                tokenData.AsociadoId = AsociadoId != null ? new Guid(AsociadoId) : Guid.Empty;
                tokenData.TipoUsuarioId = TipoUsuarioId != null ? new Guid(TipoUsuarioId) : Guid.Empty;
                tokenData.TipoUsuarioName = TipoUsuarioName ?? string.Empty;

                return tokenData;
            }
            catch (Exception ex)
            {
                // Puedes registrar el error aquí si es necesario
                return null;
            }
        }
    }
}
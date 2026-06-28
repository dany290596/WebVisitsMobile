namespace WebVisitsMobile.Services.Interfaces.Administracion.Sesion
{
    public interface ISesionService
    {
        Task<bool> Insert(Domain.Entities.Administracion.Sesion.Sesion SesionUsuario, Guid UsuarioActualId, Guid EmpresaId);
        Task<bool> VerifyFirstConnection(Guid userId);
    }
}
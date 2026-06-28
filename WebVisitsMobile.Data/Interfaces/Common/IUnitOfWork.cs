using WebVisitsMobile.Data.Interfaces.Administracion.Aplicacion;
using WebVisitsMobile.Data.Interfaces.Administracion.Modulo;
using WebVisitsMobile.Data.Interfaces.Administracion.Perfil;
using WebVisitsMobile.Data.Interfaces.Administracion.Seccion;
using WebVisitsMobile.Data.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Data.Interfaces.Configuracion;
using WebVisitsMobile.Data.Interfaces.Empresa;
using WebVisitsMobile.Data.Interfaces.HID;
using WebVisitsMobile.Data.Interfaces.Organizacion.Tarea;
using WebVisitsMobile.Data.Interfaces.Parametrizacion;
using WebVisitsMobile.Data.Interfaces.Ubicacion;

namespace WebVisitsMobile.Data.Interfaces.Common
{
    public interface IUnitOfWork : IDisposable
    {
        IUsuarioRepository UsuarioRepository { get; }
        IAplicacionRepository AplicacionRepository { get; }
        IModuloRepository ModuloRepository { get; }
        ISeccionRepository SeccionRepository { get; }
        IPerfilPermisoSeccionRepository PerfilPermisoSeccionRepository { get; }
        IPerfilRepository PerfilRepository { get; }
        ITipoUsuarioRepository TipoUsuarioRepository { get; }
        IEmpresaClienteRepository EmpresaClienteRepository { get; }
        ISesionRepository SesionRepository { get; }
        ILicenciaHIDRepository LicenciaHIDRepository { get; }
        IDipositivosHIDRepository DipositivosHIDRepository { get; }
        ICredencialHIDRepository CredencialHIDRepository { get; }
        ILicenciaUserHIDRepository LicenciaUserHIDRepository { get; }
        ITareaRepository TareaRepository { get; }
        ITipoTareaRepository TipoTareaRepository { get; }
        IConfiguracionesRepository ConfiguracionesRepository { get; }
        ITipoCredencialRepository TipoCredencialRepository { get; }
        IUsuarioHidTipoCredencialRepository UsuarioHidTipoCredencialRepository { get; }
        IPlantillaCredencialRepository PlantillaCredencialRepository { get; }
        IPaisRepository PaisRepository { get; }
        IPaisEstadoRepository PaisEstadoRepository { get; }
        ICiudadRepository CiudadRepository { get; }
        ICorreoEnviarRepository CorreoEnviarRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
        Task<int> CommitAsync();
    }
}
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Administracion.Aplicacion;
using WebVisitsMobile.Data.Implements.Administracion.Modulo;
using WebVisitsMobile.Data.Implements.Administracion.Perfil;
using WebVisitsMobile.Data.Implements.Administracion.Seccion;
using WebVisitsMobile.Data.Implements.Administracion.Sesion;
using WebVisitsMobile.Data.Implements.Configuracion;
using WebVisitsMobile.Data.Implements.Empresa;
using WebVisitsMobile.Data.Implements.HID;
using WebVisitsMobile.Data.Implements.Organizacion.Tarea;
using WebVisitsMobile.Data.Implements.Parametrizacion;
using WebVisitsMobile.Data.Implements.Ubicacion;
using WebVisitsMobile.Data.Interfaces.Administracion.Aplicacion;
using WebVisitsMobile.Data.Interfaces.Administracion.Modulo;
using WebVisitsMobile.Data.Interfaces.Administracion.Perfil;
using WebVisitsMobile.Data.Interfaces.Administracion.Seccion;
using WebVisitsMobile.Data.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Data.Interfaces.Configuracion;
using WebVisitsMobile.Data.Interfaces.Empresa;
using WebVisitsMobile.Data.Interfaces.HID;
using WebVisitsMobile.Data.Interfaces.Organizacion.Tarea;
using WebVisitsMobile.Data.Interfaces.Parametrizacion;
using WebVisitsMobile.Data.Interfaces.Ubicacion;

namespace WebVisitsMobile.Data.Implements.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WebVisitsMobileContext _context;

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAplicacionRepository _aplicacionRepository;
        private readonly IModuloRepository _moduloRepository;
        private readonly ISeccionRepository _seccionRepository;
        private readonly IPerfilPermisoSeccionRepository _perfilPermisoSeccionRepository;
        private readonly IPerfilRepository _perfilRepository;
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;
        private readonly IEmpresaClienteRepository _empresaClienteRepository;
        private readonly ISesionRepository _sesionRepository;
        private readonly ILicenciaHIDRepository _licenciaHIDRepository;
        private readonly IDipositivosHIDRepository _dipositivosHIDRepository;
        private readonly ICredencialHIDRepository _credencialHIDRepository;
        private readonly ILicenciaUserHIDRepository _licenciaUserHIDRepository;
        private readonly ITareaRepository _tareaRepository;
        private readonly ITipoTareaRepository _tipoTareaRepository;
        private readonly IConfiguracionesRepository _configuracionesRepository;
        private readonly ITipoCredencialRepository _tipoCredencialRepository;
        private readonly IUsuarioHidTipoCredencialRepository _usuarioHidTipoCredencialRepository;
        private readonly IPlantillaCredencialRepository _plantillaCredencialRepository;
        private readonly IPaisRepository _paisRepository;
        private readonly IPaisEstadoRepository _paisEstadoRepository;
        private readonly ICiudadRepository _ciudadRepository;
        private readonly ICorreoEnviarRepository _correoEnviarRepository;
        private readonly ITipoPlantillaNotificacionRepository _tipoPlantillaNotificacionRepository;
        private readonly IPlantillaNotificacionRepository _plantillaNotificacionRepository;

        public UnitOfWork(WebVisitsMobileContext context)
        {
            _context = context;
        }

        public IUsuarioRepository UsuarioRepository => _usuarioRepository ?? new UsuarioRepository(_context);
        public IAplicacionRepository AplicacionRepository => _aplicacionRepository ?? new AplicacionRepository(_context);
        public IModuloRepository ModuloRepository => _moduloRepository ?? new ModuloRepository(_context);
        public ISeccionRepository SeccionRepository => _seccionRepository ?? new SeccionRepository(_context);
        public IPerfilPermisoSeccionRepository PerfilPermisoSeccionRepository => _perfilPermisoSeccionRepository ?? new PerfilPermisoSeccionRepository(_context);
        public IPerfilRepository PerfilRepository => _perfilRepository ?? new PerfilRepository(_context);
        public ITipoUsuarioRepository TipoUsuarioRepository => _tipoUsuarioRepository ?? new TipoUsuarioRepository(_context);
        public IEmpresaClienteRepository EmpresaClienteRepository => _empresaClienteRepository ?? new EmpresaClienteRepository(_context);
        public ISesionRepository SesionRepository => _sesionRepository ?? new SesionRepository(_context);
        public ILicenciaHIDRepository LicenciaHIDRepository => _licenciaHIDRepository ?? new LicenciaHIDRepository(_context);
        public IDipositivosHIDRepository DipositivosHIDRepository => _dipositivosHIDRepository ?? new DipositivosHIDRepository(_context);
        public ICredencialHIDRepository CredencialHIDRepository => _credencialHIDRepository ?? new CredencialHIDRepository(_context);
        public ILicenciaUserHIDRepository LicenciaUserHIDRepository => _licenciaUserHIDRepository ?? new LicenciaUserHIDRepository(_context);
        public ITareaRepository TareaRepository => _tareaRepository ?? new TareaRepository(_context);
        public ITipoTareaRepository TipoTareaRepository => _tipoTareaRepository ?? new TipoTareaRepository(_context);
        public IConfiguracionesRepository ConfiguracionesRepository => _configuracionesRepository ?? new ConfiguracionesRepository(_context);
        public ITipoCredencialRepository TipoCredencialRepository => _tipoCredencialRepository ?? new TipoCredencialRepository(_context);
        public IUsuarioHidTipoCredencialRepository UsuarioHidTipoCredencialRepository => _usuarioHidTipoCredencialRepository ?? new UsuarioHidTipoCredencialRepository(_context);
        public IPlantillaCredencialRepository PlantillaCredencialRepository => _plantillaCredencialRepository ?? new PlantillaCredencialRepository(_context);
        public IPaisRepository PaisRepository => _paisRepository ?? new PaisRepository(_context);
        public IPaisEstadoRepository PaisEstadoRepository => _paisEstadoRepository ?? new PaisEstadoRepository(_context);
        public ICiudadRepository CiudadRepository => _ciudadRepository ?? new CiudadRepository(_context);
        public ICorreoEnviarRepository CorreoEnviarRepository => _correoEnviarRepository ?? new CorreoEnviarRepository(_context);
        public ITipoPlantillaNotificacionRepository TipoPlantillaNotificacionRepository => _tipoPlantillaNotificacionRepository ?? new TipoPlantillaNotificacionRepository(_context);
        public IPlantillaNotificacionRepository PlantillaNotificacionRepository => _plantillaNotificacionRepository ?? new PlantillaNotificacionRepository(_context);
        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
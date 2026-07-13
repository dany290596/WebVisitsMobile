using Microsoft.EntityFrameworkCore;
using WebVisitsMobile.Domain.Entities.Administracion.Aplicacion;
using WebVisitsMobile.Domain.Entities.Administracion.Modulo;
using WebVisitsMobile.Domain.Entities.Administracion.Perfil;
using WebVisitsMobile.Domain.Entities.Administracion.Seccion;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;
using WebVisitsMobile.Domain.Entities.Parametrizacion;
using WebVisitsMobile.Domain.Entities.Ubicacion;

namespace WebVisitsMobile.Data.Context
{
    public class WebVisitsMobileContext : DbContext
    {
        public WebVisitsMobileContext(DbContextOptions<WebVisitsMobileContext> options) : base(options) { }

        public virtual DbSet<Usuario> Usuario { get; set; } = null!;
        public virtual DbSet<Perfil> Perfil { get; set; }
        public virtual DbSet<Aplicacion> Aplicacion { get; set; }
        public virtual DbSet<Modulo> Modulo { get; set; }
        public virtual DbSet<Seccion> Seccion { get; set; }
        public virtual DbSet<PerfilPermisoSeccion> PerfilPermisoSeccion { get; set; }
        public virtual DbSet<TipoUsuario> TipoUsuario { get; set; }
        public virtual DbSet<EmpresaCliente> EmpresaCliente { get; set; }
        public virtual DbSet<Sesion> Sesion { get; set; }
        public virtual DbSet<LicenciaHID> LicenciaHID { get; set; } = null!;
        public virtual DbSet<DipositivosHid> DipositivosHid { get; set; } = null!;
        public virtual DbSet<CredencialHid> CredencialHid { get; set; } = null!;
        public virtual DbSet<LicenciaHidUser> LicenciaHidUser { get; set; } = null!;
        public virtual DbSet<Tarea> Tarea { get; set; } = null!;
        public virtual DbSet<TipoTarea> TipoTarea { get; set; } = null!;
        public virtual DbSet<Configuraciones> Configuraciones { get; set; }
        public virtual DbSet<TipoCredencial> TipoCredencial { get; set; } = null!;
        public virtual DbSet<UsuarioHidTipoCredencial> UsuarioHidTipoCredencial { get; set; }
        public virtual DbSet<PlantillaCredencial> PlantillaCredencial { get; set; }

        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<Ciudad> Ciudad { get; set; }
        public virtual DbSet<PaisEstado> PaisEstado { get; set; }

        public virtual DbSet<CorreoEnviar> CorreoEnviar { get; set; }

        public virtual DbSet<Sucursal> Sucursal { get; set; }
    }
}
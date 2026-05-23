using AutoMapper;
using WebVisitsMobile.Domain.Entities.Administracion.Aplicacion;
using WebVisitsMobile.Domain.Entities.Administracion.Modulo;
using WebVisitsMobile.Domain.Entities.Administracion.Perfil;
using WebVisitsMobile.Domain.Entities.Administracion.Seccion;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Domain.Entities.Empresa;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;
using WebVisitsMobile.Domain.Entities.Ubicacion;
using WebVisitsMobile.Models.Administracion.Aplicacion.Aplicacion;
using WebVisitsMobile.Models.Administracion.Modulo.Modulo;
using WebVisitsMobile.Models.Administracion.Perfil.Perfil;
using WebVisitsMobile.Models.Administracion.Perfil.PerfilPermisoSeccion;
using WebVisitsMobile.Models.Administracion.Seccion.Seccion;
using WebVisitsMobile.Models.Administracion.Sesion.TipoUsuario;
using WebVisitsMobile.Models.Administracion.Sesion.Usuario;
using WebVisitsMobile.Models.Configuracion.Configuraciones;
using WebVisitsMobile.Models.Empresa.EmpresaCliente;
using WebVisitsMobile.Models.HID.CredencialHID;
using WebVisitsMobile.Models.HID.DipositivosHID;
using WebVisitsMobile.Models.HID.LicenciaHID;
using WebVisitsMobile.Models.HID.PlantillaCredencial;
using WebVisitsMobile.Models.HID.TipoCredencial;
using WebVisitsMobile.Models.HID.UserHID;
using WebVisitsMobile.Models.HID.UsuarioHidTipoCredencial;
using WebVisitsMobile.Models.Organizacion.Tarea.Tarea;
using WebVisitsMobile.Models.Organizacion.Tarea.TipoTarea;
using WebVisitsMobile.Models.Ubicacion.Ciudad;
using WebVisitsMobile.Models.Ubicacion.Pais;
using WebVisitsMobile.Models.Ubicacion.PaisEstado;

namespace WebVisitsMobile.Infrastructure.Mappings
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            ConfigureMappings();
        }

        private void ConfigureMappings()
        {
            CreateMap<Token, TokenRespDTO>().ReverseMap();

            CreateMap<Usuario, UsuarioRespDTO>()
               .ForMember(udto => udto.Correo, u => u.MapFrom(mf => mf.Correo))
               .ForMember(udto => udto.PerfilId, u => u.MapFrom(mf => mf.PerfilId))
               .ForMember(udto => udto.PerfilNombre, u => u.MapFrom(mf => mf.Perfil.Nombre))
               .ForMember(udto => udto.PerfilEstado, u => u.MapFrom(mf => mf.Perfil.Estado))
               .ForMember(udto => udto.TipoUsuarioId, u => u.MapFrom(mf => mf.TipoUsuarioId))
               .ForMember(udto => udto.TipoUsuarioNombre, u => u.MapFrom(mf => mf.TipoUsuario.Nombre))
               .ForMember(udto => udto.TipoUsuarioEstado, u => u.MapFrom(mf => mf.TipoUsuario.Estado))
               .ForMember(udto => udto.IdAsociado, u => u.MapFrom(mf => mf.IdAsociado))
               .ForMember(udto => udto.Vence, u => u.MapFrom(mf => mf.Vence))
               .ForMember(udto => udto.FechaVencimiento, u => u.MapFrom(mf => mf.FechaVencimiento))
            .ReverseMap();

            CreateMap<Usuario, UsuarioReqDTO>().ReverseMap();

            CreateMap<Perfil, PerfilRespDTO>().ReverseMap();
            CreateMap<Perfil, PerfilReqDTO>().ReverseMap();

            CreateMap<TipoUsuario, TipoUsuarioRespDTO>().ReverseMap();
            CreateMap<TipoUsuario, TipoUsuarioReqDTO>().ReverseMap();

            CreateMap<CredencialHid, CredencialHIDRespDTO>().ReverseMap();
            CreateMap<CredencialHid, CredencialHIDReqDTO>().ReverseMap();

            CreateMap<DipositivosHid, DipositivosHIDRespDTO>().ReverseMap();
            CreateMap<DipositivosHid, DipositivosHIDReqDTO>().ReverseMap();

            CreateMap<LicenciaHID, LicenciaHIDRespDTO>().ReverseMap();
            CreateMap<LicenciaHID, LicenciaHIDReqDTO>().ReverseMap();

            CreateMap<LicenciaHidUser, UserHIDRespDTO>().ReverseMap();
            CreateMap<LicenciaHidUser, UserHIDReqDTO>().ReverseMap();
            CreateMap<LicenciaHidUser, UserHIDEditDTO>().ReverseMap();

            CreateMap<EmpresaCliente, EmpresaClienteRespDTO>().ReverseMap();
            CreateMap<EmpresaCliente, EmpresaClienteReqDTO>().ReverseMap();

            CreateMap<Aplicacion, AplicacionRespDTO>().ReverseMap();
            CreateMap<Aplicacion, AplicacionReqDTO>().ReverseMap();

            CreateMap<Modulo, ModuloRespDTO>().ReverseMap();
            CreateMap<Modulo, ModuloReqDTO>().ReverseMap();

            CreateMap<Seccion, SeccionRespDTO>().ReverseMap();
            CreateMap<Seccion, SeccionReqDTO>().ReverseMap();

            CreateMap<PerfilPermisoSeccion, PerfilPermisoSeccionRespDTO>().ReverseMap();
            CreateMap<PerfilPermisoSeccion, PerfilPermisoSeccionReqDTO>().ReverseMap();

            CreateMap<TipoTarea, TipoTareaRespDTO>().ReverseMap();
            CreateMap<TipoTarea, TipoTareaReqDTO>().ReverseMap();

            CreateMap<Tarea, TareaRespDTO>().ReverseMap();
            CreateMap<Tarea, TareaReqDTO>().ReverseMap();

            CreateMap<Configuraciones, ConfiguracionesRespDTO>().ReverseMap();
            CreateMap<Configuraciones, ConfiguracionesReqDTO>().ReverseMap();

            CreateMap<TipoCredencial, TipoCredencialRespDTO>().ReverseMap();
            CreateMap<TipoCredencial, TipoCredencialReqDTO>().ReverseMap();

            CreateMap<UsuarioHidTipoCredencial, UsuarioHidTipoCredencialRespDTO>().ReverseMap();
            CreateMap<UsuarioHidTipoCredencial, UsuarioHidTipoCredencialReqDTO>().ReverseMap();

            CreateMap<PlantillaCredencial, PlantillaCredencialRespDTO>().ReverseMap();
            CreateMap<PlantillaCredencial, PlantillaCredencialReqDTO>().ReverseMap();

            CreateMap<Pais, PaisRespDTO>().ReverseMap();
            CreateMap<Pais, PaisReqDTO>().ReverseMap();

            CreateMap<PaisEstado, PaisEstadoRespDTO>().ReverseMap();
            CreateMap<PaisEstado, PaisEstadoReqDTO>().ReverseMap();

            CreateMap<Ciudad, CiudadRespDTO>().ReverseMap();
            CreateMap<Ciudad, CiudadReqDTO>().ReverseMap();
        }
    }
}
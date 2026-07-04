using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebVisitsMobile.Data.Context;
using WebVisitsMobile.Data.Implements.Common;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Infrastructure.Interfaces;
using WebVisitsMobile.Infrastructure.Services;
using WebVisitsMobile.Services.Interfaces.Administracion.Aplicacion;
using WebVisitsMobile.Services.Interfaces.Administracion.Modulo;
using WebVisitsMobile.Services.Interfaces.Administracion.Perfil;
using WebVisitsMobile.Services.Interfaces.Administracion.Seccion;
using WebVisitsMobile.Services.Interfaces.Administracion.Sesion;
using WebVisitsMobile.Services.Interfaces.Configuracion;
using WebVisitsMobile.Services.Interfaces.Empresa;
using WebVisitsMobile.Services.Interfaces.Encriptacion;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.Interfaces.Organizacion.Email;
using WebVisitsMobile.Services.Interfaces.Organizacion.Tarea;
using WebVisitsMobile.Services.Interfaces.Parametrizacion;
using WebVisitsMobile.Services.Interfaces.Ubicacion;
using WebVisitsMobile.Services.Services.Administracion.Aplicacion;
using WebVisitsMobile.Services.Services.Administracion.Modulo;
using WebVisitsMobile.Services.Services.Administracion.Perfil;
using WebVisitsMobile.Services.Services.Administracion.Seccion;
using WebVisitsMobile.Services.Services.Administracion.Sesion;
using WebVisitsMobile.Services.Services.Configuracion;
using WebVisitsMobile.Services.Services.Empresa;
using WebVisitsMobile.Services.Services.Encriptacion;
using WebVisitsMobile.Services.Services.HID;
using WebVisitsMobile.Services.Services.Organizacion.Email;
using WebVisitsMobile.Services.Services.Organizacion.Tarea;
using WebVisitsMobile.Services.Services.Parametrizacion;
using WebVisitsMobile.Services.Services.Ubicacion;

namespace WebVisitsMobile.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<WebVisitsMobileContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("WebVisitsMobile"));
                options.EnableSensitiveDataLogging();
            });

            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PaginationOption>(configuration.GetSection("Pagination"));
            services.Configure<Options.PasswordOption>(configuration.GetSection("PasswordOptions"));
            services.Configure<Options.AesManagedOption>(configuration.GetSection("AesManagedOptions"));

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IPlataformaService, PlataformService>();
            services.AddTransient<IUsuarioService, UsuarioService>();
            services.AddTransient<IAplicacionService, AplicacionService>();
            services.AddTransient<IModuloService, ModuloService>();
            services.AddTransient<ISeccionService, SeccionService>();
            services.AddTransient<IPerfilPermisoSeccionService, PerfilPermisoSeccionService>();
            services.AddTransient<IEmpresaClienteService, EmpresaClienteService>();
            services.AddTransient<ITipoUsuarioService, TipoUsuarioService>();
            services.AddTransient<IPerfilService, PerfilService>();
            services.AddTransient<ISesionService, SesionService>();
            services.AddTransient<ITareaService, TareaService>();
            services.AddTransient<ITipoTareaService, TipoTareaService>();
            services.AddTransient<ILicenciaHIDService, LicenciaHIDService>();
            services.AddTransient<IDipositivosHIDService, DipositivosHIDService>();
            services.AddTransient<ICredencialHIDService, CredencialHIDService>();
            services.AddTransient<ILicenciaUserHIDService, LicenciaUserHIDService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IConfiguracionService, ConfiguracionService>();
            services.AddTransient<IWebhookEventService, WebhookEventService>();
            services.AddTransient<IHIDService, HIDService>();
            services.AddTransient<ITipoCredencialService, TipoCredencialService>();
            services.AddTransient<IUsuarioHidTipoCredencialService, UsuarioHidTipoCredencialService>();
            services.AddTransient<IPlantillaCredencialService, PlantillaCredencialService>();
            services.AddTransient<IPaisService, PaisService>();
            services.AddTransient<IPaisEstadoService, PaisEstadoService>();
            services.AddTransient<ICiudadService, CiudadService>();
            services.AddTransient<IEmailTemplateService, EmailTemplateService>();
            services.AddTransient<IHIDOrigoEventService, HIDOrigoEventService>();
            services.AddTransient<ICorreoEnviarService, CorreoEnviarService>();
            services.AddTransient<IEncriptacionService, EncriptacionService>();
            services.AddTransient<IAccountService, AccountService>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IPasswordService, PasswordService>();
            services.AddSingleton<IAccesorService, AccesorService>();

            services.AddSingleton<IUriService>(provider =>
            {
                var accesor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accesor.HttpContext!.Request;
                var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());

                return new UriService(absoluteUri);
            });

            services.AddSingleton<TaskEventService>();
            services.AddHostedService<SSECleanupService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Authentication:Issuer"],
                    ValidAudience = configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:SecretKey"]!))
                };
            });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services, string xmlFileName)
        {
            string[] pagesApplication = {
                "Autenticación",
                "Cliente",
                "Sesión",
                "HID Origo",
                "Parametrización",
                "Organización",
                "Configuración",
                "Ubicación",
                "Mi cuenta"
            };

            services.AddSwaggerGen(c =>
            {
                foreach (string page in pagesApplication)
                {
                    c.SwaggerDoc(page, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = page.Replace("WebVisitsMobile", "WebVisitsMobile"),
                        Version = "v1",
                        Description = "Backend WebVisitsMobile",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                        {
                            Email = "proyectos2@crcdemexico.com.mx",
                            Name = "CRC de México S.A. de C.V.",
                            Url = new Uri("https://www.crcdemexico.com.mx/"),
                        },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense()
                        {
                            Name = "Licencia WebVisitsMobile",
                            Url = new Uri("http://crcdemexico.com.mx/webvisits/"),
                        }
                    });
                }

                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "Autenticación JWT (Bearer)",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id= "Bearer",
                                Type=ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebVisitsMobile.Api", Version = "v1" });
            });

            return services;
        }

        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Acccess-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Acccess-Control-Allow-Origin", "*");
        }
    }
}
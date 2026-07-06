using Newtonsoft.Json;
using WebVisitsMobile.Domain.Entities.Administracion.Sesion;
using WebVisitsMobile.Domain.Entities.Configuracion;
using WebVisitsMobile.Models.Encriptacion;
using WebVisitsMobile.Services.Interfaces.Encriptacion;

namespace WebVisitsMobile.Services.Services.Encriptacion
{
    public class EncriptacionService : IEncriptacionService
    {
        public EncriptacionService()
        { }

        public async Task<DesebcriptarDTO> EncriptarCadena(string cadena)
        {
            DesebcriptarDTO encriptar = new DesebcriptarDTO();

            EncriptadorAESService oEncriptador2 = new EncriptadorAESService();
            oEncriptador2.Configurar();
            oEncriptador2.GenerarClave();
            Thread.Sleep(300);
            oEncriptador2.GenerarIV();
            var clave = oEncriptador2.Get_IndiceClave().ToString();
            var iv = oEncriptador2.Get_IndiceIV().ToString();
            encriptar.L1 = Convert.ToInt32(clave);
            encriptar.L2 = Convert.ToInt32(iv);
            var cadenaEncriptada = oEncriptador2.Encriptar_String(cadena);
            encriptar.Cad = cadenaEncriptada;

            return encriptar;
        }

        public async Task<IntentosRecuperacion> DesencriptarIntentos(DesebcriptarDTO datos)
        {
            try
            {
                EncriptadorAESService oEncriptador = new EncriptadorAESService();
                oEncriptador.Configurar();
                oEncriptador.LoadClave(datos.L1);
                oEncriptador.LoadIV(datos.L2);
                var cadena = oEncriptador.Desencriptar_String(datos.Cad);
                IntentosRecuperacion objeto = JsonConvert.DeserializeObject<IntentosRecuperacion>(cadena);

                return objeto;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ClaveRecuperacion> DesencriptarClaveRecuperacion(DesebcriptarDTO datos)
        {
            try
            {
                EncriptadorAESService oEncriptador = new EncriptadorAESService();
                oEncriptador.Configurar();
                oEncriptador.LoadClave(datos.L1);
                oEncriptador.LoadIV(datos.L2);
                var cadena = oEncriptador.Desencriptar_String(datos.Cad);
                ClaveRecuperacion objeto = JsonConvert.DeserializeObject<ClaveRecuperacion>(cadena);

                return objeto;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<List<SettingsGroupTap>> DesencriptarCredential(DesebcriptarDTO datos)
        {
            try
            {
                EncriptadorAESService oEncriptador = new EncriptadorAESService();
                oEncriptador.Configurar();
                oEncriptador.LoadClave(datos.L1);
                oEncriptador.LoadIV(datos.L2);
                var cadena = oEncriptador.Desencriptar_String(datos.Cad);
                List<SettingsGroupTap> objeto = JsonConvert.DeserializeObject<List<SettingsGroupTap>>(cadena);

                return objeto;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using WebVisitsMobile.Models.Administracion.Seguridad.AlgoritmoAES;
using WebVisitsMobile.Services.Responses;
using WebVisitsMobile.Services.Services.Encriptacion;

namespace WebVisitsMobile.Controllers.Encriptacion
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Autenticación")]
    public class EncriptacionController : ControllerBase
    {
        EncriptadorAESService _encriptacionService = new EncriptadorAESService();
        public EncriptacionController(
            )
        {
        }

        [Route("DecryptedAES")]
        [HttpPost]
        public async Task<IActionResult> GetDecryptedAES(AlgoritmoAESDTO keys)
        {
            try
            {
                var key = await _encriptacionService.DecryptedAES(keys);
                if (key == null)
                {
                    return StatusCode(404, new ApiResponse<string>(false, "No se encontró información para descifrar", 404, ""));
                }
                var response = new ApiResponse<string>(true, "La solicitud se realizó correctamente", 200, key);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, "La solicitud no se realizó correctamente", 500, ""));
            }
        }
    }
}
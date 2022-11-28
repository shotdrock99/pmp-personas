using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.DAL.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/personas/cotizaciones/{codigoCotizacion:int}/[controller]")]
    [ApiController]
    public class FichaTecnicaController : ControllerBase
    {
        private readonly FichaTecnicaDataProvider fichaTecnicaDataBuilder;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FichaTecnicaController(IHttpContextAccessor httpContextAccessor)
        {
            this.fichaTecnicaDataBuilder = new FichaTecnicaDataProvider();
            this.httpContextAccessor = httpContextAccessor;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GenerarFichaTecnicaAsync(int codigoCotizacion, int version)
        {
            if (codigoCotizacion == 0)
            {
                return BadRequest("El código de la cotización es requerido.");
            }

            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            var response = await this.fichaTecnicaDataBuilder.GenerateAsync(codigoCotizacion, userName);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("test")]
        [HttpGet]
        public ActionResult TestMethod()
        {
            return Ok("Conexión exitosa a FichaTecnicaController :: TestMethod");
        }
    }
}
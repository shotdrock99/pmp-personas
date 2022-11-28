using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Route("api/v1/personas/cotizaciones/{codigoCotizacion:int}/[controller]")]
    [ApiController]
    public class InformacionNegocioController : ControllerBase
    {
        private readonly InformacionNegocioDataProvider informacionNegocioProvider;

        public InformacionNegocioController()
        {
            this.informacionNegocioProvider = new InformacionNegocioDataProvider();
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> InsertarInformacionNegocioAsync(int codigoCotizacion, int version, InformacionNegocio model)
        {
            var response = await informacionNegocioProvider.InsertarInformacionNegocioAsync(codigoCotizacion, version, model);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Entities;

namespace ModernizacionPersonas.Api.Controllers
{
    [Route("api/v1/personas/cotizaciones/{codigoCotizacion:int}/tomador")]
    [ApiController]
    public class InformacionTomadorController : ControllerBase
    {
        private readonly InformacionTomadorDataProvider tomadorDataProvider;

        public InformacionTomadorController()
        {
            this.tomadorDataProvider = new InformacionTomadorDataProvider();
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> InsertarDatosTomadorAsync(int codigoCotizacion, int version, Tomador model)
        {
            var response = await tomadorDataProvider.InsertarDatosTomadorAsync(codigoCotizacion, model);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

    }
}

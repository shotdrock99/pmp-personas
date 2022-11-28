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
    [Route("api/v1/personas/cotizaciones/{codigoCotizacion:int}/intermediarios")]
    [ApiController]
    public class InformacionIntermediarioController : ControllerBase
    {
        private readonly DatosIntermediariosProvider intermediariosDataProvider;

        public InformacionIntermediarioController()
        {
            this.intermediariosDataProvider = new DatosIntermediariosProvider();
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CrearIntermediarioAsync(int codigoCotizacion, int version, Intermediario model)
        {
            var response = await intermediariosDataProvider.InsertarDatosIntermediarioAsync(codigoCotizacion, version, model);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoIntermediario}")]
        [HttpPatch]
        public async Task<IActionResult> ActualizarDatosIntermediarioAsync(int codigoCotizacion, int version, Intermediario model)
        {
            // TODO Set CodigoCotizacion to model
            await intermediariosDataProvider.ActualizarDatosIntermediarioAsync(codigoCotizacion, version, model);
            return Ok();
            // return BadRequest(response.Message);
        }

        [Route("{codigoIntermediario}")]
        [HttpDelete]
        public async Task<IActionResult> EliminarIntermediarioAsync(int codigoIntermediario)
        {
            // TODO Set CodigoCotizacion to model
            await intermediariosDataProvider.EliminarIntermediarioAsync(codigoIntermediario);
            return Ok();
            // return BadRequest(response.Message);
        }
    }
}

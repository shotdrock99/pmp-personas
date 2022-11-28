using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.Api.Providers;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Entities;
using System;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/personas/cotizaciones/{codigoCotizacion:int}/[controller]")]
    [ApiController]
    public class ResumenController : ControllerBase
    {
        private readonly ResumenCotizacionProvider resumenProvider;

        public ResumenController(IHttpContextAccessor httpContextAccessor)
        {
            this.resumenProvider = new ResumenCotizacionProvider();
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GenerarResumenAsync(int codigoCotizacion, int version)
        {
            if (codigoCotizacion <= 0)
            {
                return BadRequest("El código de la cotización es requerido.");
            }

            try
            {
                var response = await this.resumenProvider.GenerateAsync(codigoCotizacion, version);
                if (response.Status == ResponseStatus.Valid)
                {
                    return Ok(response);
                }

                return BadRequest(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                //return BadRequest($"Se presento un error generando el resumen de la cotización con codigo {codigoCotizacion}.");
                throw new Exception("", ex);
            }
        }        

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> GuardarDatosResumen(int codigoCotizacion, int version, GuardarResumenArgs1 args)
        {
            try
            {
                var response = await this.resumenProvider.InsertarTasaOpcionAsync(codigoCotizacion, version, args);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("test")]
        [HttpGet]
        public ActionResult TestMethod()
        {
            return Ok("Conexión exitosa a ResumenController :: TestMethod");
        }
    }
}
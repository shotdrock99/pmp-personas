using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.Api.Entities;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.SISEServices;
using ModernizacionPersonas.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/personas/cotizaciones/{codigoCotizacion:int}/[controller]")]
    [ApiController]
    public class GruposAseguradosController : ControllerBase
    {
        private readonly DatosGruposAseguradosProvider datosGruposAseguradosProvider;
        private readonly DatosGruposAseguradosMapper gruposAseguradosMapper;
        private readonly SISEAseguradosWriter SISEWriterService;
        private readonly DatosGrupoAseguradoAseguradosUploader aseguradosUploader;

        public GruposAseguradosController(IHttpContextAccessor httpContextAccessor)
        {
            this.datosGruposAseguradosProvider = new DatosGruposAseguradosProvider();
            this.gruposAseguradosMapper = new DatosGruposAseguradosMapper();
            this.SISEWriterService = new SISEAseguradosWriter();
            this.aseguradosUploader = new DatosGrupoAseguradoAseguradosUploader();
        }

        [Route("{codigoGrupoAsegurado:int}")]
        [HttpGet]
        public async Task<IActionResult> GetGrupoAseguradosAsync(int codigoCotizacion, int version, int codigoGrupoAsegurado)
        {
            var response = await gruposAseguradosMapper.ConsultarGrupoAseguradoAsync(codigoCotizacion, version, codigoGrupoAsegurado);
            if (response != null)
            {
                return Ok(response);
            }

            return NotFound("No se encontró información de un grupo de asegurados con ese código.");
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateGrupoAseguradosAsync(int codigoCotizacion, int version, GrupoAsegurado args)
        {
            var response = await this.datosGruposAseguradosProvider.CreateGrupoAseguradoAsync(codigoCotizacion, version, args);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest("Hubo un error tratando de ejecutar el método.");
        }

        [Route("{codigoGrupoAsegurado:int}")]
        [HttpPatch]
        public async Task<IActionResult> PatchGrupoAseguradosAsync(int codigoCotizacion, int version, int codigoGrupoAsegurado, GrupoAsegurado model)
        {
            try
            {
                if (model.CodigoGrupoAsegurado > 0)
                {
                    await this.datosGruposAseguradosProvider.UpdateGrupoAseguradoAsync(codigoCotizacion, model);
                }

                var response = await this.datosGruposAseguradosProvider.InsertarValoresGrupoAseguradoAsync(codigoCotizacion, version, model);
                if (response.Status == ResponseStatus.Valid)
                {
                    return Ok(response);
                }

                return BadRequest(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(new { type = ex.Message, ex.InnerException.Message });
            }
        }

        [Route("{codigoGrupoAsegurado:int}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteGrupoAseguradosAsync(int codigoCotizacion, int codigoGrupoAsegurado)
        {
            var response = await this.datosGruposAseguradosProvider.EliminarGrupoAseguradoAsync(codigoCotizacion, codigoGrupoAsegurado);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoGrupoAsegurado:int}/asegurados")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAseguradosAsync(int codigoCotizacion, int codigoGrupoAsegurado)
        {
            var response = await this.datosGruposAseguradosProvider.EliminarAseguradosAsync(codigoCotizacion, codigoGrupoAsegurado);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoGrupoAsegurados:int}/asegurados/upload")]
        [HttpPost]
        public async Task<IActionResult> UploadAseguradosAsync(int codigoCotizacion, int codigoGrupoAsegurados, int numeroSalarios, decimal valorMin, decimal valorMax, int edadMinimaAsegurado, int edadMaximaAsegurado, int edadPermanenciaAsegurado, int tipoEstructura)
        {
            try
            {
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var file = HttpContext.Request.Form.Files[0];
                    var response = await this.aseguradosUploader.UploadAseguradosAsync(codigoCotizacion, codigoGrupoAsegurados, numeroSalarios, valorMin, valorMax, edadMinimaAsegurado, edadMaximaAsegurado, file, tipoEstructura);
                    if (response.Status == ResponseStatus.Valid)
                    {
                        var registrosProcesados = response.RegistrosProcesados;
                        var registrosDuplicados = response.RegistrosDuplicados;

                        return Ok(new UploadAseguradosResponseViewModel
                        {
                            TotalAsegurados = response.TotalAsegurados,
                            TotalRegistros = response.TotalRegistros,
                            RegistrosTotales = response.TotalRegistros,
                            RegistrosProcesados = registrosProcesados,
                            RegistrosDuplicados = registrosDuplicados,
                            EdadPromedio = response.EdadPromedio,
                            ValorAsegurado = response.ValorAsegurado,
                            PorcentajeAsegurados = 100,
                            RegistrosFallidos = response.TotalRegistros - registrosProcesados,
                            WithErrors = response.Errores.Count() > 0,
                            ErrorSummary = response.Errores.ToList(),
                        });
                    }
                }
                return BadRequest("No hay archivos para importar.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        #region SISE SP 

        [Route("sise/asegurados")]
        [HttpPost]
        public async Task<IActionResult> InsertarAseguradosSISE(SISEListadoAseguradosArgs args)
        {
            try
            {
                var responseSISE = await this.SISEWriterService.InsertarAseguradosAsync(args);
                return Ok(responseSISE);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        [Route("test")]
        [HttpGet]
        public ActionResult TestMethod()
        {
            return Ok("Conexión exitosa a GruposAseguradosController :: TestMethod");
        }
    }
}
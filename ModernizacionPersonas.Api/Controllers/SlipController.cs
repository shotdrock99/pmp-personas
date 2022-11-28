using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.Api.Entities;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/personas/cotizaciones/{codigoCotizacion:int}/[controller]")]
    [ApiController]
    public class SlipController : ControllerBase
    {
        private readonly SlipDataProvider slipDataProvider;
        private readonly ConfiguracionSlipDataProvider configuracionSlipDataProvider;
        private readonly IDatosSlipWriter datosSlipWriterService;
        private readonly CotizacionDataProvider cotizacionDataProvider;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SlipController(IHttpContextAccessor httpContextAccessor)
        {
            this.slipDataProvider = new SlipDataProvider();
            this.configuracionSlipDataProvider = new ConfiguracionSlipDataProvider(httpContextAccessor);
            this.datosSlipWriterService = new DatosSlipTableWriter();
            this.cotizacionDataProvider = new CotizacionDataProvider();
            this.httpContextAccessor = httpContextAccessor;
        }

        [Route("configuracion")]
        [HttpGet]
        public async Task<IActionResult> ObtenerConfiguracionSlipAsync(int codigoCotizacion, int version)
        {
            if (codigoCotizacion == 0)
            {
                return BadRequest("El código de la cotización es requerido.");
            }

            var response = await this.configuracionSlipDataProvider.GenerateConfiguracionSlipAsync(codigoCotizacion, version);

            //Eliminar variable Anticipo de Enfermedades Graves
            foreach (var amparo in response.Data.Amparos)
            {
                if (amparo.CodigoAmparo == 3)
                {
                    foreach (var variable in amparo.Variables.ToArray())
                    {
                        if (variable.CodigoVariable == 2)
                        {
                            amparo.Variables.Remove(variable);
                        }
                    }
                }
            }

            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("configuracion")]
        [HttpPost]
        public async Task<IActionResult> GuardarConfiguracionSlipAsync([FromBody] SlipConfiguracion model)
        {
            var response = await this.configuracionSlipDataProvider.GuardarVariablesSlipAsync(model);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok();
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("preview")]
        [HttpGet]
        public async Task<IActionResult> ObtenerDatosPreviewSlipAsync(int codigoCotizacion, int version)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            var response = await this.slipDataProvider.GenerateSlipAsync(codigoCotizacion, version, userName);

            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }
        [Route("previewPDF")]
        [HttpPost]
        public async Task<IActionResult> ObtenerDatosPreviewSlipPDFAsync(int codigoCotizacion, [FromBody] SendSlipArgs args)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var response1 = await this.slipDataProvider.GetCotizacionSlipPDFAsync(args, userName);
            return Ok(response1);
        }

        //[Route("save")]
        //[HttpPost]
        //public async Task<IActionResult> SaveCotizacionSlipAsync(int codigoCotizacion)
        //{
        //    var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
        //    // var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
        //    var httpRequest = HttpContext.Request;
        //    if (httpRequest.Form.Files.Count > 0)
        //    {
        //        var file = httpRequest.Form.Files[0];
        //        var response = await this.slipDataProvider.SaveCotizacionSlipAsync(codigoCotizacion, file, userName);
        //        if (response.Status == ResponseStatus.Valid)
        //        {
        //            return Ok(response);
        //        }

        //        return BadRequest("Error cargando PDF de Slip.");
        //    }

        //    return BadRequest("No hay archivos para importar.");
        //}

        [Route("saveAttachment")]
        [HttpPost]
        public async Task<IActionResult> SaveAttachmentSlipAsync(int codigoCotizacion)
        {
            var httpRequest = HttpContext.Request;
            if (httpRequest.Form.Files.Count > 0)
            {
                var file = httpRequest.Form.Files[0];
                var response = await this.slipDataProvider.SaveAttachmentsSlipAsync(codigoCotizacion, file);
                if (response.Status == ResponseStatus.Valid)
                {
                    return Ok(response);
                }

                return BadRequest("Error cargando adjunto del Slip.");
            }

            return BadRequest("No hay archivos para importar.");
        }

        [Route("send")]
        [HttpPost]
        public async Task<IActionResult> SendCotizacionSlipAsync(int codigoCotizacion, [FromBody] SendSlipArgs args)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            if (!args.Resend)
            {
                await this.slipDataProvider.SaveCotizacionSlipAsync(args.CodigoCotizacion, userName, args.NumeroCotizacion, args.Version);
            }
            var response1 = await this.slipDataProvider.SendCotizacionSlipAsync(args, userName);
            if (!response1.HasError)
            {
                return Ok(response1);
            }


            return BadRequest(response1.DescripcionEstado);
        }

        [Route("configuracion/tasa")]
        [HttpPost]
        public async Task<ActionResult> UpdateTasaCotizacion(int codigoCotizacion, int version, UpdateCotizacionDataArgs args)
        {
            var response = await this.cotizacionDataProvider.UpdateSelectedTasaAsync(codigoCotizacion, version, args);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok();
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("configuracion/asegurabilidad")]
        [HttpPost]
        public async Task<ActionResult> CreateAsegurabilidadAsync(int codigoCotizacion, Asegurabilidad asegurabilidad)
        {

            var response = await this.configuracionSlipDataProvider.GuardarAsegurabilidadAsync(asegurabilidad, codigoCotizacion);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("configuracion/asegurabilidad/{codigoAsegurabilidad:int}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteAsegurabilidadAsync(int codigoCotizacion, int codigoAsegurabilidad)
        {

            var response = await this.configuracionSlipDataProvider.EliminarAsegurabilidadAsync(codigoCotizacion, codigoAsegurabilidad);
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
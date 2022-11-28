using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/personas/cotizaciones/{codigoCotizacion:int}/[controller]")]
    [ApiController]
    public class AuthorizationsController : ControllerBase
    {
        private readonly CotizacionDataProvider cotizacionDataProvider;
        private readonly AuthorizationControlsAuthorizer cotizacionAuthorizer;
        private readonly CotizacionAuthorizationProvider authorizationProvider;
        private readonly CotizacionTransactionsProvider cotizacionTransactionsProvider;
        private readonly AuthorizationAttachmentProvider authorizationAttachmentUploader;
        private readonly InformacionNegocioDataProvider informacionNegocioProvider;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthorizationsController(IHttpContextAccessor httpContextAccessor, IHostingEnvironment env)
        {
            var rootPath = env.ContentRootPath;

            this.httpContextAccessor = httpContextAccessor;

            this.cotizacionDataProvider = new CotizacionDataProvider();
            this.cotizacionAuthorizer = new AuthorizationControlsAuthorizer();
            this.authorizationProvider = new CotizacionAuthorizationProvider();
            this.cotizacionTransactionsProvider = new CotizacionTransactionsProvider();
            this.authorizationAttachmentUploader = new AuthorizationAttachmentProvider(rootPath);
            this.informacionNegocioProvider = new InformacionNegocioDataProvider();
        }

        [Route("~/api/v1/personas/cotizaciones/authorizations")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaCotizacionesAsync([FromQuery] CotizacionFilter filterArgs)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userRole = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.RoleId).Value;
            //var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            //var sucursalId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.SucursalId).Value;
            // TODO enviar codigo de la agencia a la que pertence el usuario logeado
            // validar con servicio de soligespro
            //filterArgs.CodigoSucursal = "376";
            filterArgs.CodigoEstado = 1111;
            filterArgs.CodigoUsuario = userName;            

            var data = await this.authorizationProvider.GetCotizacionesPorAutorizarAsync(userName, userRole, filterArgs);
            return Ok(data);
            // return BadRequest(response.Message);
        }

        [Route("~/api/v1/personas/cotizaciones/authorizations/{codigoCotizacion:int}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerCotizacionAsync(int codigoCotizacion)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            var sucursalId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.SucursalId).Value;

            var data = await this.cotizacionDataProvider.FetchCotizacionAsync(codigoCotizacion, 0);
            return Ok(data);
            // return BadRequest(response.Message);
        }

        [Route("controls")]
        [HttpGet]
        public async Task<IActionResult> GetCotizacionAuthorizationsAsync(int codigoCotizacion, [FromQuery]int version)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;

            var cotizaciones = await this.authorizationProvider.GetAuthorizationInfoAsync(codigoCotizacion, userName);
            return Ok(cotizaciones);
            // return BadRequest(response.Message);
        }

        [Route("users")]
        [HttpGet]
        public async Task<IActionResult> GetAuthorizationsUsersAsync(int codigoCotizacion, [FromQuery]int version)
        {
            var data = await this.authorizationProvider.GetAuthorizationsUsersAsync(codigoCotizacion, version);
            return Ok(data);
            // return BadRequest(response.Message);
        }

        [Route("transactions")]
        [HttpGet]
        public async Task<IActionResult> GetTransactionsAsync(int codigoCotizacion, [FromQuery]int version)
        {
            var data = await this.cotizacionTransactionsProvider.GetAuthorizationTransactionsAsync(codigoCotizacion, version);
            return Ok(data);
            // return BadRequest(response.Message);
        }

        // TODO remove anonymous
        [AllowAnonymous]
        [Route("soportes/{transactionId:int}/download")]
        [HttpGet]
        public async Task<IActionResult> DownloadAuthorizationAttachmentAsync(int codigoCotizacion, int transactionId, [FromQuery]int version)
        {
            try
            {
                var zip = await this.authorizationAttachmentUploader.CreateCompressedFile(codigoCotizacion, version, transactionId);
                var fileName = $"attachments_{codigoCotizacion}_{version}_{transactionId}.zip";
                var result = new HttpResponseMessage(HttpStatusCode.OK);

                MemoryStream stream = new MemoryStream();
                stream.Write(zip, 0, zip.Length);

                stream.Position = 0;

                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileName };
                //return result;

                return File(stream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Route("soportes/upload")]
        [HttpPost]
        public async Task<IActionResult> UploadAuthorizationAttachmentAsync(int codigoCotizacion, int version)
        {
            var userName = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var httpRequest = HttpContext.Request;
            if (httpRequest.Form.Files.Count > 0)
            {
                var file = httpRequest.Form.Files[0];
                var data = await this.authorizationAttachmentUploader.UploadAsync(codigoCotizacion, version, userName, file);
                return Ok(data);
                // response.Message = $"Error cargando el archivo. Detalles: {response.Message}";
                // return BadRequest(response);
            }

            return BadRequest("No hay archivos para importar.");
        }

        [Route("soportes/saveAttachmentToAuth")]
        [HttpPost]
        public async Task<IActionResult> SaveAttachmentToAuthAsync(int codigoCotizacion)
        {
            var httpRequest = HttpContext.Request;
            if (httpRequest.Form.Files.Count > 0)
            {
                var file = httpRequest.Form.Files[0];
                var response = await this.cotizacionDataProvider.SaveAttachmentsAuthAsync(codigoCotizacion, file);
                if (response.Status == ResponseStatus.Valid)
                {
                    return Ok(response);
                }

                return BadRequest("Error cargando adjunto del Slip.");
            }

            return BadRequest("No hay archivos para importar.");
        }

        [Route("notify")]
        [HttpPost]
        public async Task<IActionResult> NotifyCotizacionAsync(int codigoCotizacion, int version, NotifyCotizacionArgs args)
        {
            var userName = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            args.UserName = userName;

            var response = await this.cotizacionAuthorizer.NotifyAuthorizationControlsAsync(codigoCotizacion, args);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }


        [Route("authorize")]
        [HttpPost]
        public async Task<IActionResult> AutorizarCotizacionAsync(int codigoCotizacion, int version, AuthorizationArgs args)
        {
            //var userName = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            //args.UserName = userName;

            var response = await this.cotizacionAuthorizer.ConfirmAuthorizationControlsAsync(codigoCotizacion, version, args);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("applyChanges")]
        [HttpPost]
        public async Task<IActionResult> ApplyChangesAsync(ChangesArgs args)
        {
            var response = await this.informacionNegocioProvider.UpdateCompanyData(args.CodigoCotizacion, args.Version, args.GastosCompania, args.UtilidadesCompania);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }
    }
}
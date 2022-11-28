using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.Api.Entities;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/personas/[controller]")]
    [ApiController]
    public class CotizacionesController : ControllerBase
    {
        private readonly CotizacionDataProvider cotizacionDataProvider;
        private readonly CotizacionDataValidator cotizacionValidator;
        private readonly CotizacionTransactionsProvider cotizacionTransactionProvider;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly CotizacionAttachmentProvider cotizacionAttachmentProvider;
        private readonly DatosSiniestralidadProvider siniestralidadPersonasWriter;
        private readonly ExpedicionWebProvider expedicionWebProvider;

        public CotizacionesController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;

            this.cotizacionDataProvider = new CotizacionDataProvider();
            this.cotizacionValidator = new CotizacionDataValidator();
            this.cotizacionTransactionProvider = new CotizacionTransactionsProvider();
            this.cotizacionAttachmentProvider = new CotizacionAttachmentProvider();
            this.siniestralidadPersonasWriter = new DatosSiniestralidadProvider();
            this.expedicionWebProvider = new ExpedicionWebProvider();
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaCotizacionesAsync([FromQuery] CotizacionFilter filterArgs)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            var sucursalId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.SucursalId).Value;
            var roleId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.RoleId).Value;
            var zonaId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.ZonaId).Value;
            // TODO enviar codigo de la agencia a la que pertence el usuario logeado
            // validar con servicio de soligespro
            //filterArgs.CodigoSucursal = !string.IsNullOrEmpty(filterArgs.CodigoSucursal) ? filterArgs.CodigoSucursal : sucursalId;
            //if (string.IsNullOrEmpty(sucursalId) || sucursalId == "800")
            //{
            //    filterArgs.CodigoSucursal = null;
            //}

            //if (sucursalId == "800")
            //{
            //    if (string.IsNullOrEmpty(filterArgs.CodigoSucursal))
            //    {
            //        filterArgs.CodigoSucursal = null;
            //    }
            //    else
            //    {
            //        filterArgs.CodigoSucursal = filterArgs.CodigoSucursal;
            //    }
            //}
            //else
            //{
            //    filterArgs.CodigoSucursal = !string.IsNullOrEmpty(filterArgs.CodigoSucursal) ? filterArgs.CodigoSucursal : sucursalId;
            //}

            if (sucursalId == "800")
            {
                if (zonaId == "0")
                {
                    if (string.IsNullOrEmpty(filterArgs.CodigoSucursal))
                    {
                        filterArgs.CodigoSucursal = null;
                    }
                }
                else
                {
                    filterArgs.CodigoZona = Int32.Parse(zonaId);
                }
            }
            else
            {
                filterArgs.CodigoSucursal = string.IsNullOrEmpty(sucursalId) ? filterArgs.CodigoSucursal : sucursalId;
            }

            // filterArgs.CodigoUsuario = userId;            
            var data = await this.cotizacionDataProvider.GetCotizacionesAsync(filterArgs);
            data = data.Where(x => x.Version != 777);
            return Ok(data);
            // return BadRequest(response.Message);
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CrearCotizacionAsync(CrearCotizacionArgs model)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            // update model
            model.UserId = userId;
            model.UserName = userName;
            var response = await this.cotizacionDataProvider.InitializeCotizacionAsync(model);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoCotizacion:int}")]
        [HttpGet]
        public async Task<IActionResult> OpenCotizacionAsync(int codigoCotizacion, int version)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;

            var response = await this.cotizacionDataProvider.OpenCotizacionAsync(codigoCotizacion, version, userName, int.Parse(userId));
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoCotizacion:int}/copy")]
        [HttpPost]
        public async Task<IActionResult> CopyCotizacionAsync(int codigoCotizacion, int version)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            var response = await this.cotizacionDataProvider.CopyCotizacionAsync(int.Parse(userId), codigoCotizacion, version, userName);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoCotizacion:int}/copyalt")]
        [HttpPost]
        public async Task<IActionResult> CopyAltCotizacionAsync(int codigoCotizacion, int version)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            var response = await this.cotizacionDataProvider.CopyAltCotizacionAsync(int.Parse(userId), codigoCotizacion, version);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoCotizacion:int}/version")]
        [HttpPost]
        public async Task<IActionResult> CreateVersionCotizacionAsync(int codigoCotizacion, int version)
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            var response = await this.cotizacionDataProvider.CreateVersionCotizacionAsync(int.Parse(userId), codigoCotizacion, version, userName);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoCotizacion:int}/validate")]
        [HttpGet]
        public async Task<IActionResult> ValidateCotizacionAsync(int codigoCotizacion, int version, int flag)
        {
            var user = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var response = await this.cotizacionValidator.ValidateAsync(user, codigoCotizacion, version, flag);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoCotizacion:int}/continue")]
        [HttpPost]
        public async Task<IActionResult> ContinueCotizacionAsync(int codigoCotizacion, ContinueCotizacionArgs args)
        {
            var userName = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            var response = await this.cotizacionDataProvider.ContinueCotizacionAsync(codigoCotizacion, int.Parse(userId), userName, args.Reason);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoCotizacion:int}/confirm")]
        [HttpPost]
        public async Task<IActionResult> ConfirmCotizacionAsync(int codigoCotizacion, ConfirmCotizacionArgs args)
        {
            var user = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            args.UserId = int.Parse(userId);
            args.UserName = user;
            var response = await this.cotizacionDataProvider.ConfirmCotizacionAsync(codigoCotizacion, args);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoCotizacion:int}/lock")]
        [HttpPost]
        public async Task<IActionResult> LockCotizacionAsync(int codigoCotizacion, int version)
        {
            var userName = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserId).Value;
            var response = await this.cotizacionDataProvider.LockCotizacionAsync(codigoCotizacion, version, int.Parse(userId), userName);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoCotizacion:int}/unlock")]
        [HttpPost]
        public async Task<IActionResult> UnlockCotizacionAsync(int codigoCotizacion, int version)
        {
            var user = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var response = await this.cotizacionDataProvider.UnlockCotizacionAsync(user, codigoCotizacion, version);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoCotizacion:int}/confirmacion/soportes/upload")]
        [HttpPost]
        public async Task<IActionResult> UploadConfirmacionCotizacionAttachmentAsync(int codigoCotizacion, int version)
        {
            var userName = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            var httpRequest = HttpContext.Request;
            if (httpRequest.Form.Files.Count > 0)
            {
                var file = httpRequest.Form.Files[0];
                var response = await this.cotizacionAttachmentProvider.UploadAsync(codigoCotizacion, file);
                if (response.Status == ResponseStatus.Valid)
                {
                    response.ErrorMessage = "El archivo fue cargado exitosamente.";
                    return Ok(response);
                }

                response.ErrorMessage = $"Error cargando el archivo. Detalles: {response.ErrorMessage}";
                return BadRequest(response);
            }

            return BadRequest("No hay archivos para importar.");
        }

        [Route("{codigoCotizacion:int}/transactions")]
        [HttpGet]
        public async Task<IActionResult> GetCotizacionTransactionsAsync(int codigoCotizacion, [FromQuery] int version)
        {
            var data = await this.cotizacionTransactionProvider.GetTransactionsAsync(codigoCotizacion, version);
            return Ok(data);
            // return BadRequest(response.Message);
        }

        [Route("{codigoCotizacion:int}/siniestralidad")]
        [HttpPost]
        public async Task<IActionResult> InsertarSiniestralidadAsync(int codigoCotizacion, int version, InsertSiniestralidadArgs model)
        {
            var response = await this.siniestralidadPersonasWriter.InsertarSiniestralidadCotizacionAsync(codigoCotizacion, version, model.Data);
            if (response.Status == ResponseStatus.Valid)
            {
                return Ok(response);
            }

            return BadRequest(response.ErrorMessage);
        }

        [Route("{codigoCotizacion:int}/firmas")]
        [HttpGet]
        public async Task<IActionResult> GetFirmasAsync(int codigoCotizacion)
        {
            try
            {
                var firmas = await this.cotizacionDataProvider.GetFirmasRechazoAceptacion(codigoCotizacion);
                return Ok(firmas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{codigoCotizacion:int}/expedicion")]
        [HttpPost]
        public async Task<IActionResult> GenerateExpedicionWeb(int codigoCotizacion, ExpedicionArgs expedicionArgs)
        {
            try
            {
                var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.expedicionWebProvider.GenerateExpedicionAsync(codigoCotizacion, expedicionArgs, userName);
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
            return Ok("Conexión exitosa a CotizacionController :: TestMethod");
        }
    }
}
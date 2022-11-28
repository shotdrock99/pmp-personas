using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Entities;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/personas/parametrizacion/slip/secciones")]
    [ApiController]
    public class SeccionesSlipController : ControllerBase
    {
        private readonly SeccionesSlipDataProvider seccionesSlipDataProvider;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SeccionesSlipController()
        {
            this.seccionesSlipDataProvider = new SeccionesSlipDataProvider();
            this.httpContextAccessor = new HttpContextAccessor();
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetSeccionesSlip()
        {
            try
            {
                var seccionesSlip = await this.seccionesSlipDataProvider.GetSeccionesSlipAsync();
                return Ok(seccionesSlip);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateSeccionSlip([FromBody] SeccionSlip seccionSlip)
        {
            try
            {
                seccionSlip.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.seccionesSlipDataProvider.CreateSeccionSlipAsync(seccionSlip);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{codigo:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateSeccionSlip([FromBody] SeccionSlip seccionSlip)
        {
            try
            {
                seccionSlip.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.seccionesSlipDataProvider.UpdateSeccionSlipAsync(seccionSlip);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }        
    }
}

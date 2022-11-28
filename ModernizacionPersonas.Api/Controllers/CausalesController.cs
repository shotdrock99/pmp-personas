using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/personas/[controller]")]
    [ApiController]
    public class CausalesController : ControllerBase
    {
        private readonly CausalesProvider causalesProvider;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CausalesController()
        {
            this.causalesProvider = new CausalesProvider();
            this.httpContextAccessor = new HttpContextAccessor();
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetCausalesAsync()
        {
            try
            {
                var causales = await this.causalesProvider.GetCausales();
                return Ok(causales);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostCausalAsync([FromBody] Causal causal)
        {
            try
            {
                causal.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.causalesProvider.PostCausalAsync(causal);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("{codigoCausal:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateCausalAsyn(int codigoCausal, [FromBody] Causal causal)
        {
            try
            {
                causal.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.causalesProvider.UpdateCausal(causal);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{codigoCausal:int}")]
        [HttpDelete]
        public async Task<IActionResult> DisableCausalAsync(int codigoCausal)
        {
            try
            {
                var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.causalesProvider.DisableCausal(codigoCausal, userName);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

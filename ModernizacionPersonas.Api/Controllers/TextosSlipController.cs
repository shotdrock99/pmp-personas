using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Entities;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/personas/parametrizacion/slip/textos")]
    [ApiController]
    public class TextosSlipController : ControllerBase
    {
        private readonly ITextosSlipDataProvider provider;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TextosSlipController()
        {
            this.provider = new TextosSlipDataProvider();
            this.httpContextAccessor = new HttpContextAccessor();
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetTextosSlip()
        {
            try
            {
                var result = await this.provider.GetTextosSlipAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateTextoSlip([FromBody] TextoSlip model)
        {
            try
            {
                model.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.provider.CreateTextoSlipAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{codigo:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateTextoSlip([FromBody] TextoSlip model)
        {
            try
            {
                model.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.provider.UpdateTextoSlipAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

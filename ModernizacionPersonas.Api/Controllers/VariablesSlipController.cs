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
    [Route("api/v1/personas/parametrizacion/slip/variables")]
    [ApiController]
    public class VariablesSlipController : ControllerBase
    {
        private readonly VariablesParametrizacionDataProvider variablesParametrizacionDataProvider;
        private readonly IHttpContextAccessor httpContextAccessor;

        public VariablesSlipController()
        {
            this.variablesParametrizacionDataProvider = new VariablesParametrizacionDataProvider();
            this.httpContextAccessor = new HttpContextAccessor();
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetVariablesSlip()
        {
            try
            {
                var variablesSlip = await this.variablesParametrizacionDataProvider.GetVariablesSlipAsync();
                return Ok(variablesSlip);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("unused")]
        [HttpGet]
        public async Task<IActionResult> GetUnusedVariablesSlip()
        {
            try
            {
                var variablesSlip = await this.variablesParametrizacionDataProvider.GetUnusedVariablesSlipAsync();
                return Ok(variablesSlip);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{codigoTexto:int}")]
        [HttpGet]
        public async Task<IActionResult> GetVariablesSlipByCodigoTexto(int codigoTexto)
        {
            try
            {
                var variablesSlip = await this.variablesParametrizacionDataProvider.GetVariablesSlipByCodigoTextoAsync(codigoTexto);
                return Ok(variablesSlip);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{codigo:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateSeccionSlip([FromBody] VariableSlipParametrizacion variableSlip)
        {
            try
            {
                variableSlip.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.variablesParametrizacionDataProvider.UpdateVariableSlipAsync(variableSlip);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateSeccionSlip([FromBody] VariableSlipParametrizacion variableSlip)
        {
            try
            {
                variableSlip.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.variablesParametrizacionDataProvider.CreateVariableSlipAsync(variableSlip);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

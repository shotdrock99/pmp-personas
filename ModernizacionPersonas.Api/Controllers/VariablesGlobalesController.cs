using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Entities;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/personas/[controller]")]
    [ApiController]
    public class VariablesGlobalesController : ControllerBase
    {
        private readonly VariablesGlobalesDataProvider variablesGlobalesDataProvider;

        public VariablesGlobalesController()
        {
            this.variablesGlobalesDataProvider = new VariablesGlobalesDataProvider();
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetVariablesGlobalesAsync()
        {
            try
            {
                var variablesGlobales = await this.variablesGlobalesDataProvider.GetVariablesGlobalesAsync();
                return Ok(variablesGlobales);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{codigoVariable:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateVariableGlobalAsync([FromBody] ParametrizacionApp variable)
        {
            try
            {
                var response = await this.variablesGlobalesDataProvider.UpdateVariableGlobalAsync(variable);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

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
    [Route("api/v1/personas/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly EmailsDataProvider emailsDataProvider;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SolidariaExchangeEmailSender SolidariaExchangeEmailSender;

        public EmailsController()
        {
            this.emailsDataProvider = new EmailsDataProvider();
            this.httpContextAccessor = new HttpContextAccessor();
            this.SolidariaExchangeEmailSender = new SolidariaExchangeEmailSender();
        }

        [Route("{codigoTemplate:int}/{codigoSeccion:int}")]
        [HttpGet]
        public async Task<IActionResult> GetTextosEmailByTemplate(int codigoTemplate, int codigoSeccion)
        {
            try
            {
                var textoTemplate = await this.emailsDataProvider.GetTextosEmailByTemplate(codigoTemplate, codigoSeccion);
                return Ok(textoTemplate);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{codigoTemplate:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateTextoEmail([FromBody] EmailParametrizacion email)
        {
            try
            {
                email.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.emailsDataProvider.UpdateTextoEmailAsync(email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("testEmail")]
        [HttpPost]
        public async Task<IActionResult> TestEmail()
        {
            try {

                //await this.SolidariaExchangeEmailSender.EnvioCorreosLocal();
                return Ok("OK");
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

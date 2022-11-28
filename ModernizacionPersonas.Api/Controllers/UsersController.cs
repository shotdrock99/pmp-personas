using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/personas/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationUserDataProvider usersProvider;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UsersController()
        {
            this.usersProvider = new ApplicationUserDataProvider();
            this.httpContextAccessor = new HttpContextAccessor();
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            try
            {
                var users = await this.usersProvider.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{userId:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateUsuarioAsync([FromBody] ApplicationUser usuario)
        {
            try
            {
                usuario.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.usersProvider.UpdateUserAsync(usuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateUsuarioAsync([FromBody] ApplicationUser usuario)
        {
            try
            {
                usuario.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.usersProvider.CreateUserAsync(usuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("intermediario")]
        [HttpPost]
        public async Task<IActionResult> CreateIntermediarioAsync([FromBody] ApplicationUser usuario)
        {
            try
            {
                usuario.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.usersProvider.CreateUserAsync(usuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{userId:int}")]
        [HttpDelete]
        public async Task<IActionResult> DisableUserAsync(int userId)
        {
            try
            {
                var userName = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                var response = await this.usersProvider.DisableUserAsync(userId, userName);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{userName}/validate")]
        [HttpGet]
        public async Task<IActionResult> ValidateUserAsync(string userName)
        {
            try
            {
                var user = await this.usersProvider.ValidateUserAsync(userName);
                if (user.ApplicationUser == null)
                {
                    return BadRequest(user);
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("intermediario/{codigoIntermediario}/validate")]
        [HttpGet]
        public async Task<IActionResult> ValidateIntermediario(int codigoIntermediario)
        {
            try
            {
                var user = await this.usersProvider.ValidateIntermediario(codigoIntermediario);
                if (user.ApplicationUser == null)
                {
                    return BadRequest(user);
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
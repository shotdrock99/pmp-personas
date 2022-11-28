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
    public class RolesController : ControllerBase
    {

        private readonly RolesDataProvider rolesDataProvider;
        private readonly PermisosDataProvider permisosDataProvider;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RolesController()
        {
            this.rolesDataProvider = new RolesDataProvider();
            this.permisosDataProvider = new PermisosDataProvider();
            this.httpContextAccessor = new HttpContextAccessor();
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetRolesAsync()
        {
            try
            {
                var roles = await this.rolesDataProvider.GetRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("{codigo:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateRolAsync([FromBody] Rol rol)
        {
            try
            {
                rol.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                await this.rolesDataProvider.UpdateRolAsync(rol);
                await this.permisosDataProvider.UpdatePermisosRolAsync(rol.Permisos, rol.Codigo);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateRolAsync([FromBody] Rol rol)
        {
            try
            {
                rol.Usuario = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
                await this.rolesDataProvider.CreateRolAsync(rol);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("permisos")]
        [HttpGet]
        public async Task<IActionResult> GetPermisosAsync()
        {
            try
            {
                var response = await this.permisosDataProvider.GetPermisosAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
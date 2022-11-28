using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModernizacionPersonas.Api.Entities;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HttpClient httpClient;
        private readonly ApplicationUserDataProvider usersProvider;
        private readonly UtilidadesSolidariaService utilidadesSolidariaService;
        private readonly AdministracionPersonasReader administracionPersonasService;
        private readonly IOptions<ApplicationConfig> appConfig;
        private readonly IDatosPermisosRolReader permissionsReader;

        public AuthenticationController(IOptions<ApplicationConfig> appConfig, IHttpContextAccessor httpContextAccessor)
        {
            this.appConfig = appConfig;
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy(appConfig.Value.ProxyURL, false),
                UseProxy = true
            };

            this.httpContextAccessor = httpContextAccessor;
            this.httpClient = new HttpClient(handler);
            this.usersProvider = new ApplicationUserDataProvider();
            this.utilidadesSolidariaService = new UtilidadesSolidariaService();
            this.administracionPersonasService = new AdministracionPersonasReader();

            this.permissionsReader = new DatosPermisosRolTableReader();
        }

        [Route("user")]
        [HttpGet]
        public async Task<ApplicationUser> GetUserAsync(string userName)
        {
            try
            {
                var user = await this.usersProvider.GetUserAsync(userName);
                if (user == null)
                {
                    throw new Exception("El usuario que esta tratando de consultar no existe o no está activo.");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error realizando la validación del usuario.", ex);
            }
        }

        [AllowAnonymous]
        [Route("verifyUser")]
        [HttpGet]
        public async Task<VerifyUserResponse> VerifyUserAsync(string userName)
        {
            try
            {
                var user = await this.usersProvider.GetUserAsync(userName);
                if (user != null)
                {
                    if (user.Activo)
                    {
                        var permissions = await this.GetUserPermissionsAsync(user.Rol.RoleId);
                        user.Permissions = permissions.Select(x => x.ActionName);

                        var token = this.AuthorizeUser(user);
                        return new VerifyUserResponse
                        {
                            User = user,
                            AuthorizationToken = token
                        };
                    }
                }

                return new VerifyUserResponse { Message = "El usuario no esta registrado o no tiene permiso para acceder a la aplicación." };
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error realizando la validación del usuario.", ex);
            }
        }

        [AllowAnonymous]
        [Route("verifyUserByToken")]
        [HttpGet]
        public async Task<VerifyUserByTokenResponse> VerifyUserByTokenAsync(string token)
        {
            try
            {
                var userName = await this.GetUserNameByTokenAsync(token);
                return new VerifyUserByTokenResponse
                {
                    UserName = userName
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error realizando la validación del usuario.", ex);
            }
        }

        private async Task<string> GetUserNameByTokenAsync(string token)
        {
            int outToken;
            var canConvert = int.TryParse(token, out outToken);
            if (canConvert)
            {
                var user = await this.administracionPersonasService.TraerIntermediarioAsync(outToken);
                if (user != null)
                {
                    return user.Codigo.ToString();
                }

                return null;
            }
            else
            {
                var userName = await this.utilidadesSolidariaService.FetchFuncionarioByTokenAsync(token);
                if (userName != null)
                {
                    return userName;
                }

                return null;
            }
        }

        [AllowAnonymous]
        [Route("menu")]
        [HttpGet]
        public async Task<ApplicationMenu> GetUserMenu()
        {
            var claim = httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.RoleId);
            if (claim != null)
            {
                var roleId = claim.Value;
                var permissions = await this.GetUserPermissionsAsync(int.Parse(roleId));
                var menu = ApplicationMenu.BuildMenu(permissions);
                return menu;
            }

            return new ApplicationMenu();
        }

        private async Task<IEnumerable<UserPermission>> GetUserPermissionsAsync(int roleId)
        {
            var result = new List<UserPermission>();
            var permissions = await this.permissionsReader.GetPermisosRolAsync(roleId);
            foreach (var permission in permissions)
            {
                PermissionAction action = (PermissionAction)permission.Codigo;
                result.Add(new UserPermission { Action = action, ActionName = Enum.GetName(typeof(PermissionAction), permission.Codigo) });
            }

            return result;
        }

        private string AuthorizeUser(ApplicationUser user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Solidaria.AuthJwtAspNetCore"));
            var claims = new Claim[] {
                new Claim(ModernizacionPersonasClaimNames.UserName, user.UserName),
                new Claim(ModernizacionPersonasClaimNames.UserId, user.UserId.ToString()),
                new Claim(ModernizacionPersonasClaimNames.RoleId, user.Rol.RoleId.ToString()),
                new Claim(ModernizacionPersonasClaimNames.Email, user.ExternalInfo.EmailUsuario),
                new Claim(ModernizacionPersonasClaimNames.ZonaId, user.ExternalInfo.Zona.ToString()),
                new Claim(ModernizacionPersonasClaimNames.SucursalId, user.ExternalInfo.Sucursal.ToString()),
                new Claim(ModernizacionPersonasClaimNames.CargoId, user.ExternalInfo.CodigoCargo.ToString())
            };

            var token = new JwtSecurityToken(
                    issuer: "PMP.Api",
                    audience: "PMP.Client",
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddHours(24),
                    signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256));

            //var token = new JwtSecurityToken(new JwtHeader(new SigningCredentials(key, SecurityAlgorithms.HmacSha256)), new JwtPayload(claims));
            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }
}
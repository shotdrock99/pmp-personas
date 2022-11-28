using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.Entities;
using ModernizacionPersonas.Web.Configuration;
using ModernizacionPersonas.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient httpClient;
        private static AppConfiguration appConfig = new AppConfiguration();

        public LoginController()
        {
            var proxyUri = appConfig.ProxyURL;
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy(proxyUri, false),
                UseProxy = true
            };

            this.httpClient = new HttpClient(handler);
        }

        [AllowAnonymous]
        [Route("login")]
        public IActionResult Login(string message)
        {
            var model = new LoginModel
            {
                Message = message
            };
            return View(model);
        }

        [Route("redirect")]
        public IActionResult Redirect(LoginUserResponse model)
        {
            return View(model);
        }

        [AllowAnonymous]
        [Route("login/external")]
        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string id)
        {
            var url = $"{appConfig.ApiBaseURL}/api/v1/auth/verifyUserByToken?token={id}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var httpResponse = await httpClient.SendAsync(request);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var stream = await httpResponse.Content.ReadAsStreamAsync();
                    var serializer = new JsonSerializer();
                    using (var sr = new StreamReader(stream))
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        var result = serializer.Deserialize<VerifyUserByTokenResponse>(jsonTextReader);
                        if (!string.IsNullOrEmpty(result.UserName))
                        {
                            var model = new LoginModel
                            {
                                UserName = result.UserName
                            };

                            return await this.DoLogin(model.UserName);
                        }

                        return Unauthorized("El usuario no existe o no ha sido registrado en la aplicación.");
                    }
                }
                else
                {
                    return BadRequest(httpResponse);
                }
            }
            catch (Exception ex)
            {
                //throw new Exception($"Error invocando servicio de autenticacion.", ex);
                return BadRequest(ex);
            }
        }


        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.UserName))
            {
                return BadRequest("El nombre de usuario no puede estar vacío");
            }

            return await this.DoLogin(model.UserName);
        }
        [AllowAnonymous]
        [Route("loginSG")]
        [HttpGet]
        public async Task<IActionResult> LoginSG(String UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return BadRequest("El nombre de usuario no puede estar vacío");
            }

            return await this.DoLogin(UserName);
        }

        private async Task<IActionResult> DoLogin(string userName)
        {
            var _userName = userName.ToUpper();
            var url = $"{appConfig.ApiBaseURL}/api/v1/auth/verifyUser?userName={_userName}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var httpResponse = await httpClient.SendAsync(request);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var stream = await httpResponse.Content.ReadAsStreamAsync();
                    var serializer = new JsonSerializer();
                    using (var sr = new StreamReader(stream))
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        var result = serializer.Deserialize<VerifyUserResponse>(jsonTextReader);
                        if (result.User != null)
                        {
                            await this.SignInAsync(_userName);

                            var jsonSerializerSettings = new JsonSerializerSettings
                            {
                                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                            };

                            var json = JsonConvert.SerializeObject(result.User, jsonSerializerSettings);

#if DEBUG
                            var redirectUrl = "/cotizaciones";
#else
var redirectUrl = "/PMP/cotizaciones";
#endif

                            var response = new LoginUserResponse
                            {
                                AuthorizationToken = result.AuthorizationToken,
                                User = json,
                                RedirectUri = redirectUrl
                            };

                            return View("Redirect", response);
                        }

                        return RedirectToAction("Login", new LoginModel
                        {
                            Message = "El usuario no existe, no ha sido registrado en la aplicación o no se encuentra habilitado."
                        });
                        //return Unauthorized("El usuario no existe, no ha sido registrado en la aplicación o no se encuentra habilitado.");
                    }
                }
                else
                {
                    return BadRequest(httpResponse);
                }
            }
            catch (Exception ex)
            {
                //throw new Exception($"Error invocando servicio de autenticacion.", ex);
                return BadRequest(ex);
            }
        }

        private async Task SignInAsync(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, "Administrator"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }

    public class LoginUserResponse
    {
        public string User { get; set; }
        public string AuthorizationToken { get; set; }
        public string RedirectUri { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ModernizacionPersonas.Entities;
using ModernizacionPersonas.Web.Extensions;
using ModernizacionPersonas.Web.Models;
using Newtonsoft.Json;

namespace ModernizacionPersonas.Web.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly string UrlEntity;
        private readonly string UrlMassive;
        public MainController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // POST: api/Entity
        [HttpPost("rest")]
        public string CallRest([FromBody] RequestService data)
        {
            Entities.EMethodHttp method = (Entities.EMethodHttp)Enum.Parse(typeof(Entities.EMethodHttp), data.Method);
            var con = new ConsumeService();
            var resp = con.Call(data.Url, data.Token, method, data.Body);
            return resp.Body;
        }
    }
}



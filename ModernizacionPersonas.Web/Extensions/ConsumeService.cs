using ModernizacionPersonas.Web.Controllers;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ModernizacionPersonas.Web.Models;

namespace ModernizacionPersonas.Web.Extensions
{

    public class ConsumeService
    {
        public ResponseRest Call(string url, string token, EMethodHttp method, object body)
        {
            UtilServices uServices = new UtilServices();

            Dictionary<string, string> headers = new Dictionary<string, string> {
                {"accept", "application/json" },
                {"content-type", "application/json" }
            };
            if (token != null)
                headers.Add("Authorization", $"Bearer {token}");

            var resTemp = uServices.CallRestService(new RequestRest
            {
                Url = $"{url}",
                Headers = headers,
                Method = method,
                Body = body,
                TimeOut = 15000,
                Proxy = new WebProxy("http://192.1.3.3:8080")
            });
            return resTemp;
        }
    }

}

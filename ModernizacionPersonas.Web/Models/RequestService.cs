using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Web.Models
{
    public class RequestService
    {
        public string Url { get; set; }
        public string Token { get; set; }
        public string Body { get; set; }
        public string Method { get; set; }
    }
}

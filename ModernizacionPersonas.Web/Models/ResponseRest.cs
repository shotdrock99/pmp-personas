using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Web.Models
{
    public class ResponseRest
    {
        /// <summary>
        /// Estado de respuesta
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// Estado en texto de respuesta
        /// </summary>
        public string StatusDescription { get; set; }
        /// <summary>
        /// Cabeceras de respuesta
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// Cuerpo de respuesta
        /// </summary>
        public string Body { get; set; }
    }
}

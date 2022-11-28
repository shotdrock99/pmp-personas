using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ModernizacionPersonas.Entities;

namespace ModernizacionPersonas.Web.Models
{
    public class RequestRest
    {
        /// <summary>
        /// URL a la que se realiza la petición
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Enumerador que contiene las posibles cabeceras
        /// </summary>
        public EMethodHttp Method { get; set; }
        /// <summary>
        /// Tiempo de espera maximo para que el servicio devuelva una respuesta
        /// </summary>
        public int TimeOut { get; set; }
        /// <summary>
        /// Diccionario que contienes las cabeceras de la petición 
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// Cuerpo de la petición
        /// </summary>
        public object Body { get; set; }
        /// <summary>
        /// Objeto que configura el proxy en caso de que haya necesidad.
        /// </summary>
        public WebProxy Proxy { get; set; }
    }
}

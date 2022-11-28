using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ModernizacionPersonas.Entities;
using ModernizacionPersonas.Web.Models;

namespace ModernizacionPersonas.Web.Controllers
{
    public class UtilServices
    {
        /// <summary>
        /// Efectua la llamada a un servicio Rest y devuelve la respuesta
        /// </summary>
        /// <param name="requestRest">Objeto que contiene la petición del servicio REST</param>
        /// <returns>Devuelve la respuesta</returns>
        public async Task<ResponseRest> CallRestServiceAsync<T>(RequestRest requestRest)
        {
            ResponseRest result = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestRest.Url);
            HttpWebResponse response = null;

            try
            {
                if (requestRest.Proxy != null)
                    request.Proxy = requestRest.Proxy;

                request = AddHeaders(requestRest.Headers, request);
                request.Method = requestRest.Method.ToString();
                request.Timeout = requestRest.TimeOut > 0 ? requestRest.TimeOut : 6000;

                if (requestRest.Body != null)
                    using (var stream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                    {
                        var body = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(requestRest.Body));
                        stream.Write(body, 0, body.Length);
                    }

                response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);

                result = new ResponseRest
                {
                    Body = await new StreamReader(response.GetResponseStream()).ReadToEndAsync().ConfigureAwait(false),
                    Headers = GetHeaders(response.Headers),
                    StatusCode = (int)response.StatusCode,
                    StatusDescription = response.StatusDescription
                };
                return result;
            }
            catch (HttpRequestException e)
            {
                new HttpRequestException(e.Message);
            }
            catch (Exception e)
            {
                new HttpRequestException(e.Message);
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
            return result;
        }
        /// <summary>
        /// Efectua la llamada a un servicio Rest y devuelve la respuesta
        /// </summary>
        /// <param name="requestRest">Objeto que contiene la petición del servicio REST</param>
        /// <returns>Devuelve la respuesta</returns>
        public ResponseRest CallRestService(RequestRest requestRest)
        {
            ResponseRest result = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestRest.Url);
            HttpWebResponse response = null;
            try
            {
                if (requestRest.Proxy != null)
                    request.Proxy = requestRest.Proxy;

                request = AddHeaders(requestRest.Headers, request);
                request.Method = requestRest.Method.ToString();
                request.Timeout = requestRest.TimeOut > 0 ? requestRest.TimeOut : 6000;

                if (requestRest.Body != null)
                    using (var stream = request.GetRequestStream())
                    {
                        var body = Encoding.ASCII.GetBytes(requestRest.Body.ToString());
                        stream.Write(body, 0, body.Length);
                    }

                response = (HttpWebResponse)request.GetResponse();
                var temp = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //List<string> results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(temp);


                result = new ResponseRest
                {
                    Body = temp,
                    Headers = GetHeaders(response.Headers),
                    StatusCode = (int)response.StatusCode,
                    StatusDescription = response.StatusDescription
                };
            }
            catch (HttpRequestException e)
            {
                new HttpRequestException(e.Message);
            }
            catch (Exception e)
            {
                new HttpRequestException(e.Message);
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
            return result;
        }
        /// <summary>
        /// Efectua la llamada a un servicio Rest y devuelve la respuesta
        /// </summary>
        /// <param name="requestRest">Objeto que contiene la petición del servicio REST</param>
        /// <param name="isBody">Objeto que contiene la petición del servicio REST</param>
        /// <returns>Devuelve la respuesta</returns>
        public ResponseRest CallRestService(RequestRest requestRest, bool isBody)
        {
            ResponseRest result = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestRest.Url);
            HttpWebResponse response = null;
            try
            {
                if (requestRest.Proxy != null)
                    request.Proxy = requestRest.Proxy;

                request = AddHeaders(requestRest.Headers, request);
                request.Method = requestRest.Method.ToString();
                request.Timeout = requestRest.TimeOut > 0 ? requestRest.TimeOut : 6000;
                if (isBody)
                    using (var stream = request.GetRequestStream())
                    {
                        var body = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(requestRest.Body));
                        stream.Write(body, 0, body.Length);
                    }

                response = (HttpWebResponse)request.GetResponse();

                result = new ResponseRest

                {
                    Body = new StreamReader(response.GetResponseStream()).ReadToEnd(),
                    Headers = GetHeaders(response.Headers),
                    StatusCode = (int)response.StatusCode,
                    StatusDescription = response.StatusDescription
                };
            }
            catch (HttpRequestException e)
            {
                new HttpRequestException(e.Message);
            }
            catch (Exception e)
            {
                new HttpRequestException(e.Message);
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
            return result;
        }
        /// <summary>
        /// Obtiene las cabeceras de la respuesta y las devuelve en un diccionario
        /// </summary>
        /// <param name="headers">Cabeceras de la respuesta de la petición</param>
        /// <returns>Devuelve diccionario con las cabeceras de la respuesta</returns>
        private Dictionary<string, string> GetHeaders(WebHeaderCollection headers)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (string item in headers.AllKeys)
            {
                result.Add(item, headers[item]);
            }
            return result;
        }
        /// <summary>
        /// Agrega las cabeceras a la petición
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private HttpWebRequest AddHeaders(Dictionary<string, string> headers, HttpWebRequest request)
        {
            if (request != null)
            {
                if (headers != null && headers.Count > 0)
                    foreach (var item in headers)
                        request = ValidateAddHeader(item.Key, item.Value, request);
                else
                    request = HeadersDefault(request);
            }

            return request;
        }
        /// <summary>
        /// Valida las cabeceras restringidas y asigna los valores a cada una
        /// </summary>
        /// <param name="key">Nombre de la cabecera</param>
        /// <param name="value">Valor de la cabecera</param>
        /// <param name="request">Peticion</param>
        private HttpWebRequest ValidateAddHeader(string key, string value, HttpWebRequest request)
        {
            if (WebHeaderCollection.IsRestricted(key.ToLower()))
            {
                switch (key.ToLower())
                {
                    case "accept":
                        request.Accept = value;
                        break;
                    case "connection":
                        request.Connection = value;
                        break;
                    case "content-length":
                        request.ContentLength = long.Parse(value);
                        break;
                    case "content-type":
                        request.ContentType = value;
                        break;
                    case "date":
                        request.Date = DateTime.Parse(value);
                        break;
                    case "expect":
                        request.Expect = value;
                        break;
                    case "host":
                        request.Host = value;
                        break;
                    case "if-modified-since":
                        request.IfModifiedSince = DateTime.Parse(value);
                        break;
                    case "referer":
                        request.Referer = value;
                        break;
                    case "transfer-encoding":
                        request.TransferEncoding = value;
                        break;
                    case "user-agent":
                        request.UserAgent = value;
                        break;
                }
            }
            else
            {
                request.Headers.Add(key, value);
            }
            return request;
        }
        /// <summary>
        /// Agrega las cabeceras por defecto para la llamada de un servicio rest
        /// </summary>
        /// <returns>Devuelve las cabeceras de la petición</returns>
        private HttpWebRequest HeadersDefault(HttpWebRequest request)
        {
            request.Accept = "application/json";
            request.ContentType = "application/json";

            return request;
        }
        /// <summary>
        /// Convierte un JSON en un diccionario
        /// </summary>
        /// <param name="body">JSON a convertir</param>
        /// <returns>Devuelve un diccionario con la información del JSON</returns>
        public static Dictionary<string, object> ConvertJson(string body)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
        }
        /// <summary>
        /// Agrega parametros a una url
        /// </summary>
        /// <param name="param">Diccionario que va a contener todos los valores</param>
        /// <returns>Devuelve una cadena de texto en formato paramQuery</returns>
        public static string AddParamQuery(Dictionary<string, string> param)
        {
            string result = string.Empty;
            param = param ?? new Dictionary<string, string>();

            var keys = param.Keys;
            foreach (var item in keys)
                result += $"{item}={param[item]}&";
            return "?" + new string(result.Take(result.Count() - 1).ToArray());
        }
       
        /// <summary>
        /// Objeto de respuesta al finalizar llamada de un servicio REST
        /// </summary>
        
        /// <summary>
        /// Un objeto que contiene la respuesta de un metodo ya sea de un metodo web o metodo general.
        /// </summary>
        public class ManageResponse
        {
            /// <summary>
            /// Un mensaje de acuerdo al error que se haya generado.
            /// </summary>
            public string Message { get; set; }
            /// <summary>
            /// Indica si hubo algún fallo en el proceso al momento de consumir el metodo.
            /// </summary>
            public bool IsException { get; set; }
            /// <summary>
            /// Devuelve el objeto que contiene una excepción.
            /// </summary>
            public object Exception { get; set; }
            /// <summary>
            /// Respuesta del servicio.
            /// </summary>
            public object Response { get; set; }
        }
    }
}

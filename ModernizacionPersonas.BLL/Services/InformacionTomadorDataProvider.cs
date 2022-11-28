using ModernizacionPersonas.Api.Entities;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.Common.Configuration;
using ModernizacionPersonas.Common.Services;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ModernizacionPersonas.BLL.Services
{
    public class InformacionTomadorDataProvider
    {
        private readonly CotizacionTransactionsProvider transactionProvider;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosCotizacionWriter cotizacionWriterService;
        private readonly IDatosTomadorReader datosTomadorReader;
        private readonly IDatosTomadorWriter datosTomadorWriter;
        private readonly CotizacionStateWriter cotizacionStateWriter;
        private readonly SARLAFTValidatorService SARLAFTService;
        private HttpClient httpClient;
        private static AppConfiguration appConfig = new AppConfiguration();

        public InformacionTomadorDataProvider()
        {
            this.transactionProvider = new CotizacionTransactionsProvider();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.cotizacionWriterService = new DatosCotizacionTableWriter();
            this.datosTomadorReader = new DatosTomadorTableReader();
            this.datosTomadorWriter = new DatosTomadorTableWriter();
            this.cotizacionStateWriter = new CotizacionStateWriter();
            this.SARLAFTService = new SARLAFTValidatorService();
            var proxyUri = appConfig.ProxyURL;
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy(proxyUri, false),
                UseProxy = true
            };

            this.httpClient = new HttpClient(handler);
        }
        public async Task<ActionResponseBase> ValidarTomador(int tipoDoc, string numDoc)
        {
            using (var client = new System.Net.Http.HttpClient())
            {

                try
                {
                     var proxyUri = "http://192.1.3.3:8080";
                    //var proxyUri = appConfig.ProxyURL;
                    var handler = new HttpClientHandler
                    {
                        Proxy = new WebProxy(proxyUri, false),
                        UseProxy = true
                    };
                    //var handler = new HttpClientHandler { Proxy = new WebProxy(proxyUri, false), UseProxy = true };
                    HttpClient httpClient = new HttpClient(handler);
                    //HttpClient httpClient = new HttpClient();
                    var serializer = new JsonSerializer();


                    using (var request_ = new HttpRequestMessage())
                    {
                        var todoItem = new credentials
                        {
                            P_st_UserName = "SolidariaVetos$",
                            P_st_Password = "4dm1n5up3rV3t0s$%&"
                        };

                        var jsonString = new StringContent(JsonConvert.SerializeObject(todoItem), Encoding.UTF8, Application.Json);

                        request_.Content = jsonString;
                        request_.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                        request_.Method = new HttpMethod("POST");
                        Uri myUriLog = new Uri(appConfig.VetosLogin.ToString(), UriKind.Absolute);
                        request_.RequestUri = myUriLog;
                        //request_.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        httpClient.Timeout = TimeSpan.FromMinutes(20);
                        var response_ = await httpClient.SendAsync(request_, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                        //var responseText_ = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        if (response_.StatusCode == System.Net.HttpStatusCode.OK) // 200
                        {
                            if (response_.IsSuccessStatusCode)
                            {
                                var stream = await response_.Content.ReadAsStreamAsync();
                                serializer = new JsonSerializer();
                                using (var sr = new StreamReader(stream))
                                using (var jsonTextReader = new JsonTextReader(sr))
                                {
                                    var result = serializer.Deserialize<VerifyUserResponse>(jsonTextReader);
                                    if (!string.IsNullOrEmpty(result.response.p_st_Token))
                                    {
                                        var dataTomador = new dataTomador
                                        {
                                            documentNumber = numDoc,
                                            documentype = tipoDoc
                                        };

                                        var jsonStringTomador = new StringContent(JsonConvert.SerializeObject(dataTomador), Encoding.UTF8, Application.Json);

                                        return await ValidarTomadorEndpont(jsonStringTomador, result.response.p_st_Token);
                                    }
                                }

                            }
                        }
                        httpClient.Dispose();

                        return new ActionResponseBase
                        {
                            Status = ResponseStatus.Invalid,
                            ErrorCode = "Error",
                            ErrorMessage = "ocurrio un error al hacer la consulta"
                        };
                    }

                }


                catch (Exception ex)
                {
                    return new ActionResponseBase
                    {
                        Status = ResponseStatus.Invalid,
                        ErrorCode = "Error",
                        ErrorMessage = ex.StackTrace + ex.Message + "URL:" + appConfig.VetosLogin.ToString()
                    };
                }


            }
        }
        public async static Task<ActionResponseBase> ValidarTomadorEndpont(StringContent jsonPeticion, string accessToken)
        {
            try
            {
                // var proxyUri = "http://192.1.3.3:8080";
                var proxyUri = appConfig.ProxyURL;
                var handler = new HttpClientHandler
                {
                    Proxy = new WebProxy(proxyUri, false),
                    UseProxy = true
                };
                //var handler = new HttpClientHandler { Proxy = new WebProxy(proxyUri, false), UseProxy = true };
                HttpClient httpClient = new HttpClient(handler);
                //HttpClient httpClient = new HttpClient();
                var serializer = new JsonSerializer();


                using (var request_ = new HttpRequestMessage())
                {

                    request_.Content = jsonPeticion;
                    request_.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    request_.Method = new HttpMethod("GET");
                    Uri myUri = new Uri(appConfig.VetosData.ToString(), UriKind.Absolute);
                    request_.RequestUri = myUri;
                    request_.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    httpClient.Timeout = TimeSpan.FromMinutes(20);
                    var response_ = await httpClient.SendAsync(request_, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                    //var responseText_ = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (response_.StatusCode == System.Net.HttpStatusCode.OK) // 200
                    {
                        if (response_.IsSuccessStatusCode)
                        {
                            var streamTomador = await response_.Content.ReadAsStreamAsync();
                            var serializerTomador = new JsonSerializer();
                            using (var sr2 = new StreamReader(streamTomador))
                            using (var jsonTextReaderTomador = new JsonTextReader(sr2))
                            {
                                var resultTomador = serializer.Deserialize<VerifyUserResponseTomador>(jsonTextReaderTomador);
                                if (!string.IsNullOrEmpty(resultTomador.response))
                                {
                                    resultTomador.response = resultTomador.response.Replace("[", "").Replace("]", "");
                                    responseTomadorInt data = JsonConvert.DeserializeObject<responseTomadorInt>(resultTomador.response);
                                    if (data.estado == "true")
                                    {
                                        return new ActionResponseBase
                                        {
                                            Status = ResponseStatus.Valid,
                                            ErrorCode = "OK",
                                            ErrorMessage = data.Respuesta,

                                        };
                                    }
                                    else
                                    {
                                        return new ActionResponseBase
                                        {
                                            Status = ResponseStatus.Valid,
                                            ErrorCode = "Error",
                                            ErrorMessage = "Esta operación no se puede realizar. Favor comunicarse con la Gerencia Oficial de Cumplimiento"
                                        };
                                    }


                                }
                            }
                        }

                    }
                    httpClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                return new ActionResponseBase
                {
                    Status = ResponseStatus.Invalid,
                    ErrorCode = "Error",
                    ErrorMessage = ex.StackTrace + ex.Message + "URL:" + appConfig.VetosData.ToString()
                };



            }
            return null;
        }
        public async Task<ActionResponseBase> InsertarDatosTomadorAsync(int codigoCotizacion, Tomador model)
        {
            model.CodigoCotizacion = codigoCotizacion; // Asignar CodigoCotizacion al modelo
            var tomador = await this.datosTomadorReader.GetTomadorAsync(codigoCotizacion);
            if (!model.Equals(tomador))
            {
                // Inserta tomador
                var codigoTomador = await datosTomadorWriter.CrearTomadorAsync(codigoCotizacion, model);
                // Enviara mensaje de validacion de SARLAFT
                var isValid = await this.ValidarTomadorAsync(model);
                if (!isValid)
                {
                    // cambiar estado a reachazado por SARLAFT
                    await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.RejectedByCompany);
                    await this.transactionProvider.CreateTransactionAsync(codigoCotizacion, 0, "", "Cotización rechazada por asegurado vetado.");
                    return new ActionResponseBase
                    {
                        Status = ResponseStatus.Invalid,
                        ErrorCode = "2001",
                        ErrorMessage = "Esta operación no se puede realizar. Favor comunicarse con la Gerencia Oficial de Cumplimiento"
                    };
                }

                // Actualiza codigo tomador en cotizacion
                await this.cotizacionWriterService.UpdateCotizacionTomadorAsync(codigoCotizacion, codigoTomador);
                // Actualizar el estado de la cotizacion
                var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                if (informacionNegocio.CotizacionState < CotizacionState.OnDatosTomador)
                {
                    await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.OnDatosTomador);
                }

                return new ActionResponseBase
                {
                    CodigoCotizacion = codigoCotizacion,
                    CodigoEstadoCotizacion = informacionNegocio.CodigoEstadoCotizacion,
                    Version = informacionNegocio.Version,
                    NumeroCotizacion = informacionNegocio.NumeroCotizacion
                };
            }

            // return 304 Not Modified
            return new ActionResponseBase();
        }

        private async Task<bool> ValidarTomadorAsync(Tomador model)
        {
            var parts = model.Nombres.Split(' ');
            var validation = await this.SARLAFTService.ValidateAsync(model.CodigoTipoDocumento, model.NumeroDocumento, parts[0], parts[1], model.PrimerApellido, model.SegundoApellido);
            return validation;
        }
    }
}

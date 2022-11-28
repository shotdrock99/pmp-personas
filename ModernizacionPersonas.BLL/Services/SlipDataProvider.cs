using Microsoft.AspNetCore.Http;
using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Services.PDFSlipGenerate;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.Common.Utilities;
using ModernizacionPersonas.DAL;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Soligespro;
using ModernizacionPersonas.Entities;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModernizacionPersonas.BLL.AppConfig;

namespace ModernizacionPersonas.BLL.Services
{
    public class SlipDataProvider
    {
        private readonly IDatosAmparoGrupoAseguradoReader amparoGrupoAseguradoReader;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosGruposAseguradoReader gruposAseguradosReader;
        private readonly IDatosOpcionValorAseguradoReader opcionValorReader;
        private readonly IDatosTasaOpcionReader tasaOpcionValorReader;
        private readonly IDatosEdadesReader edadesReader;
        private readonly IDatosTomadorReader tomadorReader;
        private readonly IDatosIntermediarioReader intermediarioReader;
        private readonly IDatosSlipReader slipReader;
        private readonly IEmailSender emailSender;
        private readonly IDatosAsegurabilidadReader asegurabilidadReader;
        private readonly CotizacionDataProcessorFactory primasDataProcessorFactory;
        private readonly CotizacionStateWriter cotizacionStateWriter;
        private readonly CotizacionTransactionsProvider cotizacionTransactionsProvider;
        private readonly IDatosEnvioSlipWriter datosEnvioSlipWriter;
        private readonly IDatosEnvioSlipReader datosEnvioSlipReader;
        private readonly DatosParametrizacionReader parametrizacionReader;
        private readonly CotizacionStateWriter cotizacionStateUpdater;
        private readonly ISoligesproDataUsuariosReader soligesproUsersDataReader;
        private readonly CotizacionDataProcessorFactory cotizacionDataProcessorFactory;
        private readonly IDatosCotizacionWriter datosCotizacionWriter;
        private PDFSlipBuilderService pdfSlipBuilderService;
        private readonly EmailsDataProvider emailsDataProvider;
        private string basePath = "";
        private readonly FichaTecnicaDataProvider fichaProvider;
        private readonly DatosGruposAseguradosMapper gruposAseguradosMapper;
        private readonly AppConfigurationFromJsonFile AppConfig;

        private IResumenDataProcessor primasDataProcessor;
        private SlipVariablesDataProvider slipVariablesReader;
        private InformacionNegocio informacionNegocio;
        private IEnumerable<TipoSumaAsegurada> tiposSumaAsegurada;
        private decimal valorSalarioMinimo = 0;
        private readonly ConfiguracionSlipDataProvider ConfiguracionSlipDataProvider;
        public SlipDataProvider()
        {
            this.primasDataProcessorFactory = new CotizacionDataProcessorFactory();
            this.amparoGrupoAseguradoReader = new DatosAmparoGrupoAseguradoTableReader();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.gruposAseguradosReader = new DatosGruposAseguradosTableReader();
            this.tomadorReader = new DatosTomadorTableReader();
            this.intermediarioReader = new DatosIntermediarioTableReader();
            this.parametrizacionReader = new DatosParametrizacionReader();
            this.slipReader = new DatosSlipTableReader();
            this.opcionValorReader = new DatosOpcionValorAseguradoTableReader();
            this.tasaOpcionValorReader = new DatosTasaOpcionTableReader();
            this.edadesReader = new DatosEdadesTableReader();
            this.cotizacionStateWriter = new CotizacionStateWriter();
            this.cotizacionTransactionsProvider = new CotizacionTransactionsProvider();
            this.datosEnvioSlipWriter = new DatosEnvioSlipTableWriter();
            this.datosEnvioSlipReader = new DatosEnvioSlipTableReader();
            this.emailSender = new SolidariaExchangeEmailSender();
            this.soligesproUsersDataReader = new SoligesproDataUsuariosReader();
            this.asegurabilidadReader = new DatosAsegurabilidadTableReader();
            this.cotizacionStateUpdater = new CotizacionStateWriter();
            this.cotizacionDataProcessorFactory = new CotizacionDataProcessorFactory();
            this.datosCotizacionWriter = new DatosCotizacionTableWriter();
            this.emailsDataProvider = new EmailsDataProvider();
            this.fichaProvider = new FichaTecnicaDataProvider();
            this.gruposAseguradosMapper = new DatosGruposAseguradosMapper();
            this.ConfiguracionSlipDataProvider = new ConfiguracionSlipDataProvider();
            this.AppConfig = new AppConfigurationFromJsonFile();
            var rootPMP = @"\\FILESERVERIBM\PMP_Repositorio";
            this.basePath = $@"{rootPMP}\{this.AppConfig.NameEnvironemnt}\";
        }

        public async Task<GenerarSlipDataResponse> GenerateSlipAsync(int codigoCotizacion, int version, string userName)
        {
            try
            {
                this.informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                if (this.informacionNegocio.CotizacionState < CotizacionState.ApprovedAuthorization)
                {
                    return new GenerarSlipDataResponse
                    {
                        Status = ResponseStatus.Invalid,
                        CodigoCotizacion = codigoCotizacion,
                        NumeroCotizacion = informacionNegocio.NumeroCotizacion,
                        CodigoEstadoCotizacion = informacionNegocio.CodigoEstadoCotizacion,
                        ErrorMessage = $"No es posible acceder al Slip de la cotización {informacionNegocio.NumeroCotizacion}, compruebe el estado e intente nuevamente.",
                        Details = $"La cotización {informacionNegocio.NumeroCotizacion}, con codigo {codigoCotizacion} se encuentra en estado {informacionNegocio.CotizacionState} y no se puede acceder al Slip.",
                        Version = informacionNegocio.Version
                    };
                }

                this.tiposSumaAsegurada = await this.informacionPersonasReader.TraerTiposSumaAsegurada(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo);
                this.slipVariablesReader = new SlipVariablesDataProvider(this.slipReader, codigoCotizacion, informacionNegocio.CodigoRamo, informacionNegocio.CodigoSector, informacionNegocio.CodigoSubramo);
                var sucursal = await this.informacionPersonasReader.TraerSucursalAsync(this.informacionNegocio.CodigoSucursal);
                var ramo = await this.informacionPersonasReader.TraerRamoAsync(this.informacionNegocio.CodigoRamo);
                var sector = this.informacionNegocio.CodigoSector;
                var subramo = this.informacionNegocio.CodigoSubramo;
                var departamentos = await this.parametrizacionReader.TraerDepartamentosAsync();
                var actividad = this.slipVariablesReader.ObtenerVariablePorNombre(NombreSlipVariable.Actividad);
                var gruposAsegurados = await this.gruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);

                var esTipoSumaAseguradaSalario = tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == 10).Count() > 0;
                if (esTipoSumaAseguradaSalario)
                {
                    this.valorSalarioMinimo = tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == 10).FirstOrDefault().ValorSalarioMinimo;
                }

                var tomador = await this.GetTomadorAsync(codigoCotizacion);
                tomador.Actividad = actividad;

                var tomadorIntermediario = await this.GetTomadorIntermediarioAsync(codigoCotizacion);

                var dirigidoSlip = await this.GetDirigidoSlip(codigoCotizacion);

                var amparos = await this.informacionPersonasReader.TraerAmparosAsync(this.informacionNegocio.CodigoRamo, this.informacionNegocio.CodigoSubramo, this.informacionNegocio.CodigoSector);
                var amparosOrden = amparos.OrderBy(x => x.CodigoGrupoAmparo).ThenByDescending(p => p.SiNoBasico).ThenByDescending(m => m.SiNoAdicional).ThenBy(n => n.NombreAmparo).ToList();

                var gruposAseguradosSlip = await this.GetGruposAseguradosSlipAsync(gruposAsegurados);

                var seccionesResponse = await this.LeerTextosSlipAsync(codigoCotizacion, version, sector, subramo);
                var asegurabilidad = await this.asegurabilidadReader.LeerAsegurabilidadAsync(codigoCotizacion);
                seccionesResponse.Clausulas.Asegurabilidad = asegurabilidad;
                var seccionesAmparo = seccionesResponse.Amparos;
                var seccionesClausula = seccionesResponse.Clausulas;
                var seccionesPortada = seccionesResponse.InfoGeneral;
                var seccionesDisposiciones = seccionesResponse.Disposiciones;

                foreach (var amparosTextos in seccionesAmparo.Amparos)
                {
                    foreach (Amparo amparoPersonas in amparosOrden)
                    {
                        if (amparoPersonas.CodigoAmparo == int.Parse(amparosTextos.CodigoAmparo))
                        {
                            amparosTextos.Basico = amparoPersonas.SiNoBasico;
                            amparosTextos.Adicional = amparoPersonas.SiNoAdicional;
                        }
                    }
                }

                var queryAmparos = seccionesAmparo.Amparos.OrderByDescending(o => o.Basico).ThenBy(o => o.Adicional).ToList();
                var variablesSlip = await this.ConfiguracionSlipDataProvider.ObtenerVariablesSlipAsync(codigoCotizacion, ramo.CodigoRamo, this.informacionNegocio.CodigoSector, informacionNegocio.CodigoSubramo);
                var ciudadSrt = "";
                foreach (var i in variablesSlip)
                {

                    if (i.Nombre == "Ciudad Poliza" && i.Valor != "")
                    {
                        ciudadSrt = i.Valor;
                    }
                }
                if (ciudadSrt == "")
                {
                    ciudadSrt = this.ObtenerCiudadExpedicionSlip();
                }
                seccionesAmparo.Amparos = queryAmparos;

                var seccionesCondiciones = new CondicionesTextosSeccionSlip();
                var seccionesEdades = new EdadesSlipSection();

                var asunto = $"COTIZACIÓN {ramo.NombreRamo}";
                var ciudad = ciudadSrt;
                var descripcion = this.ObtenerDescripcionSlip(codigoCotizacion);
                var fecha = DateTime.Now;
                var imagenProductoUri = this.ObtenerImagenProductoSlip(codigoCotizacion);
                var infoEncabezado = new PageHeaderInfo
                {
                    // TODO configura imagenes de encabezado del pagina
                    HeaderImageUri = this.GetHeaderImage(),
                    FooterImageUri = this.GetFooterImage()
                };

                var tipoPoliza = ramo.NombreAbreviado;
                var diasVigencia = this.slipVariablesReader.ObtenerVariablePorNombre(NombreSlipVariable.DiasVigencia);
                var vigencia = this.ObtenerVigenciaSlip(informacionNegocio.FechaInicio, informacionNegocio.FechaFin);
                var diasVigenciaWords = Number2WordConverter.Convert(diasVigencia);
                var condiciones = this.slipVariablesReader.ObtenerVariablePorNombre(NombreSlipVariable.Codiciones);
                seccionesCondiciones.Condiciones = condiciones;

                var informacionEnvio = await this.BuildInformacionEnvioSlip(sucursal, informacionNegocio.NombreDirectorComercial);

                // Actualizar el estado de la cotizacion                                
                if (informacionNegocio.CotizacionState < CotizacionState.OnSlip)
                {
                    await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.OnSlip);
                    // register transaction
                    await this.cotizacionTransactionsProvider.CreateTransactionAsync(codigoCotizacion, informacionNegocio.Version, userName, "Generación de Slip");
                }

                var slip = new Slip
                {
                    CodigoRamo = this.informacionNegocio.CodigoRamo,
                    CodigoSubramo = this.informacionNegocio.CodigoSubramo,
                    Asunto = asunto,
                    Ciudad = ciudad,
                    Descripcion = descripcion,
                    Fecha = fecha,
                    ImagenProductoUri = imagenProductoUri,
                    InfoEncabezado = infoEncabezado,
                    TipoPoliza = tipoPoliza,
                    Vigencia = vigencia,
                    DiasVigencia = int.Parse(diasVigencia),
                    DiasVigenciaWords = diasVigenciaWords,
                    Tomador = tomador,
                    TomadorIntermediario = tomadorIntermediario,
                    DirigidoSlip = dirigidoSlip,
                    GruposAsegurados = gruposAseguradosSlip,
                    Amparos = seccionesAmparo,
                    Clausulas = seccionesClausula,
                    Edades = seccionesEdades,
                    InfoGeneral = seccionesPortada,
                    Disposiciones = seccionesDisposiciones,
                    Condiciones = seccionesCondiciones,
                    InformacionEnvio = informacionEnvio,
                    NombreSucursal = sucursal.NombreSucursal
                };

                return new GenerarSlipDataResponse
                {
                    Data = slip,
                    CodigoCotizacion = codigoCotizacion,
                    CodigoEstadoCotizacion = this.informacionNegocio.CodigoEstadoCotizacion,
                    NumeroCotizacion = this.informacionNegocio.NumeroCotizacion
                };
            }
            catch (Exception ex)
            {
                throw new Exception("GenerateSlipAsync :: SlipDataProvider", ex);
            }
        }

        private async Task<TomadorSlip> GetTomadorAsync(int codigoCotizacion)
        {
            // conulta informacion de tomador
            var informacionTomador = await this.tomadorReader.GetTomadorAsync(codigoCotizacion);
            var ciudadResponse = await parametrizacionReader.TraerMunicipioAsync(informacionTomador.CodigoDepartamento, informacionTomador.CodigoMunicipio);
            var departamentoResponse = await parametrizacionReader.TraerDepartamentoAsync(informacionTomador.CodigoDepartamento);
            var nombre = informacionTomador.CodigoTipoDocumento == 3 ? informacionTomador.PrimerApellido
                : $"{informacionTomador.Nombres } {informacionTomador.PrimerApellido} {informacionTomador.SegundoApellido}";
            var tomador = new TomadorSlip
            {
                CodigoTomador = informacionTomador.CodigoTomador,
                Nombre = nombre,
                CodigoTipoDocumento = informacionTomador.CodigoTipoDocumento,
                NumeroDocumento = informacionTomador.NumeroDocumento,
                CodigoDepartamento = departamentoResponse.CodigoDepartamento,
                CodigoCiudad = ciudadResponse.CodigoMunicipio,
                Direccion = informacionTomador.Direccion,
                Telefono = informacionTomador.Telefono1Contacto,
                Email = informacionTomador.Email,
                NombreTomadorSlip = informacionTomador.TomadorSlip
            };

            return tomador;
        }

        private async Task<DirigidoSlip> GetDirigidoSlip(int codigoCotizacion)
        {
            var informacionTomador = await this.tomadorReader.GetTomadorAsync(codigoCotizacion);
            var ciudadResponse = await parametrizacionReader.TraerMunicipioAsync(informacionTomador.CodigoDepartamento, informacionTomador.CodigoMunicipio);
            var departamentoResponse = await parametrizacionReader.TraerDepartamentoAsync(informacionTomador.CodigoDepartamento);
            var nombre = informacionTomador.CodigoTipoDocumento == 3 ? informacionTomador.PrimerApellido
                : $"{informacionTomador.Nombres } {informacionTomador.PrimerApellido} {informacionTomador.SegundoApellido}";
            var dirigoSlip = new DirigidoSlip
            {
                Codigo = informacionTomador.CodigoTomador,
                Nombre = informacionTomador.TomadorSlip,
                Direccion = informacionTomador.Direccion,
                Email = informacionTomador.Email,
                Telefono = informacionTomador.Telefono1Contacto
            };

            //consulta infomacion de intermediario
            var intermediarios = await this.intermediarioReader.GetIntermediariosAsync(codigoCotizacion);
            var hasIntermediarios = intermediarios.Count() > 0;
            if (hasIntermediarios)
            {
                // retorna el intermediario con mayor participacion, o en su defecto el primer intermediario si la participacion es igual
                var maxParticipacion = intermediarios.Max(x => x.Participacion);
                var intermediario = intermediarios.Where(x => x.Participacion == maxParticipacion).FirstOrDefault();
                dirigoSlip = new DirigidoSlip
                {
                    Codigo = intermediario.Codigo,
                    Nombre = intermediario.IntermediarioSlip,
                    Direccion = intermediario.Direccion,
                    Email = intermediario.Email,
                    Telefono = intermediario.Telefono
                };
            }

            return dirigoSlip;
        }

        private async Task<TomadorSlip> GetTomadorIntermediarioAsync(int codigoCotizacion)
        {
            // conulta informacion de tomador
            var informacionTomador = await this.tomadorReader.GetTomadorAsync(codigoCotizacion);
            var ciudadResponse = await parametrizacionReader.TraerMunicipioAsync(informacionTomador.CodigoDepartamento, informacionTomador.CodigoMunicipio);
            var departamentoResponse = await parametrizacionReader.TraerDepartamentoAsync(informacionTomador.CodigoDepartamento);
            var nombre = informacionTomador.CodigoTipoDocumento == 3 ? informacionTomador.PrimerApellido
                : $"{informacionTomador.Nombres } {informacionTomador.PrimerApellido} {informacionTomador.SegundoApellido}";
            var tomador = new TomadorSlip
            {
                CodigoTomador = informacionTomador.CodigoTomador,
                Nombre = nombre,
                CodigoTipoDocumento = informacionTomador.CodigoTipoDocumento,
                NumeroDocumento = informacionTomador.NumeroDocumento,
                CodigoDepartamento = departamentoResponse.CodigoDepartamento,
                CodigoCiudad = ciudadResponse.CodigoMunicipio,
                Direccion = informacionTomador.Direccion,
                Telefono = informacionTomador.Telefono1Contacto,
                Email = informacionTomador.Email,
                NombreTomadorSlip = informacionTomador.TomadorSlip
            };

            // consulta infomacion de intermediario
            var intermediarios = await this.intermediarioReader.GetIntermediariosAsync(codigoCotizacion);
            var hasIntermediarios = intermediarios.Count() > 0;
            if (hasIntermediarios)
            {
                // retorna el intermediario con mayor participacion, o en su defecto el primer intermediario si la participacion es igual
                var maxParticipacion = intermediarios.Max(x => x.Participacion);
                var intermediario = intermediarios.Where(x => x.Participacion == maxParticipacion).FirstOrDefault();
                tomador = new TomadorSlip
                {
                    CodigoTomador = intermediario.Codigo,
                    Nombre = $"{intermediario.PrimerNombre} {intermediario.SegundoNombre} {intermediario.PrimerApellido} {intermediario.SegundoApellido}",
                    CodigoTipoDocumento = intermediario.CodigoTipoDocumento,
                    NumeroDocumento = intermediario.NumeroDocumento,
                    CodigoDepartamento = intermediario.CodigoDepartamento,
                    CodigoCiudad = intermediario.CodigoMunicipio,
                    Direccion = intermediario.Direccion,
                    Telefono = intermediario.Telefono,
                    Email = intermediario.Email,
                    EsIntermediario = true,
                    NombreTomadorSlip = intermediario.IntermediarioSlip
                };
            }

            return tomador;
        }

        private async Task<IEnumerable<string>> BuildInformacionEnvioSlip(Sucursal sucursal, string dt)
        {
            var contacts = await this.soligesproUsersDataReader.GetDirectoresAsync(sucursal.CodigoSucursal);
            if (dt == null)
            {
                var gerente = await this.soligesproUsersDataReader.GetUserGerenteAsync(sucursal.CodigoSucursal);
                dt = gerente.NombreUsuario;

            }
            var trueDt = contacts.Where(x => x.NombreUsuario == dt).First();

            var result = contacts
                .Where(x => (x.EmailUsuario == trueDt.EmailUsuario && x.NombreUsuario == trueDt.NombreUsuario) || (x.EmailUsuario != trueDt.EmailUsuario))
                .Select(y => y.EmailUsuario);

            //    if (x.EmailUsuario == trueDt.EmailUsuario)
            //    {
            //        if (x.NombreUsuario != trueDt.NombreUsuario)
            //            return null;
            //    }
            //    return x.EmailUsuario;
            //}).Where(y => y!=null);

            return result;
        }

        public async Task<ActionResponseBase> SaveCotizacionSlipAsync(int codigoCotizacion, string userName, string numCotizacion, int? version)
        {
            try
            {
                //New Slip
                this.informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                var slip = await this.GenerateSlipAsync(codigoCotizacion, informacionNegocio.Version, userName);
                var variablesSlip = await this.ConfiguracionSlipDataProvider.ObtenerVariablesSlipAsync(codigoCotizacion, informacionNegocio.CodigoRamo, informacionNegocio.CodigoSector, informacionNegocio.CodigoSubramo);
                this.pdfSlipBuilderService = new PDFSlipBuilderService(slip.Data, informacionNegocio.CodigoCotizacion, informacionNegocio.NumeroCotizacion, informacionNegocio.Version, variablesSlip);
                var slipArrayByte = await this.pdfSlipBuilderService.GenerateSlipDF();
                var numCotizacionInt = int.Parse(numCotizacion);
                var fileName = $"Slip_Cotización_#{numCotizacionInt + "VR"+ version}.pdf";

                var response = await this.datosEnvioSlipWriter.CrearAdjuntoEnvioSlipAsync(codigoCotizacion, slipArrayByte, fileName);

                return new CreateActionResponse
                {
                    CodigoCotizacion = codigoCotizacion,
                    Codigo = response
                };
            }
            catch (Exception ex)
            {
                throw new Exception("SlipDataProvider :: SaveCotizacionSlipAsync", ex);
            }
        }

        public async Task<ActionResponseBase> SaveAttachmentsSlipAsync(int codigoCotizacion, IFormFile file)
        {
            try
            {
                using (var s = file.OpenReadStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    s.CopyTo(ms);
                    ms.Position = 0;
                    var array = ms.ToArray();
                    var response = await this.datosEnvioSlipWriter.CrearAdjuntoEnvioSlipAsync(codigoCotizacion, array, file.FileName);
                    return new CreateActionResponse
                    {
                        CodigoCotizacion = codigoCotizacion,
                        Codigo = response
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SlipDataProvider :: SaveAttachmentsSlipAsync", ex);
            }
        }

        public async Task<SendEmailResponse> SendCotizacionSlipAsync(SendSlipArgs args, string userName)
        {
            try
            {
                this.informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(args.CodigoCotizacion);
                var tomador = await this.tomadorReader.GetTomadorAsync(args.CodigoCotizacion);
                var nombreTomador = $"{tomador.Nombres} {tomador.PrimerApellido}";
                var ramo = await this.informacionPersonasReader.TraerRamoAsync(this.informacionNegocio.CodigoRamo);
                await this.SaveEnvioCotizacionSlip(args);
                // send emails
                args.Version = informacionNegocio.Version;
                args.NumeroCotizacion = informacionNegocio.NumeroCotizacion;

                if (informacionNegocio.EsNegocioDirecto)
                {
                    await this.SendSlipTomadorAsync(args, nombreTomador, ramo.NombreRamo, userName);
                }
                else
                {
                    await this.SendSlipIntermediarioComercialAsync(args, nombreTomador, ramo.NombreRamo, userName);
                }


                //Close In_cierre at slip send
                await this.datosCotizacionWriter.CloseCotizacionAsyn(args.CodigoCotizacion);
                if (informacionNegocio.CodigoEstadoCotizacion < 1300) {
                    await this.cotizacionStateUpdater.UpdateCotizacionStateAsync(args.CodigoCotizacion, CotizacionState.Sent);
                }

                var message = "Envío de Slip";
                await this.cotizacionTransactionsProvider.CreateTransactionAsync(args.CodigoCotizacion, (int)args.Version, userName, message);

                return new SendEmailResponse
                {
                    // TODO complete
                };
            }
            catch (Exception ex)
            {
                throw new Exception("SlipDataProvider :: SendCotizacionSlipAsync", ex);
            }
        }
        public async Task<SendEmailResponse> GetCotizacionSlipPDFAsync(SendSlipArgs args, string userName)
        {
            try
            {
                var adjuntos = await this.datosEnvioSlipReader.LeerAdjuntoEnvioSlipAsync(args.CodigoCotizacion);
                await this.SaveTemporalAttachment(args.CodigoCotizacion, adjuntos);
                string slipPathFile = "";
                foreach (var adjunto in adjuntos)
                {
                    if (adjunto.FileName.Contains("Slip_Cotización_#"))
                    {
                        slipPathFile = Path.Combine(basePath, args.CodigoCotizacion.ToString(), adjunto.FileName);
                    }
                }
                Byte[] bytes = File.ReadAllBytes(slipPathFile);
                return new SendEmailResponse
                {
                    HasError = false,
                    RutaPDF = Convert.ToBase64String(bytes)
                };

            }
            catch (Exception ex)
            {
                throw new Exception("SlipDataProvider :: SendCotizacionSlipAsync", ex);
            }
        }

        private async Task SaveEnvioCotizacionSlip(SendSlipArgs args)
        {
            var emailTo = string.Join(',', args.Recipients);
            var emailCC = ""; //TODO define value
            var emailComments = args.Comments;
            await this.datosEnvioSlipWriter.CrearEnvioSlipAsync(args.CodigoCotizacion, emailTo, emailCC, emailComments);
            await this.datosCotizacionWriter.UpdateEnvioSlipCotizacionAsync(args.CodigoCotizacion);
        }

        private async Task SendSlipIntermediarioComercialAsync(SendSlipArgs args, string nombreTomador, string nombreRamo, string userName)
        {
            var codigoCotizacion = this.informacionNegocio.CodigoCotizacion;
            var numeroCotizacion = this.informacionNegocio.NumeroCotizacion;
            var agenciaNombre = await this.GetAgencia(codigoCotizacion);
            var sucursal = await this.informacionPersonasReader.TraerSucursalAsync(this.informacionNegocio.CodigoSucursal);
            var contacts = await this.soligesproUsersDataReader.GetDirectoresAsync(sucursal.CodigoSucursal);
            contacts = contacts.Where(x => x.CodigoCargo != 9 && x.CodigoCargo != 16).Concat(contacts.Where(x => x.CodigoCargo == 16 && x.NombreUsuario == this.informacionNegocio.NombreDirectorComercial));
            var templateName = "send_slip_inter_come_template.html";
            var bodyTemplate = await this.emailsDataProvider.GetTextosEmailByTemplate(7, 2);
            var data = new
            {
                bodyTemplate = bodyTemplate.Texto,
                comments = args.Comments,
                sucursal = sucursal.NombreSucursal,
                contacts = contacts,
                agencia = agenciaNombre.NombreSucursal
            };

            var adjuntos = await this.datosEnvioSlipReader.LeerAdjuntoEnvioSlipAsync(args.CodigoCotizacion);
            

            var cc_emails = contacts.Select(x => x.EmailUsuario).ToArray();
            var cc = new string[] { "director-comercial@solidaria.com.co", "gerente-agencia@solidaria.com.co" };
            var cco = new string[] { "ext.svalencia@solidaria.com.co" };
            var body = HtmlTemplateBuilder.BuildHtmlTemplate(data, templateName);
            var subject = $"PROPUESTA COTIZACION TOMADOR - {nombreTomador} RAMO – {nombreRamo} Cotización {Convert.ToInt32(numeroCotizacion)} V{args.Version}";
            // Slip de cotización, Condicionado general del ramo
            var attachments = new List<string>();
            List<AdjuntoEnvioSlip> adjuntosFinal = new List<AdjuntoEnvioSlip>();
            foreach (var adjunto in adjuntos)
            {
                if (!adjunto.FileName.Contains("248699"))
                {
                    var attachmentPath = Path.Combine(basePath, codigoCotizacion.ToString(), adjunto.FileName);
                    attachments.Add(attachmentPath);
                    adjuntosFinal.Add(adjunto);
                }

            }
            await this.SaveTemporalAttachment(args.CodigoCotizacion, adjuntosFinal);
            // add carta aceptacion si existe
            var cartaPath = $@"{basePath}\{codigoCotizacion}\{numeroCotizacion}_CartaAceptacion.pdf";
            if (File.Exists(cartaPath))
            {
                attachments.Add(cartaPath);
            }

            var sendEmailArgs = new SendEmailArgs
            {
                Attachments = attachments,
                Body = body,
                //CC = args.WithCopy,
                CCO = args.WithCopy,
                Recipients = args.Recipients,
                Subject = subject
            };

            await this.emailSender.SendEmailUsingTemplateAsync(sendEmailArgs);

            // delete temporal file
            //foreach (var attachment in attachments)
            //{
            //    File.Delete(attachment);
            //}
        }

        private async Task SendSlipTomadorAsync(SendSlipArgs args, string nombreTomador, string nombreRamo, string userName)
        {
            var codigoCotizacion = this.informacionNegocio.CodigoCotizacion;
            var numeroCotizacion = this.informacionNegocio.NumeroCotizacion;
            var agenciaNombre = await this.GetAgencia(codigoCotizacion);
            var sucursal = await this.informacionPersonasReader.TraerSucursalAsync(this.informacionNegocio.CodigoSucursal);
            var templateName = "send_slip_tomador_template.html";
            var bodyTemplate = await this.emailsDataProvider.GetTextosEmailByTemplate(8, 2);
            var contacts = await this.soligesproUsersDataReader.GetDirectoresAsync(sucursal.CodigoSucursal);
            contacts = contacts.Where(x => x.CodigoCargo != 9 && x.CodigoCargo != 16).Concat(contacts.Where(x => x.CodigoCargo == 16 && x.NombreUsuario == this.informacionNegocio.NombreDirectorComercial));
            var data = new
            {
                bodyTemplate = bodyTemplate.Texto,
                comments = args.Comments,
                sucursal = sucursal.NombreSucursal,
                contacts = contacts,
                agencia = agenciaNombre.NombreSucursal
            };

            var adjuntos = await this.datosEnvioSlipReader.LeerAdjuntoEnvioSlipAsync(args.CodigoCotizacion);
            

            var cc_emails = contacts.Select(x => x.EmailUsuario).ToArray();
            // TODO obtener correos CC de contacts
            var cc = new string[] { "intermediario@solidaria.com.co", "director-comercial@solidaria.com.co", "gerente-agencia@solidaria.com.co" };
            var cco = new string[] { "ext.jmalagon@solidaria.com.co" };
            var body = HtmlTemplateBuilder.BuildHtmlTemplate(data, templateName);
            var subject = $"PROPUESTA COTIZACION TOMADOR - {nombreTomador} RAMO – {nombreRamo} Cotización {Convert.ToInt32(numeroCotizacion)} V{args.Version}";
            // Slip de condiciones, Condicionado General del ramo
            var attachments = new List<string>();
            List<AdjuntoEnvioSlip> adjuntosFinal = new List<AdjuntoEnvioSlip>();
            foreach (var adjunto in adjuntos)
            {
                if (!adjunto.FileName.Contains("248699"))
                {
                    var attachmentPath = Path.Combine(basePath, codigoCotizacion.ToString(), adjunto.FileName);
                    attachments.Add(attachmentPath);
                    adjuntosFinal.Add(adjunto);
                }

            }
            await this.SaveTemporalAttachment(args.CodigoCotizacion, adjuntosFinal);

            // add carta aceptacion si existe
            var cartaPath = $@"{basePath}\{codigoCotizacion}\{numeroCotizacion}_CartaAceptacion.pdf";
            if (File.Exists(cartaPath))
            {
                attachments.Add(cartaPath);
            }

            var sendEmailArgs = new SendEmailArgs
            {
                Attachments = attachments,
                Body = body,
                //CC = args.WithCopy,
                CCO = args.WithCopy,
                Recipients = args.Recipients,
                Subject = subject
            };

            await this.emailSender.SendEmailUsingTemplateAsync(sendEmailArgs);

            // delete temporal file
            //foreach (var attachment in attachments)
            //{
            //    File.Delete(attachment);
            //}
        }

        private async Task SaveTemporalAttachment(int codigoCotizacion, IEnumerable<AdjuntoEnvioSlip> adjuntos)
        {
            var envios = await this.datosEnvioSlipReader.LeerEnvioSlipAsync(codigoCotizacion);
            var last = envios.LastOrDefault();
            //var adjunto = adjuntos.LastOrDefault();

            var directoryPath = $@"{ this.basePath}\{ codigoCotizacion}";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var countFN = 1;

            foreach (var adjunto in adjuntos)
            {
                if (adjunto.FileName == null)
                {
                    adjunto.FileName = "Archivo No." + countFN.ToString();
                    countFN += 1;
                }
                var path = $@"{this.basePath}\{codigoCotizacion}\{adjunto.FileName}";
                using (MemoryStream ms = new MemoryStream(adjunto.Adjunto))
                using (var fileStream = File.Create(path))
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    for (int i = 0; i < adjunto.Adjunto.Length; i++)
                    {
                        byte result = reader.ReadByte();
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    ms.CopyTo(fileStream);
                }
            }
        }

        private async Task<GruposAseguradosSlip> GetGruposAseguradosSlipAsync(IEnumerable<GrupoAsegurado> gruposAsegurados)
        {
            var codigoTipoTasa1 = this.informacionNegocio.CodigoTipoTasa1;
            var codigoTipoTasa2 = this.informacionNegocio.CodigoTipoTasa2;
            decimal valorSalarioBasico = 0;
            int tipoValor = 0;// 0 = moneda,  1 = Sueldos, 2 = porcentaje, 3 = nada
            var amparos = await this.informacionPersonasReader.TraerAmparosAsync(this.informacionNegocio.CodigoRamo, this.informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector);
            var tiposumas = await this.informacionPersonasReader.TraerTiposSumaAsegurada(this.informacionNegocio.CodigoRamo, this.informacionNegocio.CodigoSubramo);
            var gruposAseguradosSlip = new GruposAseguradosSlip();
            foreach (GrupoAsegurado grupo in gruposAsegurados)
            {
                this.primasDataProcessor = this.primasDataProcessorFactory.Resolve(codigoTipoTasa1, codigoTipoTasa2, grupo.CodigoTipoSuma);
                var grupoAseguradoSlip = new GrupoAseguradoSlip
                {
                    CodigoGrupoAsegurado = grupo.CodigoGrupoAsegurado,
                    Nombre = grupo.NombreGrupoAsegurado,
                };

                var amparosGrupo = await this.amparoGrupoAseguradoReader.LeerAmparoGrupoAseguradoAsync(grupo.CodigoGrupoAsegurado);

                foreach (var amparoGrupo in amparosGrupo)
                {
                    amparoGrupo.AmparoInfo = amparos.Where(x => x.CodigoAmparo == amparoGrupo.CodigoAmparo).FirstOrDefault();
                    var amparoSlip = new ValoresAseguradosAmparoSlip
                    {
                        CodigoTipoSumaAsegurada = grupo.CodigoTipoSuma,
                        NombreAmparo = amparoGrupo.AmparoInfo.NombreAmparo
                    };
                    amparoGrupo.CodigoGrupoAmparo = amparoGrupo.AmparoInfo.CodigoGrupoAmparo;
                }

                amparosGrupo = amparosGrupo.OrderBy(x => x.CodigoGrupoAmparo).ThenByDescending(p => p.AmparoInfo.SiNoBasico).ThenBy(m => m.AmparoInfo.SiNoAdicional).ThenBy(n => n.AmparoInfo.NombreAmparo).ToList();


                grupo.AmparosGrupo = amparosGrupo.ToList();


                decimal valorAsegTotalBasico = 0;
                decimal salariosBasico = 0;
                foreach (var amparoGrupo in grupo.AmparosGrupo)
                {
                    amparoGrupo.AmparoInfo = amparos.Where(x => x.CodigoAmparo == amparoGrupo.CodigoAmparo).FirstOrDefault();
                    var amparoSlip = new ValoresAseguradosAmparoSlip
                    {
                        CodigoTipoSumaAsegurada = grupo.CodigoTipoSuma,
                        NombreAmparo = amparoGrupo.AmparoInfo.NombreAmparo
                    };

                    // Leer Opciones Valor
                    var opcionValores = await this.opcionValorReader.LeerOpcionValorAseguradoAsync(amparoGrupo.CodigoAmparoGrupoAsegurado);
                    if (grupo.ConDistribucionAsegurados)
                    {
                        grupo.AseguradosOpcion1 = opcionValores.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                        grupo.AseguradosOpcion2 = opcionValores.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                        grupo.AseguradosOpcion3 = opcionValores.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                    }


                    amparoGrupo.OpcionesValores = opcionValores.ToList();
                    decimal valorAsegTotal = 0;
                    decimal[] valoresAmparo = new decimal[3]; // Dimensión de valores para 3 opciones
                    var count = 0; // Contador opciones

                    foreach (var opcion in opcionValores)
                    {
                        // Valor Max Asegurado Grupo
                        if (grupo.CodigoTipoSuma == 1)
                        {
                            if (amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false)
                            {
                                valoresAmparo[count] = opcion.ValorAsegurado;
                            }
                        }
                        else
                        {
                            valoresAmparo[count] = grupo.ValorMaxAsegurado;
                        }

                        decimal vai = 0;
                        decimal valdia = 0;
                        decimal dias = 0;
                        if (grupo.NumeroAsegurados != 0)
                        {
                            valdia = opcion.ValorDiario;
                            dias = opcion.NumeroDias;
                            // Calcular Valor Individual para tablas valores Slip
                            // SUMA VARIABLE
                            if (grupo.CodigoTipoSuma == 3)
                            {
                                if (amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false)
                                {
                                    vai = 0;
                                    tipoValor = 3;
                                }
                                else
                                {
                                    if (amparoGrupo.AmparoInfo.CodigoAmparo == 3)
                                    {
                                        vai = opcion.PorcentajeCobertura;
                                        tipoValor = 2;
                                    }
                                    else
                                    {
                                        if (opcion.PorcentajeCobertura > 0)
                                        {
                                            vai = opcion.PorcentajeCobertura;
                                            tipoValor = 2;
                                        }
                                        else if (opcion.ValorAsegurado > 0)
                                        {
                                            vai = opcion.ValorAsegurado;
                                            tipoValor = 0;
                                        }
                                        else if (opcion.NumeroSalarios > 0)
                                        {
                                            vai = opcion.ValorAsegurado;
                                            tipoValor = 1;
                                        }
                                        else if (amparoGrupo.CodigoAmparo == 95)
                                        {
                                            vai = opcion.Prima;
                                            tipoValor = 0;
                                        }

                                    }

                                }
                            }
                            // MULTIPLOS DE SUELDO 
                            else if (grupo.CodigoTipoSuma == 2)
                            {
                                if (amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false)
                                {
                                    valorSalarioBasico = opcion.NumeroSalarios;
                                }
                                if (opcion.PorcentajeCobertura > 0)
                                {
                                    if ((amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false))
                                    {
                                        vai = opcion.NumeroSalarios;
                                        tipoValor = 1;
                                    }
                                    else
                                    {
                                        vai = opcion.PorcentajeCobertura;
                                        tipoValor = 2;
                                    }

                                }
                                else
                                {
                                    if ((amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false))
                                    {
                                        vai = opcion.NumeroSalarios;
                                        tipoValor = 1;
                                    }
                                    else
                                    {
                                        vai = amparoGrupo.CodigoAmparo == 95 ? opcion.Prima : opcion.NumeroSalarios;
                                        tipoValor = amparoGrupo.CodigoAmparo == 95 ? 0 : 1;
                                        if (amparoGrupo.AmparoInfo.Modalidad.Codigo == 4)
                                        {
                                            vai = opcion.ValorAsegurado;
                                            tipoValor = 0;
                                        }
                                    }

                                }
                            }
                            // SMMLV
                            else if (grupo.CodigoTipoSuma == 10)
                            {
                                if (amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false)
                                {
                                    valorSalarioBasico = opcion.NumeroSalarios;
                                }
                                if (opcion.PorcentajeCobertura > 0)
                                {
                                    if ((amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false))
                                    {
                                        vai = opcion.NumeroSalarios;
                                        tipoValor = 1;
                                    }
                                    else
                                    {
                                        vai = opcion.PorcentajeCobertura;
                                        tipoValor = 2;
                                    }

                                }
                                else
                                {
                                    if ((amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false))
                                    {
                                        vai = opcion.NumeroSalarios;
                                        tipoValor = 1;
                                    }
                                    else
                                    {
                                        vai = amparoGrupo.CodigoAmparo == 95 ? opcion.Prima : opcion.NumeroSalarios > 0 ? opcion.NumeroSalarios : opcion.ValorAsegurado;
                                        tipoValor = opcion.NumeroSalarios > 0 ? 1 : 0;
                                    }

                                }
                            }
                            // SUMA FIJA Y MULTIPLOS SUELDO
                            else if (grupo.CodigoTipoSuma == 5 || grupo.CodigoTipoSuma == 1)
                            {


                                var tipoValorBasico = 0;
                                if (amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false)
                                {
                                    if (opcion.NumeroSalarios > 0)
                                    {
                                        valorSalarioBasico = opcion.NumeroSalarios;
                                        tipoValorBasico = 1;
                                    }
                                    else
                                    {
                                        valorSalarioBasico = opcion.ValorAsegurado;
                                        tipoValorBasico = 0;
                                    }
                                }
                                if (opcion.PorcentajeCobertura > 0)
                                {
                                    if ((amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false))
                                    {
                                        vai = opcion.NumeroSalarios > 0 ? opcion.NumeroSalarios : opcion.ValorAsegurado;
                                        tipoValor = opcion.NumeroSalarios > 0 ? 1 : 0;
                                    }
                                    else
                                    {
                                        if (amparoGrupo.AmparoInfo.CodigoAmparo == 3)
                                        {
                                            vai = opcion.PorcentajeCobertura;
                                            tipoValor = 2;
                                        }
                                        else
                                        {
                                            vai = opcion.PorcentajeCobertura;
                                            tipoValor = 2;
                                        }

                                    }

                                }
                                else
                                {
                                    if ((amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false))
                                    {
                                        vai = opcion.NumeroSalarios > 0 ? opcion.NumeroSalarios : opcion.ValorAsegurado;
                                        tipoValor = opcion.NumeroSalarios > 0 ? 1 : 0;
                                    }
                                    else
                                    {
                                        vai = opcion.ValorAsegurado > 0 ? opcion.ValorAsegurado : amparoGrupo.CodigoAmparo == 95 ? opcion.Prima : opcion.NumeroSalarios;
                                        tipoValor = opcion.ValorAsegurado > 0 ? 0 : amparoGrupo.CodigoAmparo == 95 ? 0 : 1;
                                    }

                                }

                            }

                            // SALDO DEUDORES
                            else if (grupo.CodigoTipoSuma == 6)
                            {
                                if (opcion.PorcentajeCobertura > 0)
                                {

                                    vai = opcion.PorcentajeCobertura;
                                    tipoValor = 2;

                                }
                                else
                                {
                                    if ((amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false))
                                    {
                                        vai = 0;
                                        tipoValor = 3;
                                    }
                                    else
                                    {
                                        vai = amparoGrupo.CodigoAmparo == 95 ? opcion.Prima : opcion.ValorAsegurado;
                                        tipoValor = 0;
                                    }
                                }

                            }


                            if ((amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == false))
                            {
                                if (grupo.CodigoTipoSuma == 5)
                                {
                                    valorAsegTotal = opcion.NumeroSalarios > 0 ? grupo.ValorAsegurado * opcion.NumeroSalarios : opcion.ValorAsegurado * grupo.NumeroAsegurados;
                                }
                                else
                                {
                                    valorAsegTotal = grupo.CodigoTipoSuma == 10 ? opcion.ValorAsegurado * grupo.NumeroAsegurados : opcion.ValorAsegurado;
                                }
                                valorAsegTotalBasico = valorAsegTotal;
                                salariosBasico = opcion.NumeroSalarios;
                            }
                            else if ((amparoGrupo.AmparoInfo.SiNoBasico == true && amparoGrupo.AmparoInfo.SiNoAdicional == true))
                            {

                                if (opcion.PorcentajeCobertura > 0)
                                {
                                    valorAsegTotal = grupo.CodigoTipoSuma == 5 || grupo.CodigoTipoSuma == 2 ? (valorAsegTotalBasico * opcion.PorcentajeCobertura) / 100 : grupo.CodigoTipoSuma == 10 ? ((grupo.ValorAsegurado * opcion.PorcentajeCobertura) / 100) * grupo.NumeroAsegurados : opcion.ValorAsegurado;
                                }
                                else
                                {
                                    if (opcion.NumeroSalarios > 0)
                                    {
                                        valorAsegTotal = grupo.CodigoTipoSuma == 5 || grupo.CodigoTipoSuma == 2
                                                        ? amparoGrupo.Modalidad.Codigo == 4 ?
                                                        valorAsegTotalBasico * grupo.NumeroAsegurados :
                                                        (valorAsegTotalBasico * opcion.NumeroSalarios) / salariosBasico
                                                        : grupo.CodigoTipoSuma == 1
                                                        ? opcion.ValorAsegurado
                                                        : opcion.ValorAsegurado * grupo.NumeroAsegurados;
                                    }
                                    else
                                    {
                                        valorAsegTotal = grupo.CodigoTipoSuma == 1
                                                        ? opcion.ValorAsegurado
                                                        : amparoGrupo.Modalidad.Codigo == 4 ?
                                                        valorAsegTotalBasico * grupo.NumeroAsegurados :
                                                        opcion.ValorAsegurado * grupo.NumeroAsegurados;
                                    }

                                }
                            }
                            else
                            {
                                if (opcion.PorcentajeCobertura > 0)
                                {
                                    valorAsegTotal = grupo.CodigoTipoSuma == 5 || grupo.CodigoTipoSuma == 2 ? (valorAsegTotalBasico * opcion.PorcentajeCobertura) / 100 : grupo.CodigoTipoSuma == 10 ? ((grupo.ValorAsegurado * opcion.PorcentajeCobertura) / 100) * grupo.NumeroAsegurados : opcion.ValorAsegurado;
                                }
                                else
                                {
                                    valorAsegTotal = grupo.CodigoTipoSuma == 5 ?
                                                    opcion.NumeroSalarios > 0 ?
                                                    (valorAsegTotalBasico / grupo.NumeroAsegurados) * opcion.NumeroSalarios :
                                                    opcion.ValorAsegurado * grupo.NumeroAsegurados : grupo.CodigoTipoSuma == 1 ?
                                                    amparoGrupo.CodigoAmparo == 95 ?
                                                    opcion.Prima :
                                                    opcion.ValorAsegurado :
                                                    grupo.CodigoTipoSuma == 2 ?
                                                    amparoGrupo.CodigoAmparo == 98 || amparoGrupo.CodigoAmparo == 11 ?
                                                    opcion.ValorAsegurado * grupo.NumeroAsegurados :
                                                    (valorAsegTotalBasico * opcion.NumeroSalarios) / salariosBasico :
                                                    grupo.CodigoTipoSuma == 10 ?
                                                    opcion.NumeroSalarios > 0 ?
                                                    (this.valorSalarioMinimo * opcion.NumeroSalarios) * grupo.NumeroAsegurados :
                                                    opcion.ValorAsegurado * grupo.NumeroAsegurados :
                                                    opcion.ValorAsegurado * grupo.NumeroAsegurados;

                                }
                            }
                        }

                        //valorAsegTotal = opcion.ValorAsegurado;

                        var opcionesValor = new OpcionValoresAseguradosAmparoSlip
                        {
                            ValorAsegurado = Math.Round(valorAsegTotal, 0),
                            ValorAseguradoIndividual = vai,
                            ValorDiario = valdia,
                            NumeroDias = dias,
                            TipoValor = tipoValor,
                            TablaValoresDiarios = dias > 0? true: false
                        };

                        amparoSlip.Opciones.Add(opcionesValor);
                        count += 1; // Se aumenta el contador.
                    }

                    // Autorización para suma asegurada    
                    var valorAsegAuth = valoresAmparo.Max();

                    grupoAseguradoSlip.valorMaximo.Texto = grupoAseguradoSlip.valorMaximo.Texto.Replace("$var_suma_aseg_maxima", valorAsegAuth.ToString("c0"));

                    // Set variable snTasaMensual para determinar si el grupo muestra la tasa Mensual en vez de la prima.
                    var sumaAsegurada = tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == grupo.CodigoTipoSuma).FirstOrDefault();

                    grupoAseguradoSlip.snTasaMensual = sumaAsegurada.SiNoTasaMensualSlip;



                    amparoSlip.CountOpciones = opcionValores.Count();
                    // Leer Edades
                    var informacionEdadesAmparo = await this.edadesReader.LeerEdadesAsync(amparoGrupo.CodigoGrupoAsegurado, amparoGrupo.CodigoAmparo);
                    var edadesSlip = new EdadAmparoSlip
                    {
                        EdadMinimaIngreso = informacionEdadesAmparo.EdadMinAsegurado,
                        EdadMaximaIngreso = informacionEdadesAmparo.EdadMaxAsegurado,
                        EdadMaximaPermanencia = informacionEdadesAmparo.edadMaxPermanencia,
                        NombreAmparo = amparoGrupo.AmparoInfo.NombreAmparo
                    };

                    // Amparos en Grupo
                    grupoAseguradoSlip.ValoresAmparos.Add(amparoSlip);
                    grupoAseguradoSlip.Edades.Add(edadesSlip);
                }

                var primaTotalOpciones = await this.MapPrimaTotalOpcionesAsync(grupo);
                var primaIndividualOpciones = await this.MapPrimaIndividualOpcionesAsync(grupo);

                if (grupo.CodigoTipoSuma == 1)
                {
                    grupoAseguradoSlip.ValoresAmparos.Add(primaIndividualOpciones);
                }
                grupoAseguradoSlip.ValoresAmparos.Add(primaTotalOpciones);
                gruposAseguradosSlip.GruposAsegurados.Add(grupoAseguradoSlip);
            }

            //Calcular tasa general
            decimal sumaPrimas = 0;
            decimal sumavalores = 0;
            var fichaProvider = this.fichaProvider.GenerateAsync(gruposAsegurados.FirstOrDefault().CodigoCotizacion, informacionNegocio.LastAuthorName).Result;

            foreach (var gr in gruposAseguradosSlip.GruposAsegurados)
            {
                foreach (var grFt in fichaProvider.Data.GruposAsegurados)
                {
                    foreach (var valoresO in gr.ValoresAmparos)
                    {
                        var cont = 1;
                        foreach (var opAmp in valoresO.Opciones)
                        {

                            foreach (var opFt in grFt.Primas.PrimaTotalAnual)
                            {

                                if (valoresO.NombreAmparo == "PRIMA ANUAL TOTAL")
                                {
                                    if (opFt.IndiceOpcion == cont && gr.CodigoGrupoAsegurado == grFt.Codigo)
                                    {
                                        opAmp.ValorAsegurado = Math.Round(opFt.Valor, 0);
                                    }
                                }

                            }
                            cont++;
                        }
                    }

                }
            }

            return gruposAseguradosSlip;
        }

        private async Task<ValoresAseguradosAmparoSlip> MapPrimaTotalOpcionesAsync(GrupoAsegurado grupo)
        {
            var optionsCount = grupo.CodigoTipoSuma == 1 ? 3 : 1;
            var result = new ValoresAseguradosAmparoSlip
            {
                CodigoTipoSumaAsegurada = grupo.CodigoTipoSuma,
                NombreAmparo = "PRIMA ANUAL TOTAL"
            };

            var tipoSumaAsegurada = this.tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == grupo.CodigoTipoSuma).FirstOrDefault();
            var codigoTipoTasa1 = informacionNegocio.CodigoTipoTasa1;
            var codigoTipoTasa2 = informacionNegocio.CodigoTipoTasa2;
            var codigoTipoTasa = codigoTipoTasa1 == 5 ? codigoTipoTasa2 : codigoTipoTasa1;
            var tieneSiniestralidad = codigoTipoTasa1 == 5 || codigoTipoTasa2 == 5;
            var args = new CotizacionDataProcessorArgs
            {
                CodigoCotizacion = informacionNegocio.CodigoCotizacion,
                IBNR = informacionNegocio.IBNR,
                FactorG = informacionNegocio.FactorG,
                TipoSumaAsegurada = tipoSumaAsegurada,
                CodigoTipoTasa = codigoTipoTasa,
                TieneSiniestralidad = tieneSiniestralidad,
                ValorSalarioMinimo = valorSalarioMinimo
            };

            var dataProcessor = this.cotizacionDataProcessorFactory.Resolve(args);
            var factorg = informacionNegocio.FactorG / 100;

            var sumatoriaPrimaAmparoOpciones = this.GetSumatoriaPrimaAmparoOpcion(grupo.AmparosGrupo);
            // aggregate prima total anual
            for (int i = 0; i < optionsCount; i++)
            {
                var tasaopcion = this.tasaOpcionValorReader.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, i + 1).Result;
                var valorAsistencia = this.GetValorAsistencia(grupo.AmparosGrupo, i);
                valorAsistencia = valorAsistencia * grupo.NumeroAsegurados;
                valorAsistencia = (valorAsistencia * (decimal)1.19) / (1 - factorg);
                var sumPrimaAmparoOpcion = sumatoriaPrimaAmparoOpciones[i];
                //    var primas = await dataProcessor.CalcularPrimasGrupoAseguradoFichaTecnicaAsync(informacionNegocio, grupo);
                
                var primaValor = new OpcionValoresAseguradosAmparoSlip
                {
                    ValorAsegurado = tasaopcion.PrimaTotal,
                    tasaMensual = tieneSiniestralidad ? tasaopcion.TasaComercialTotal < tasaopcion.TasaSiniestralidadTotal ? tasaopcion.TasaSiniestralidadTotal / 12 : tasaopcion.TasaComercialTotal / 12 : tasaopcion.TasaComercialTotal / 12

                };

                result.Opciones.Add(primaValor);
            }

            return result;
        }

        private async Task<ValoresAseguradosAmparoSlip> MapPrimaIndividualOpcionesAsync(GrupoAsegurado grupo)
        {
            var optionsCount = grupo.CodigoTipoSuma == 1 ? 3 : 1;
            var result = new ValoresAseguradosAmparoSlip
            {
                CodigoTipoSumaAsegurada = grupo.CodigoTipoSuma,
                NombreAmparo = "PRIMA ANUAL INDIVIDUAL"
            };

            var tipoSumaAsegurada = this.tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == grupo.CodigoTipoSuma).FirstOrDefault();
            var codigoTipoTasa1 = informacionNegocio.CodigoTipoTasa1;
            var codigoTipoTasa2 = informacionNegocio.CodigoTipoTasa2;
            var codigoTipoTasa = codigoTipoTasa1 == 5 ? codigoTipoTasa2 : codigoTipoTasa1;
            var tieneSiniestralidad = codigoTipoTasa1 == 5 || codigoTipoTasa2 == 5;
            var args = new CotizacionDataProcessorArgs
            {
                CodigoCotizacion = informacionNegocio.CodigoCotizacion,
                IBNR = informacionNegocio.IBNR,
                FactorG = informacionNegocio.FactorG,
                TipoSumaAsegurada = tipoSumaAsegurada,
                CodigoTipoTasa = codigoTipoTasa,
                TieneSiniestralidad = tieneSiniestralidad,
                ValorSalarioMinimo = valorSalarioMinimo
            };

            var dataProcessor = this.cotizacionDataProcessorFactory.Resolve(args);

            var sumatoriaPrimaAmparoOpciones = this.GetSumatoriaPrimaAmparoOpcion(grupo.AmparosGrupo);
            // aggregate prima total anual
            for (int i = 0; i < optionsCount; i++)
            {
                var tasaopcion = this.tasaOpcionValorReader.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, i + 1).Result;
                var valorAsistencia = this.GetValorAsistencia(grupo.AmparosGrupo, i);
                var sumPrimaAmparoOpcion = sumatoriaPrimaAmparoOpciones[i];
                //var primas = await dataProcessor.CalcularPrimasGrupoAseguradoFichaTecnicaAsync(informacionNegocio, grupo);

                var primaValor = new OpcionValoresAseguradosAmparoSlip
                {
                    ValorAsegurado = tasaopcion.PrimaIndividual
                };

                result.Opciones.Add(primaValor);
            }

            return result;
        }

        private decimal GetValorAsistencia(ICollection<AmparoGrupoAsegurado> amparosGrupo, int opcionIndex)
        {
            decimal valorAsistenciaIndividual = 0;
            var asistencia = amparosGrupo.Where(x => x.AmparoInfo.CodigoGrupoAmparo == 3).FirstOrDefault();
            if (asistencia != null && asistencia.OpcionesValores.Count > 0)
            {
                valorAsistenciaIndividual = asistencia.OpcionesValores[opcionIndex].Prima;
            }

            return valorAsistenciaIndividual;
        }

        private Dictionary<int, decimal> GetSumatoriaPrimaAmparoOpcion(ICollection<AmparoGrupoAsegurado> amparosGrupo)
        {
            var count = 3;
            var result = new Dictionary<int, decimal>();
            var totalOpciones = new decimal[count];
            foreach (var a in amparosGrupo)
            {
                // No se debe sumar si el amparo es de tipo asistencia, CodigoGrupoAsegurado 95
                if (a.CodigoAmparo != 95)
                {
                    a.OpcionesValores.ForEach(o =>
                    {
                        totalOpciones[o.IndiceOpcion - 1] += o.Prima;
                    });
                }
            }

            for (int i = 0; i < count; i++)
            {
                result.Add(i, totalOpciones[i]);
            }

            return result;
        }

        private string GetHeaderImage()
        {
            switch (this.informacionNegocio.CodigoRamo)
            {
                // ACC. PERSONALES
                case 1:
                // ACC. PERS. PLAT
                case 37:
                    return "assets/header_acc_personales.png";
                // ACC. ESCOLARES
                case 2:
                    return "assets/header_acc_escolares.png";
                // VIDA GRUPO
                case 15:
                    return "assets/header_vidagrupo.png";
                // VIDA GRUPO DEUDORES
                case 16:
                    return "assets/header_vidagrupo_deudores.png";
                default:
                    return "";
            }
        }

        private string GetFooterImage()
        {
            return "assets/footer_acc_escolares.png";
        }
        public async Task<PersonasServiceReference.Sucursal> GetAgencia(int codigoCotizacion)
        {
            var infoNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var sucursal = await this.informacionPersonasReader.TraerSucursalAsync(infoNegocio.CodigoSucursal);
            return sucursal;
        }
        private async Task<GetTextosSeccionSlipResponse> LeerTextosSlipAsync(int codigoCotizacion, int version, int sector, int subramo)
        {
            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var ramo = informacionNegocio.CodigoRamo;
            var response = await this.slipReader.LeerTextosSlipAsync(codigoCotizacion, ramo, sector, subramo);
            var grupos = this.gruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion).Result;
            var gruposEnfermedades = new List<GrupoAsegurado>();
            foreach (var grupo in grupos)
            {
                var grupoMap = this.gruposAseguradosMapper.ConsultarGrupoAseguradoAsync(codigoCotizacion, informacionNegocio.Version, grupo.CodigoGrupoAsegurado).Result;
                grupo.AmparosGrupo = grupoMap.AmparosGrupo;
                var ampenfer = grupo.AmparosGrupo.Where(x => x.CodigoAmparo == 3);
                if (ampenfer.Count() > 0)
                {
                    gruposEnfermedades.Add(grupo);
                }
            }
            if (gruposEnfermedades.Count() > 0)
            {
                var valorEnfermedades = gruposEnfermedades.FirstOrDefault().AmparosGrupo.Where(x => x.CodigoAmparo == 3).FirstOrDefault().OpcionesValores.FirstOrDefault().PorcentajeCobertura;
                var enfermedadesTxt = Math.Round(valorEnfermedades, 0).ToString();

                foreach (var amparo in response.Amparos.Amparos)
                {
                    if (amparo.CodigoAmparo == "3" && enfermedadesTxt != null)
                    {
                        amparo.Texto = amparo.Texto.Replace("var_enfermedades", enfermedadesTxt);

                    }
                }
            }


            // TODO implemetar consulta de variables por amparo
            return response;
        }

        private VigenciaSlip ObtenerVigenciaSlip(DateTime? desde, DateTime? hasta)
        {
            var today = DateTime.Now;
            return new VigenciaSlip
            {
                Desde = desde,
                Hasta = hasta
            };
        }

        private string ObtenerImagenProductoSlip(int codigoCotizacion)
        {
            return "data:image/png;base64,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";
        }

        private string ObtenerCiudadExpedicionSlip()
        {
            return "Bogotá";
        }

        private string ObtenerDescripcionSlip(int codigoCotizacion)
        {
            return @"Vida Grupo Deudores, es un seguro de carácter voluntario cuyo objeto básico es indemnizar al Tomador (acreedor) 
                        hasta por el valor de la obligación o crédito insoluto del Asegurado, cuando se presente el fallecimiento o la incapacidad 
                        total y permanente";
        }
    }

    public class GenerarSlipConfiguracionResponse : ActionResponseBase
    {
        public SlipConfiguracion Data { get; set; }
    }

    public class GenerarSlipDataResponse : ActionResponseBase
    {
        public Slip Data { get; set; }
    }

}

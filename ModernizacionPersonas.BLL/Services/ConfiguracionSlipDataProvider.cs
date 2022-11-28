using Microsoft.AspNetCore.Http;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class ConfiguracionSlipDataProvider
    {
        private readonly IDatosCotizacionWriter cotizacionWriter;
        private readonly IDatosCotizacionReader cotizacionReader;
        private readonly IDatosAmparoGrupoAseguradoReader amparoGrupoAseguradoReader;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosIntermediarioWriter datosIntermediarioWriter;
        private readonly IDatosGruposAseguradoReader gruposAseguradosReader;
        private readonly IDatosTomadorReader datosTomadorReader;
        private readonly IDatosTomadorWriter datosTomadorWriter;
        private readonly IDatosIntermediarioReader intermediarioReader;
        private readonly DatosGruposAseguradosMapper gruposAseguradosMapper;
        private readonly DatosParametrizacionReader parametrizacionReader;
        private readonly IDatosSlipReader slipReader;
        private readonly IDatosSlipWriter slipWriter;
        private readonly IDatosAsegurabilidadReader asegurabilidadReader;
        private readonly IDatosAsegurabilidadWriter asegurabilidadWriter;
        private readonly CotizacionStateWriter cotizacionStateWriter;
        private readonly IDatosAseguradoReader aseguradoReader;
        private readonly IAuthorizationsDataWriter authWriter;
        private readonly IDatosInformacionNegocioWriter informacionNegocioWriter;
        private readonly ITransactionsDataReader trasactionsReader;
        private IEnumerable<Amparo> amparos;
        private PdfCartaAceptacionService pdfService;
      

        public ConfiguracionSlipDataProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.cotizacionWriter = new DatosCotizacionTableWriter();
            this.cotizacionReader = new DatosCotizacionTableReader();
            this.amparoGrupoAseguradoReader = new DatosAmparoGrupoAseguradoTableReader();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.datosIntermediarioWriter = new DatosIntermediarioTableWriter();
            this.gruposAseguradosReader = new DatosGruposAseguradosTableReader();
            this.datosTomadorReader = new DatosTomadorTableReader();
            this.datosTomadorWriter = new DatosTomadorTableWriter();
            this.intermediarioReader = new DatosIntermediarioTableReader();
            this.gruposAseguradosMapper = new DatosGruposAseguradosMapper();
            this.parametrizacionReader = new DatosParametrizacionReader();
            this.slipReader = new DatosSlipTableReader();
            this.slipWriter = new DatosSlipTableWriter();
            this.asegurabilidadReader = new DatosAsegurabilidadTableReader();
            this.asegurabilidadWriter = new DatosAsegurabilidadTableWriter();
            this.cotizacionStateWriter = new CotizacionStateWriter();
            this.aseguradoReader = new DatosAseguradoTableReader();
            this.authWriter = new AuthorizationsDataTableWriter();
            this.informacionNegocioWriter = new DatosInformacionNegocioTableWriter();
            this.trasactionsReader = new TransactionsDataTableReader();
        }
        public ConfiguracionSlipDataProvider()
        {
            this.cotizacionWriter = new DatosCotizacionTableWriter();
            this.cotizacionReader = new DatosCotizacionTableReader();
            this.amparoGrupoAseguradoReader = new DatosAmparoGrupoAseguradoTableReader();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.datosIntermediarioWriter = new DatosIntermediarioTableWriter();
            this.gruposAseguradosReader = new DatosGruposAseguradosTableReader();
            this.datosTomadorReader = new DatosTomadorTableReader();
            this.datosTomadorWriter = new DatosTomadorTableWriter();
            this.intermediarioReader = new DatosIntermediarioTableReader();
            this.gruposAseguradosMapper = new DatosGruposAseguradosMapper();
            this.parametrizacionReader = new DatosParametrizacionReader();
            this.slipReader = new DatosSlipTableReader();
            this.slipWriter = new DatosSlipTableWriter();
            this.asegurabilidadReader = new DatosAsegurabilidadTableReader();
            this.asegurabilidadWriter = new DatosAsegurabilidadTableWriter();
            this.cotizacionStateWriter = new CotizacionStateWriter();
            this.aseguradoReader = new DatosAseguradoTableReader();
            this.authWriter = new AuthorizationsDataTableWriter();
            this.informacionNegocioWriter = new DatosInformacionNegocioTableWriter();
            this.trasactionsReader = new TransactionsDataTableReader();
        }

        public async Task<GenerarSlipConfiguracionResponse> GenerateConfiguracionSlipAsync(int codigoCotizacion, int version)
        {
            try
            {
                // consulta informacion de negocio
                var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);

                var codigoSubramo = informacionNegocio.CodigoSubramo;
                // Actualizar el estado de la cotizacion
                if (informacionNegocio.CotizacionState < CotizacionState.OnSlipConfiguration)
                {
                    await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.OnSlipConfiguration);
                }
                

                var variablesSlip = await this.ObtenerVariablesSlipAsync(codigoCotizacion, informacionNegocio.CodigoRamo, informacionNegocio.CodigoSector, informacionNegocio.CodigoSubramo);
                var ciudadSrt = "";
                var departamentoSrt = "";
                foreach (var i in variablesSlip)
                {
                    if (i.Nombre == "Departamento poliza" && i.Valor != "") {
                        departamentoSrt = i.Valor;
                    }
                    
                    if (i.Nombre == "Ciudad Poliza" && i.Valor != "")
                    {
                        ciudadSrt = i.Valor;
                    }
                }
                var informacionUsuario = new
                {
                    CodigoDepartamento = 0,
                    CodigoMunicipio = 0
                };
                // TODO definir codigoDepartamento y codigoMunicipio de usuario
                if (ciudadSrt == "" && departamentoSrt == "")
                {
                    informacionUsuario = new
                    {
                        CodigoDepartamento = 11,
                        CodigoMunicipio = 11001
                    };
                }
                else
                {
                    var result = await parametrizacionReader.TraerDepartamentosAsync();
                    var codDepartamento = 0;
                    var codCiudad = 0;
                    foreach (var ind in result)
                    {
                        if (ind.NombreDepartamento == departamentoSrt) {
                            codDepartamento = ind.CodigoDepartamento;
                        }
                    }
                    var resultMuni = await parametrizacionReader.TraerMunicipiosxDepartamentoAsync(codDepartamento);
                    foreach (var ind in resultMuni)
                    {
                        if (ind.NombreMunicipio == ciudadSrt)
                        {
                            codCiudad = ind.CodigoMunicipio;
                        }
                    }
                    informacionUsuario = new
                    {
                        CodigoDepartamento = codDepartamento,
                        CodigoMunicipio = codCiudad
                    };
                }
               

                var ciudadResponse = await parametrizacionReader.TraerMunicipioAsync(informacionUsuario.CodigoDepartamento, informacionUsuario.CodigoMunicipio);
                var departamentoResponse = await parametrizacionReader.TraerDepartamentoAsync(informacionUsuario.CodigoDepartamento);
                var riesgoResponse = await informacionPersonasReader.TraerRiesgoActividadAsync(informacionNegocio.CodigoTipoRiesgo);
                var tomador = await this.GetTomadorAsync(codigoCotizacion);
                this.amparos = await this.informacionPersonasReader.TraerAmparosAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector);
                var gruposAsegurados = await this.ObtenerGruposAseguradosAsync(codigoCotizacion, informacionNegocio.CodigoRamo, codigoSubramo, informacionNegocio);
                var clausulas = await this.ConstruirClausulasSlipAsync(codigoCotizacion, informacionNegocio.CodigoRamo, codigoSubramo, informacionNegocio.CodigoSector);
                var clausulasActivas = clausulas.Where(x => x.Activo == true).ToList();
                var transCotizacion = this.trasactionsReader.GetTransactionsAsync(codigoCotizacion, version).Result;
                var versiones = this.cotizacionReader.GetVersionesCotizacionQueryAsync(codigoCotizacion).Result;
                if (versiones.Any())
                {
                    var versionesPadre = this.cotizacionReader.GetVersionesCotizacionQueryAsync(versiones.FirstOrDefault().CodigoCotizacionPadre, versiones.FirstOrDefault().VersionPadre).Result;
                    if(clausulasActivas.Count() < 0 || !clausulasActivas.Any())
                    {

                        if(transCotizacion.Transactions.Count() < 3 )
                        {
                            if(transCotizacion.Transactions.FirstOrDefault().Description == "VERSION")
                            {
                                var cotPadre = await this.cotizacionReader.GetCotizacionPadreAsync(versiones.FirstOrDefault().CodigoCotizacionPadre, versiones.FirstOrDefault().VersionPadre);
                                variablesSlip = await this.ObtenerVariablesSlipAsync(cotPadre.CodigoCotizacion, informacionNegocio.CodigoRamo, informacionNegocio.CodigoSector, informacionNegocio.CodigoSubramo);
                                if (versiones.Count() < 2)
                                {
                                    clausulas = await this.ConstruirClausulasSlipAsync(versionesPadre.FirstOrDefault().CodigoCotizacion, informacionNegocio.CodigoRamo, codigoSubramo, informacionNegocio.CodigoSector);
                                }
                            }
                        }
                    }

                }
                

                var diasValidez = variablesSlip.Where(x => x.CodigoVariable == 17).FirstOrDefault().Valor;
                var condiciones = variablesSlip.Where(x => x.CodigoVariable == 19).FirstOrDefault().Valor;
                var ciudadPoliza = variablesSlip.Where(x => x.CodigoVariable == 20).FirstOrDefault().Valor;
                var departamentoPoliza = variablesSlip.Where(x => x.CodigoVariable == 21).FirstOrDefault().Valor;
                var actividad = tomador.Actividad;
                var amparosSlip = new List<AmparoSlip>();

                foreach (GrupoAseguradoSlipList grupo in gruposAsegurados)
                {
                    foreach (AmparoSlip amparoGrupo in grupo.Amparos)
                    {
                        amparosSlip.AddRange(grupo.Amparos);
                    }
                }
                var distinctAmparos = amparosSlip.GroupBy(x => x.CodigoAmparo).Select(y => y.First());

                var slipConfiguracion = new SlipConfiguracion
                {
                    CodigoCotizacion = codigoCotizacion,
                    NumeroCotizacion = informacionNegocio.NumeroCotizacion,
                    CodigoCiudad = ciudadResponse.CodigoMunicipio,
                    Actividad = actividad,
                    CodigoDepartamento = departamentoResponse.CodigoDepartamento,
                    Tomador = tomador,
                    Amparos = distinctAmparos.ToList(),
                    Clausulas = clausulas,
                    DiasValidezCotizacion = int.Parse(diasValidez),
                    Condiciones = condiciones

                };

                return new GenerarSlipConfiguracionResponse
                {
                    Data = slipConfiguracion,
                    CodigoCotizacion = codigoCotizacion,
                    CodigoEstadoCotizacion = informacionNegocio.CodigoEstadoCotizacion,
                    NumeroCotizacion = informacionNegocio.NumeroCotizacion
                };
            }
            catch (Exception ex)
            {
                throw new Exception("GenerateConfiguracionSlipAsync :: SlipDataProvider", ex);
            }
        }

        private async Task<TomadorSlip> GetTomadorAsync(int codigoCotizacion)
        {
            // conulta informacion de tomador
            var informacionTomador = await this.datosTomadorReader.GetTomadorAsync(codigoCotizacion);
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
                NombreTomadorSlip = informacionTomador.TomadorSlip,
                Actividad = informacionTomador.Actividad
            };

            // consulta infomacion de intermediario
            /*var intermediarios = await this.intermediarioReader.GetIntermediariosAsync(codigoCotizacion);
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
                    NombreTomadorSlip = intermediario.IntermediarioSlip,
                    Actividad = informacionTomador.Actividad
                };
            }*/

            return tomador;
        }

        private async Task<IEnumerable<GrupoAseguradoSlipList>> ObtenerGruposAseguradosAsync(int codigoCotizacion, int codigoRamo, int codigoSubramo, InformacionNegocio informacionNegocio)
        {
            var result = new List<GrupoAseguradoSlipList>();
            var grupos = await this.gruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);
            foreach (var g in grupos)
            {
                var amparosGrupo = await this.amparoGrupoAseguradoReader.LeerAmparoGrupoAseguradoAsync(g.CodigoGrupoAsegurado);
                var amparosSlip = await this.BuildAmparosSlipAsync(codigoCotizacion, codigoRamo, codigoSubramo, amparosGrupo, informacionNegocio.CodigoSector);

                var ga = new GrupoAseguradoSlipList
                {
                    CodigoGrupoAsegurado = g.CodigoGrupoAsegurado,
                    Nombre = g.NombreGrupoAsegurado,
                    Amparos = amparosSlip.ToList()
                };

                result.Add(ga);
            }

            return result;
        }

        private async Task<IEnumerable<AmparoSlip>> BuildAmparosSlipAsync(int codigoCotizacion, int codigoRamo, int codigoSubramo, IEnumerable<AmparoGrupoAsegurado> amparosGrupo, int codigoSector)
        {
            var result = new List<AmparoSlip>();
            foreach (var amparo in amparosGrupo)
            {
                var infoAmparo = this.amparos.Single(x => x.CodigoAmparo == amparo.CodigoAmparo);
                var variables = await this.ObtenerVariablesAmparoAsync(codigoCotizacion, codigoRamo, amparo, codigoSector, codigoSubramo);
                var textoDesc = this.slipReader.LeerTextoAmparoAsync(codigoCotizacion, amparo.CodigoAmparo, codigoRamo, codigoSubramo, codigoSector);
                result.Add(new AmparoSlip
                {
                    CodigoRamo = codigoRamo,
                    CodigoSubramo = codigoSubramo,
                    CodigoAmparo = amparo.CodigoAmparo,
                    Activo = false,
                    NombreAmparo = infoAmparo.NombreAmparo,
                    DescripcionAmparo = textoDesc.Result.DescripcionAmparo,
                    Variables = variables.ToList()
                });
            }

            return result;
        }

        public async Task<List<Clausula>> ConstruirClausulasSlipAsync(int codigoCotizacion, int codigoRamo, int subramo, int sector)
        {
            var result = new List<Clausula>();
            var valoresSlip = await this.slipReader.LeerValoresSlipAsync(codigoCotizacion, codigoRamo, sector, subramo);
            var groupClausulas = valoresSlip
                                .Where(z => z.CodigoTipoSeccion == 3)
                                .GroupBy(v => v.CodigoSeccion)
                                .Select(grp => new
                                {
                                    CodigoSeccion = grp.Key,
                                    CodigoSubramo = grp.ToList().First().CodigoSubRamo,
                                    grp.ToList().First().NombreSeccion,
                                    grp.ToList().First().CodigoRamo,
                                    ResponseValores = grp.ToList()
                                })
                                .ToList();

            foreach (var element in groupClausulas)
            {
                var clausula = new Clausula();
                var textDesc = await this.slipReader.LeerTextoClausulaAsync(codigoCotizacion, element.CodigoSeccion, element.CodigoRamo, sector,  subramo);
                clausula.CodigoSeccion = element.CodigoSeccion;
                clausula.CodigoRamo = element.CodigoRamo;
                clausula.CodigoSubramo = element.CodigoSubramo;
                clausula.Nombre = element.NombreSeccion;
                clausula.DescripcionClausula = textDesc.DescripcionClausula;

                foreach (var valores in element.ResponseValores)
                {
                    if (valores.CodigoVariable != 23 )
                    {
                        var variable = new SlipVariable
                        {
                            CodigoVariable = valores.CodigoVariable,
                            CodigoSeccion = valores.CodigoSeccion,
                            Nombre = valores.NombreVariable,
                            TipoDato = valores.TipoDato,
                            Valor = valores.ValorVariable.ToString(),
                            ValorMaximo = decimal.Parse(valores.ValorTope, CultureInfo.InvariantCulture)
                        };
                        clausula.Variables.Add(variable);
                    }
                    clausula.Activo = valores.Activo.Equals("1") ? true : false;
                }

                if (clausula.CodigoSeccion == 35)
                {
                    var asegurabilidad = await this.asegurabilidadReader.LeerAsegurabilidadAsync(codigoCotizacion);
                    clausula.Asegurabilidad = asegurabilidad;
                }

                result.Add(clausula);
            }

            return result;
        }

        public async Task<IEnumerable<SlipVariable>> ObtenerVariablesAmparoAsync(int codigoCotizacion, int codigoRamo, AmparoGrupoAsegurado amparo, int sector, int subramo)
        {
            var variablesLista = new List<SlipVariable>();
            var valoresSlip = await this.slipReader.LeerValoresSlipAsync(codigoCotizacion, codigoRamo, sector, subramo);
            foreach (var valores in valoresSlip)
            {
                if (valores.CodigoTipoSeccion == 2)
                {
                    var variable = new SlipVariable();
                    if (amparo.CodigoAmparo == valores.CodigoAmparo)
                    {
                        variable.CodigoVariable = valores.CodigoVariable;
                        variable.CodigoSeccion = valores.CodigoSeccion;
                        variable.Nombre = valores.NombreVariable;
                        variable.TipoDato = valores.TipoDato;
                        variable.Valor = valores.ValorVariable.ToString();
                        variable.ValorMaximo = decimal.Parse(valores.ValorTope, CultureInfo.InvariantCulture);
                        variablesLista.Add(variable);
                    }
                }
            }

            // TODO implemetar consulta de variables por amparo
            return variablesLista;
        }

       
        public async Task<List<SlipVariable>> ObtenerVariablesSlipAsync(int codigoCotizacion, int codigoRamo, int sector, int subramo)
        {
            var variablesLista = new List<SlipVariable>();
            var valoresSlip = await this.slipReader.LeerValoresSlipAsync(codigoCotizacion, codigoRamo, sector, subramo);
            foreach (var valores in valoresSlip)
            {
                if (valores.Activo == "1" || valores.CodigoTipoSeccion == 0 )
                {
                    var variable = new SlipVariable
                    {
                        CodigoVariable = valores.CodigoVariable,
                        CodigoSeccion = valores.CodigoSeccion,
                        Nombre = valores.NombreVariable,
                        TipoDato = valores.TipoDato,
                        Valor = valores.ValorVariable.ToString(),
                        ValorMaximo = decimal.Parse(valores.ValorTope, CultureInfo.InvariantCulture)
                    };
                    variablesLista.Add(variable);
                }
            }       


            // TODO implemetar consulta de variables por amparo
            return variablesLista;
        }

        public async Task<ActionResponseBase> GuardarVariablesSlipAsync(SlipConfiguracion configuracion)
        {
            var codigoCotizacion = configuracion.CodigoCotizacion;
            var variablesLista = new List<SlipVariable>();
            var diasValidez = new SlipVariable();
            var condiciones = new SlipVariable();
            var ciudad = new SlipVariable();
            var departamento = new SlipVariable();
            var actividad = new SlipVariable();
            var changed = false;

            var infoNegocio = await informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var numCot = int.Parse(infoNegocio.NumeroCotizacion);
            var departamentoResponse = await parametrizacionReader.TraerDepartamentoAsync(configuracion.CodigoDepartamento);
            var ciudadResponse = await parametrizacionReader.TraerMunicipioAsync(configuracion.CodigoDepartamento, configuracion.CodigoCiudad);
            var variablesListaOld = await ObtenerVariablesSlipAsync(codigoCotizacion, infoNegocio.CodigoRamo, infoNegocio.CodigoSector, infoNegocio.CodigoSubramo);
            variablesListaOld = variablesListaOld.Where(x => x.CodigoVariable != 23).ToList();

            diasValidez.CodigoVariable = 17;
            diasValidez.Valor = configuracion.DiasValidezCotizacion.ToString();
            variablesLista.Add(diasValidez);

            var textCondiciones = configuracion.Condiciones.ToString();
            if (!String.IsNullOrEmpty(textCondiciones))
            {
                

                condiciones.CodigoVariable = 19;
                condiciones.Valor = configuracion.Condiciones.ToString();
                variablesLista.Add(condiciones);

                CotizacionAuthorization modelAuth = new CotizacionAuthorization
                {
                    CodigoCotizacion = numCot,
                    Version = infoNegocio.Version,
                    CodigoSucursal = infoNegocio.CodigoSucursal,
                    CodigoRamo = infoNegocio.CodigoRamo,
                    CodigoSubramo = infoNegocio.CodigoSubramo,
                    CampoEntrada = "condiciones",
                    ValorEntrada = 1,
                    CodigoTipoAutorizacion = 2,
                    RequiereAutorizacion = true,
                    MensajeValidacion = "Existen condiciones especiales",
                };
    
                await this.authWriter.SaveAuthorizationAsync(modelAuth);
                
            }
            else
            {
                await this.authWriter.DeleteAuthorizationByQueryAsync(numCot, infoNegocio.Version);
                condiciones.CodigoVariable = 19;
                condiciones.Valor = configuracion.Condiciones.ToString();
                await this.slipWriter.GuardarConfiguracionSlip(codigoCotizacion, condiciones);
            }

            ciudad.CodigoVariable = 20;
            ciudad.Valor = ciudadResponse.NombreMunicipio;
            variablesLista.Add(ciudad);

            departamento.CodigoVariable = 21;
            departamento.Valor = departamentoResponse.NombreDepartamento;
            variablesLista.Add(departamento);

            actividad.CodigoVariable = 22;
            actividad.Valor = configuracion.Actividad.ToString();
            variablesLista.Add(actividad);

            var grupo = this.gruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion).Result.First();

            var grupoMap = this.gruposAseguradosMapper.ConsultarGrupoAseguradoAsync(codigoCotizacion, infoNegocio.Version, grupo.CodigoGrupoAsegurado).Result;

            var ampGrupo = grupoMap.AmparosGrupo;

            var amparos = configuracion.Amparos;
            var valorEnfermedades = new decimal();
            if (ampGrupo.Where(x => x.CodigoAmparo == 3).Count()  > 0) { 
                valorEnfermedades = ampGrupo.Where(x => x.CodigoAmparo == 3).FirstOrDefault().OpcionesValores.FirstOrDefault().PorcentajeCobertura;
            }
            else
            {
                valorEnfermedades = 0;
            }
            foreach (var amp in amparos)
            {
                var variablesAmp = amp.Variables;
                foreach (var vamp in variablesAmp)
                {
                    var variableAmparo = new SlipVariable();
                    variableAmparo.CodigoVariable = vamp.CodigoVariable;
                    if (vamp.CodigoVariable == 2)
                    {
                        variableAmparo.Valor = valorEnfermedades.ToString();
                    }
                    else
                    {
                        
                        variableAmparo.Valor = vamp.Valor.ToString();
                    }
                    variablesLista.Add(variableAmparo);
                }
            }


            await this.slipWriter.ClearSeleccionClausulaAsync(codigoCotizacion);

            //await this.asegurabilidadWriter.EliminarAsegurabilidadAsync(codigoCotizacion);
            foreach (var clausula in configuracion.Clausulas)
            {
                var seccion = clausula.CodigoSeccion;
                var variablesClasula = clausula.Variables;
                await this.slipWriter.SeleccionarClausulasSlipAsync(codigoCotizacion, seccion);
                foreach (var vcla in variablesClasula)
                {
                    var variableClausula = new SlipVariable
                    {
                        CodigoVariable = vcla.CodigoVariable,
                        Valor = vcla.Valor.ToString()
                    };

                    variablesLista.Add(variableClausula);
                }

                //foreach (var asegurabilidad in clausula.Asegurabilidad)
                //{
                //    await this.asegurabilidadWriter.CrearAsegurabilidadAsync(asegurabilidad, codigoCotizacion);
                //}
            }

            /// Comparación de Variables nuevas y antiguas para verificar si cambió la config Slip

            foreach (var varList in variablesLista)
            {              

                var responseInsertarVariable = this.slipWriter.GuardarConfiguracionSlip(codigoCotizacion, varList);

                if (variablesListaOld.Count() > 0)
                {
                    foreach (var varlistOld in variablesListaOld)
                    {

                        if (varList.CodigoVariable == varlistOld.CodigoVariable)
                        {
                            varList.Valor = varList.Valor == null ? "" : varList.Valor;
                            changed = varList.Valor != varlistOld.Valor ? true : changed == true ? true :false;
                            infoNegocio.SelfAuthorize = changed == true ? false : infoNegocio.SelfAuthorize;
                        }
                    }
                }
            }
            if (configuracion.SnCambioClausulas)
            {
                infoNegocio.SelfAuthorize = false;
                changed = true;
            }
            else
            {
                if (!changed)
                {
                    changed = false;
                }
            }

            var condicioneVarOld = variablesListaOld.Where(x => x.CodigoVariable == 19).FirstOrDefault().Valor.ToString();
            if (String.IsNullOrEmpty(condicioneVarOld))
            {
                var exclusion = variablesListaOld.Where(x => x.CodigoVariable == 19).ToList();
                variablesListaOld = variablesListaOld.Except(exclusion).ToList();
            }

            changed = variablesLista.Count() != variablesListaOld.Count() ? true : changed == true ? true : false;

            //COnsultar información tomador antes de guardar nuevo objeto
            // validar que la información actual sea diferente al tomador que se va a almacenar
            var tomadorOld = new TomadorSlip();
            var model = await this.datosTomadorReader.GetTomadorAsync(codigoCotizacion);
            tomadorOld.Actividad = model.Actividad;
            tomadorOld.CodigoCiudad = model.CodigoMunicipio;
            tomadorOld.CodigoDepartamento = model.CodigoDepartamento;
            tomadorOld.CodigoTipoDocumento = model.CodigoTipoDocumento;
            tomadorOld.CodigoTomador = model.CodigoTomador;
            tomadorOld.Direccion = model.Direccion;
            tomadorOld.Email = model.Email;
            tomadorOld.EsIntermediario = configuracion.Tomador.EsIntermediario;
            tomadorOld.Nombre = configuracion.Tomador.Nombre;
            tomadorOld.NombreTomadorSlip = configuracion.Tomador.NombreTomadorSlip;
            tomadorOld.NumeroDocumento = model.NumeroDocumento;
            tomadorOld.Telefono = model.Telefono1Contacto;
            /*if (!configuracion.Tomador.EsIntermediario)
            {
                var model = await this.datosTomadorReader.GetTomadorAsync(codigoCotizacion);
                tomadorOld.Actividad = model.Actividad;
                tomadorOld.CodigoCiudad = model.CodigoMunicipio;
                tomadorOld.CodigoDepartamento = model.CodigoDepartamento;
                tomadorOld.CodigoTipoDocumento = model.CodigoTipoDocumento;
                tomadorOld.CodigoTomador = model.CodigoTomador;
                tomadorOld.Direccion = model.Direccion;
                tomadorOld.Email = model.Email;
                tomadorOld.EsIntermediario = configuracion.Tomador.EsIntermediario;
                tomadorOld.Nombre = configuracion.Tomador.Nombre;
                tomadorOld.NombreTomadorSlip = configuracion.Tomador.NombreTomadorSlip;
                tomadorOld.NumeroDocumento = model.NumeroDocumento;
                tomadorOld.Telefono = model.Telefono1Contacto;

            }
            else
            {
                var intermediarios = await this.intermediarioReader.GetIntermediariosAsync(codigoCotizacion);
                var model = intermediarios.OrderByDescending(x => x.Participacion).FirstOrDefault();
                tomadorOld.Actividad = configuracion.Tomador.Actividad;
                tomadorOld.CodigoCiudad = model.CodigoMunicipio;
                tomadorOld.CodigoDepartamento = model.CodigoDepartamento;
                tomadorOld.CodigoTipoDocumento = configuracion.Tomador.CodigoTipoDocumento;
                tomadorOld.CodigoTomador = model.Codigo;
                tomadorOld.Direccion = model.Direccion;
                tomadorOld.Email = model.Email;
                tomadorOld.EsIntermediario = configuracion.Tomador.EsIntermediario;
                tomadorOld.Nombre = model.IntermediarioSlip;
                tomadorOld.NombreTomadorSlip = configuracion.Tomador.NombreTomadorSlip;
                tomadorOld.NumeroDocumento = configuracion.Tomador.NombreTomadorSlip;
                tomadorOld.Telefono = model.Telefono;

            }*/


            await this.SaveTomadorSlipAsync(configuracion.CodigoCotizacion, configuracion.Tomador);

            
            if ((configuracion.Tomador.CodigoCiudad != tomadorOld.CodigoCiudad) || 
                (configuracion.Tomador.CodigoDepartamento != tomadorOld.CodigoDepartamento) || 
                (configuracion.Tomador.Direccion != tomadorOld.Direccion) || 
                (configuracion.Tomador.Email != tomadorOld.Email) ||
                (configuracion.Tomador.Telefono != tomadorOld.Telefono)
                )
            {
                infoNegocio.SelfAuthorize = false;
                changed = true;
            }
            // Validar si ya fueron autorizados controles
            var transAuth = this.trasactionsReader.GetAuthorizationTransactionsAsync(infoNegocio.CodigoCotizacion, infoNegocio.Version).Result;
            var ultimasTrans = transAuth.Reverse().Take(2).ToList();
            var autorizado = ultimasTrans.Where(x => x.CodigoEstadoCotizacion == 1111).OrderByDescending(x => x.CreationDate);
            var solicitud = ultimasTrans.Where(x => x.CodigoEstadoCotizacion == 1110).OrderByDescending(x => x.CreationDate);
            changed = transAuth.Count() > 0 ? (autorizado.Count() == solicitud.Count()) ? changed == true ? true : changed == true ? true : false : infoNegocio.SelfAuthorize != true ? true : changed == true ? true: false : changed == true ? true: false; 

            await this.cotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, changed);

            // update selfAuthorizeFlag infoNegocio if cotizacion changed
            if (changed) await this.informacionNegocioWriter.UpdateSelfAuthorizeFlagASync(codigoCotizacion, false);

            await this.BuildCartaAceptacionAsync(configuracion);

            // TODO implemetar consulta de variables por amparo
            return new ActionResponseBase
            {
                Status = ResponseStatus.Valid
            };
        }

        private async Task BuildCartaAceptacionAsync(SlipConfiguracion slipConfiguracion)
        {
            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(slipConfiguracion.CodigoCotizacion);
            if (slipConfiguracion.Tomador.EsIntermediario)
            {
                var unitTomador = await this.datosTomadorReader.GetTomadorAsync(slipConfiguracion.CodigoCotizacion);
                TomadorSlip ts = new TomadorSlip();
                ts.Nombre = unitTomador.Nombres;
                if (unitTomador.CodigoTipoDocumento == 3)
                {
                    ts.Nombre = unitTomador.PrimerApellido;
                }
                
                ts.CodigoDepartamento = unitTomador.CodigoDepartamento;
                ts.CodigoCiudad = unitTomador.CodigoMunicipio;
                slipConfiguracion.Tomador = ts;
            }
            this.pdfService = new PdfCartaAceptacionService(informacionNegocio, slipConfiguracion.Tomador);

            var groups = await this.gruposAseguradosReader.GetGruposAseguradosAsync(slipConfiguracion.CodigoCotizacion);
            foreach (var group in groups)
            {
                var asegurados = await this.LeerAseguradosVetadosAsync(group.CodigoGrupoAsegurado);
                await this.pdfService.CreatePdfAsync(asegurados.ToList());
            }
        }

        private async Task<IEnumerable<Asegurado>> LeerAseguradosVetadosAsync(int codigoGrupoAsegurado)
        {
            var asegurados = await this.aseguradoReader.LeerAseguradosAsync(codigoGrupoAsegurado);
            return asegurados.Where(x => x.VetadoSarlaft);
        }

        public async Task<GuardarAsegurabilidadResponse> GuardarAsegurabilidadAsync(Asegurabilidad asegurabilidad, int codigoCotizacion)
        {
            try
            {
                var asegurabilidadId = await this.asegurabilidadWriter.CrearAsegurabilidadAsync(asegurabilidad, codigoCotizacion);
                return new GuardarAsegurabilidadResponse
                {
                    AsegurabilidadId = asegurabilidadId,
                    CodigoCotizacion = codigoCotizacion,
                    Status = ResponseStatus.Valid
                };
            }
            catch (Exception ex)
            {
                throw new Exception("ConfiguracionSlipDataProvider :: GuardarAsegurabilidadAsync", ex);
            }
        }

        public async Task<ActionResponseBase> EliminarAsegurabilidadAsync(int codigoCotizacion, int codigoAsegurabilidad)
        {
            try
            {
                await this.asegurabilidadWriter.EliminarAsegurabilidadIdAsync(codigoCotizacion, codigoAsegurabilidad);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("ConfiguracionSlipDataProvider :: GuardarAsegurabilidadAsync", ex);
            }
        }

        private async Task SaveTomadorSlipAsync(int codigoCotizacion, TomadorSlip tomador)
        {
            try
            {
                if (!tomador.EsIntermediario)
                {
                    var completeTomador = await this.datosTomadorReader.GetTomadorAsync(codigoCotizacion);
                    var model = new Tomador
                    {
                        CodigoTomador = tomador.CodigoTomador,
                        CodigoDepartamento = tomador.CodigoDepartamento,
                        CodigoMunicipio = tomador.CodigoCiudad,
                        Direccion = tomador.Direccion,
                        Email = tomador.Email,
                        Telefono1Contacto = tomador.Telefono,
                        TomadorSlip = tomador.Nombre,
                        CodigoTipoDocumento = completeTomador.CodigoTipoDocumento
                    };

                    await this.datosTomadorWriter.ActualizarTomadorAsync(codigoCotizacion, model);
                }
                else
                {
                    var intermediarios = await this.intermediarioReader.GetIntermediariosAsync(codigoCotizacion);
                    var completeIntermediario = intermediarios.OrderByDescending(x => x.Participacion).FirstOrDefault();
                    var model = new ModernizacionPersonas.Entities.Intermediario
                    {
                        Codigo = tomador.CodigoTomador,
                        CodigoDepartamento = tomador.CodigoDepartamento,
                        CodigoMunicipio = tomador.CodigoCiudad,
                        Direccion = tomador.Direccion,
                        Email = tomador.Email,
                        Telefono = tomador.Telefono,
                        CodigoTipoDocumento = completeIntermediario.CodigoTipoDocumento,
                        IntermediarioSlip = tomador.Nombre
                    };

                    await this.datosIntermediarioWriter.UpdateIntermediarioAsync(codigoCotizacion, model);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ConfiguracionSlipDataProvider :: SaveTomadorSlipAsync", ex);
            }
        }
    }
}

public class GuardarAsegurabilidadResponse : ActionResponseBase
{
    public int AsegurabilidadId { get; set; }
}
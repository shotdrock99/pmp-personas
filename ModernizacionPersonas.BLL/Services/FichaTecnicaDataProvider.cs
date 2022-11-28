using Microsoft.AspNetCore.Http;
using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.SISEServices;
using ModernizacionPersonas.DAL.Soligespro;
using ModernizacionPersonas.Entities;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class FichaTecnicaDataProvider
    {
        private const string NOSELECTION = "(SIN SELECCION)";

        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly DatosGruposAseguradosProvider gruposAseguradosProvider;
        private readonly IDatosTomadorReader tomadorReader;
        private readonly IDatosIntermediarioReader intermediarioReader;
        private readonly IDatosOpcionValorAseguradoReader opcionValorReaderService;
        private readonly DatosParametrizacionReader parametrizacionReader;
        private readonly SISEAseguradosSummaryProcessor siseAseguradosProcessor;
        private readonly IDatosTasaOpcionReader tasaOpcioneReader;
        private readonly CotizacionStateWriter cotizacionStateWriter;
        private readonly CotizacionTransactionsProvider cotizacionTransactionsProvider;
        private readonly CotizacionDataProcessorFactory cotizacionDataProcessorFactory;
        private readonly IDatosInformacionNegocioWriter informacionNegocioWriter;
        private readonly IDatosTasaOpcionWriter tasaOpcionWriter;
        private readonly IDatosAseguradoReader aseguradosReader;
        private readonly IDatosGruposAseguradoReader gruposReader;
        private readonly IDatosRangoGrupoAseguradoReader rangoReader;
        private readonly RangosCotizacionUtilities utilRango;
        private readonly CotizacionDataProcessorBase cotpro;
        private readonly IDatosAmparoGrupoAseguradoReader amparoGrupoAseguradoReaderService;
        private readonly DatosGrupoAseguradosUtilities gruposUtilities;
        private readonly CotizacionDataProcessorBase BaseProcesor;

        private PyGDataProcessor pyGDataProcessor;
        private SiniestralidadDataProcessor siniestralidadDataProcessor;
        private IEnumerable<TipoSumaAsegurada> tiposSumaAsegurada;
        private IEnumerable<Amparo> amparos;

        private decimal factorG;
        private InformacionNegocio informacionNegocio;
        private bool tieneTasaSiniestralidad;
        private decimal valorSalarioMinimo = 0;

        public FichaTecnicaDataProvider()
        {
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.gruposAseguradosProvider = new DatosGruposAseguradosProvider();
            this.tomadorReader = new DatosTomadorTableReader();
            this.intermediarioReader = new DatosIntermediarioTableReader();
            this.opcionValorReaderService = new DatosOpcionValorAseguradoTableReader();
            this.parametrizacionReader = new DatosParametrizacionReader();
            this.siseAseguradosProcessor = new SISEAseguradosSummaryProcessor();
            this.tasaOpcioneReader = new DatosTasaOpcionTableReader();
            this.cotizacionStateWriter = new CotizacionStateWriter();
            this.cotizacionTransactionsProvider = new CotizacionTransactionsProvider();
            this.cotizacionDataProcessorFactory = new CotizacionDataProcessorFactory();
            this.informacionNegocioWriter = new DatosInformacionNegocioTableWriter();
            this.tasaOpcionWriter = new DatosTasaOpcionTableWriter();
            this.aseguradosReader = new DatosAseguradoTableReader();
            this.gruposReader = new DatosGruposAseguradosTableReader();
            this.rangoReader = new DatosRangoGrupoAseguradoTableReader();
            this.utilRango = new RangosCotizacionUtilities(this.informacionNegocioReader, this.informacionPersonasReader);
            this.cotpro = new CotizacionDataProcessorBase(1, 1, factorG);
            this.amparoGrupoAseguradoReaderService = new DatosAmparoGrupoAseguradoTableReader();
            this.gruposUtilities = new DatosGrupoAseguradosUtilities();
            this.BaseProcesor = new CotizacionDataProcessorBase(1, 1, 1);
        }

        private async Task InitilizeProviderAsync(int codigoCotizacion)
        {
            // Consulta informacion de negocio
            this.informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            this.tieneTasaSiniestralidad = informacionNegocio.CodigoTipoTasa1 == 5 || informacionNegocio.CodigoTipoTasa2 == 5;
            this.factorG = informacionNegocio.FactorG;

            // Inicializa servicio de calculos de Siniestralidad
            this.siniestralidadDataProcessor = new SiniestralidadDataProcessor(codigoCotizacion, informacionNegocio.IBNR, this.factorG);
            this.tiposSumaAsegurada = await this.informacionPersonasReader.TraerTiposSumaAsegurada(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo);
            this.amparos = await this.informacionPersonasReader.TraerAmparosAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector);
        }

        private void FetchAndSetSalarioMinimoValue()
        {
            // Obtiene el valor del salario minimo
            this.valorSalarioMinimo = 0;
            var esTipoSumaAseguradaSalario = tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == 10).Count() > 0;
            if (esTipoSumaAseguradaSalario)
            {
                this.valorSalarioMinimo = tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == 10).FirstOrDefault().ValorSalarioMinimo;
            }
        }

        public async Task<decimal> getSalarioMinimoAsync()
        {
            this.tiposSumaAsegurada = await this.informacionPersonasReader.TraerTiposSumaAsegurada(15, 1);
            var salario = tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == 10).FirstOrDefault().ValorSalarioMinimo;
            return salario;
        }

        private async Task<PygAnualFichaTecnica> BuildPyGDataAsync(InformacionNegocio informacionNegocio, InformacionSiniestralidad informacionSiniestralidad, IEnumerable<GrupoAseguradoFichaTecnica> gruposAsegurados)
        {
            var informacionFactorG = this.BuildInformacionFactorG(informacionNegocio);
            // Inicializa servicio de calculos de P&G
            this.pyGDataProcessor = new PyGDataProcessor(informacionNegocio, informacionSiniestralidad, gruposAsegurados);
            var result = await this.pyGDataProcessor.CalcularPyGAnualAsync(informacionFactorG);

            return result;
        }

        private async Task<PerfilValores> ObtenerPerfilValoresAsync(int codigoCotizacion, int codigoPerfil, bool listadoAsegurados)
        {
            if (!listadoAsegurados)
            {
                return new PerfilValores();
            }
            else
            {

                var result = new PerfilValores();
                var args = new SISEObtenerRangoPerfilArgs
                {
                    CodigoCotizacion = codigoCotizacion,
                    CodigoPerfil = codigoPerfil
                };


                List<Asegurado> aseguradosList = new List<Asegurado>();

                var grupos = await this.gruposReader.GetGruposAseguradosAsync(codigoCotizacion);


                foreach (GrupoAsegurado ga in grupos)
                {
                    var amparosGrupo = await this.amparoGrupoAseguradoReaderService.LeerAmparoGrupoAseguradoAsync(ga.CodigoGrupoAsegurado);
                    var esBasico = ObtenerAmparoBasicoNoAdicionalAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector, amparosGrupo).Result;
                    var opciones = this.opcionValorReaderService.LeerOpcionValorAseguradoAsync(esBasico.CodigoAmparoGrupoAsegurado).Result;
                    if (ga.ConDistribucionAsegurados)
                    {
                        ga.AseguradosOpcion1 = opciones.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                        ga.AseguradosOpcion2 = opciones.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                        ga.AseguradosOpcion3 = opciones.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                    }

                    esBasico.OpcionesValores.AddRange(opciones);
                    var aseguradosGrupo = await this.aseguradosReader.LeerAseguradosAsync(ga.CodigoGrupoAsegurado);

                    foreach (var asegurados in aseguradosGrupo)
                    {
                        if (ga.CodigoTipoSuma == 5 || ga.CodigoTipoSuma == 2)
                        {
                            asegurados.ValorAsegurado = esBasico.OpcionesValores[0].NumeroSalarios > 0 ? esBasico.OpcionesValores[0].NumeroSalarios * asegurados.ValorAsegurado : asegurados.ValorAsegurado;

                        }
                    }

                    aseguradosList.AddRange(aseguradosGrupo);


                }


                var perfiles = await this.siseAseguradosProcessor.ObtenerRangosxValorAsync(args);

                var rangos = perfiles.Rangos;


                var valorAsegTotal = aseguradosList.Count > 0 ? aseguradosList.Sum(x => x.ValorAsegurado) : grupos.Sum(x => x.ValorAsegurado);
                decimal totalAseg = aseguradosList.Count > 0 ? aseguradosList.Count() : grupos.Sum(x => x.NumeroAsegurados);

                foreach (var r in rangos)
                {

                    var aseg = aseguradosList.Where(x => (decimal)x.ValorAsegurado >= r.ValorAseguradoDesde && (decimal)x.ValorAsegurado <= r.ValorAseguradoHasta);
                    decimal count_aseg = aseg.Count();
                    var valor_aseg = aseg.Sum(x => x.ValorAsegurado);
                    r.CantidadAsegurados = count_aseg;
                    r.PorcentajeParticipacionAsegurados = (count_aseg * 100) / totalAseg;
                    r.ValorAseguradoTotal = valor_aseg;
                    r.PromedioValorAsegurado = count_aseg > 0 ? valor_aseg / count_aseg : 0;
                    r.PorcentajeParticipacionValorAsegurado = (valor_aseg * 100) / valorAsegTotal;
                }

                var totales = new PerfilTotales
                {
                    SumCantidadAsegurados = rangos.Sum(x => x.CantidadAsegurados),
                    SumPorcentajeParticipacionAsegurados = rangos.Sum(x => x.PorcentajeParticipacionAsegurados),
                    SumPorcentajeParticipacionValorAsegurado = rangos.Sum(x => x.PorcentajeParticipacionValorAsegurado),
                    SumValorAseguradoTotal = rangos.Sum(x => x.ValorAseguradoTotal),
                    SumPromedioValorAsegurado = (rangos.Sum(x => x.CantidadAsegurados) > 0) ?
                        rangos.Sum(x => x.ValorAseguradoTotal) / rangos.Sum(x => x.CantidadAsegurados) : 0
                };

                result.Rangos = rangos;
                result.Totales = totales;
                return result;
            }
        }

        private async Task<AmparoGrupoAsegurado> ObtenerAmparoBasicoNoAdicionalAsync(int codigoRamo, int codigoSubramo, int codigoSector, IEnumerable<AmparoGrupoAsegurado> amparosGrupo)
        {
            var amparos = await this.informacionPersonasReader.TraerAmparosAsync(codigoRamo, codigoSubramo, codigoSector);
            var result = (from amparoGrupo in amparosGrupo
                          join amparo in amparos.Where(x => x.SiNoBasico && !x.SiNoAdicional) on amparoGrupo.CodigoAmparo equals amparo.CodigoAmparo
                          select amparoGrupo).FirstOrDefault();

            return result;
        }

        private async Task<PerfilEdades> ObtenerPerfilEdadesAsync(int codigoCotizacion, int codigoPerfil)
        {
            var result = new PerfilEdades();
            var args = new SISEObtenerRangoPerfilArgs
            {
                CodigoCotizacion = codigoCotizacion,
                CodigoPerfil = codigoPerfil
            };

            List<Asegurado> aseguradosList = new List<Asegurado>();
            List<Rango> rangoList = new List<Rango>();

            var grupos = await this.gruposReader.GetGruposAseguradosAsync(codigoCotizacion);



            foreach (GrupoAsegurado ga in grupos)
            {
                var amparosGrupo = await this.amparoGrupoAseguradoReaderService.LeerAmparoGrupoAseguradoAsync(ga.CodigoGrupoAsegurado);
                var esBasico = ObtenerAmparoBasicoNoAdicionalAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector, amparosGrupo).Result;
                var opciones = this.opcionValorReaderService.LeerOpcionValorAseguradoAsync(esBasico.CodigoAmparoGrupoAsegurado).Result;
                if (ga.ConDistribucionAsegurados)
                {
                    ga.AseguradosOpcion1 = opciones.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                    ga.AseguradosOpcion2 = opciones.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                    ga.AseguradosOpcion3 = opciones.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                }

                esBasico.OpcionesValores.AddRange(opciones);
                var aseguradosGrupo = await this.aseguradosReader.LeerAseguradosAsync(ga.CodigoGrupoAsegurado);
                var rangosDb = await this.rangoReader.LeerRangoGrupoAseguradoAsync(ga.CodigoGrupoAsegurado);
                foreach (var asegurados in aseguradosGrupo)
                {
                    if (ga.CodigoTipoSuma == 5 || ga.CodigoTipoSuma == 2)
                    {
                        asegurados.ValorAsegurado = esBasico.OpcionesValores[0].NumeroSalarios > 0 ? esBasico.OpcionesValores[0].NumeroSalarios * asegurados.ValorAsegurado : asegurados.ValorAsegurado;
                    }
                }

                aseguradosList.AddRange(aseguradosGrupo);
                rangoList.AddRange(rangosDb);


            }

            var perfiles = new PerfilEdades();
            if (informacionNegocio.ConListaAsegurados)
            {
                var perfilesSise = await this.siseAseguradosProcessor.ObtenerRangosxEdadAsync(args);

            }


            var rangoListReal = rangoList.GroupBy(r => r.EdadMinAsegurado)
                         .Select(x => new PerfilEdadesRango
                         {

                             AseguradoMayorEdad = x.Select(m => m.EdadMaxAsegurado).FirstOrDefault(),
                             EdadHasta = x.Select(m => m.EdadMaxAsegurado).FirstOrDefault(),
                             EdadDesde = x.Select(m => m.EdadMinAsegurado).FirstOrDefault(),
                             CantidadAsegurados = x.Sum(m => m.NumeroAsegurados),
                             Indice = x.Select(m => m.CodigoRangoGrupoAsegurado).FirstOrDefault(),
                             PorcentajeParticipacionAsegurados = x.Select(m => m.TasaComercial).FirstOrDefault(),
                             PorcentajeParticipacionValorAsegurado = x.Select(m => m.TasaRiesgo).FirstOrDefault(),
                             PromedioValorAsegurado = x.Select(m => m.ValorPrimaBasico).FirstOrDefault(),
                             ValorAseguradoTotal = x.Sum(m => m.ValorAsegurado)
                         }).ToList();

            var rangos = perfiles.Rangos;

            var valorAsegTotal = aseguradosList.Count > 0 ? aseguradosList.Sum(x => x.ValorAsegurado) : rangoList.Count() > 0 ? rangoList.Sum(x => x.ValorAsegurado) : grupos.Sum(x => x.ValorAsegurado);

            var valorListado = aseguradosList.Sum(x => x.ValorAsegurado);
            var valorRango = rangoList.Sum(x => x.ValorAsegurado);
            var valorGrupo = grupos.Sum(x => x.ValorAsegurado);

            decimal totalAseg = aseguradosList.Count > 0 ? aseguradosList.Count() : grupos.Sum(x => x.NumeroAsegurados);
            if (rangoListReal.Count() > 0)
            {
                foreach (var r in rangoListReal)
                {

                    var aseg = aseguradosList.Where(x => this.cotpro.CalcularEdad(x.FechaNacimiento) >= r.EdadDesde && this.cotpro.CalcularEdad(x.FechaNacimiento) <= r.EdadHasta && !x.VetadoSarlaft);
                    decimal count_aseg = aseg.Count() > 0 ? aseg.Count() : r.CantidadAsegurados;
                    var valor_aseg = aseg.Count() > 0 ? aseg.Sum(x => x.ValorAsegurado) : r.ValorAseguradoTotal;
                    r.CantidadAsegurados = count_aseg;
                    r.PorcentajeParticipacionAsegurados = (count_aseg * 100) / totalAseg;
                    r.ValorAseguradoTotal = valor_aseg;
                    r.PromedioValorAsegurado = count_aseg > 0 ? valor_aseg / count_aseg : 0;
                    r.PorcentajeParticipacionValorAsegurado = (valor_aseg * 100) / valorAsegTotal;
                    r.AseguradoMayorEdad = aseg.Count() > 0 ? aseg.Max(x => this.cotpro.CalcularEdad(x.FechaNacimiento)) : r.AseguradoMayorEdad;
                }
            }
            else
            {
                foreach (var r in rangos)
                {

                    var aseg = aseguradosList.Where(x => this.cotpro.CalcularEdad(x.FechaNacimiento) >= r.EdadDesde && this.cotpro.CalcularEdad(x.FechaNacimiento) <= r.EdadHasta && !x.VetadoSarlaft);
                    decimal count_aseg = aseg.Count() > 0 ? aseg.Count() : rangos.Sum(x => x.CantidadAsegurados);
                    var valor_aseg = aseg.Count() > 0 ? aseg.Sum(x => x.ValorAsegurado) : rangos.Sum(x => x.ValorAseguradoTotal);
                    r.CantidadAsegurados = count_aseg;
                    r.PorcentajeParticipacionAsegurados = (count_aseg * 100) / totalAseg;
                    r.ValorAseguradoTotal = valor_aseg;
                    r.PorcentajeParticipacionValorAsegurado = (valor_aseg * 100) / valorAsegTotal;
                    r.AseguradoMayorEdad = aseg.Count() > 0 ? aseg.Max(x => this.cotpro.CalcularEdad(x.FechaNacimiento)) : r.AseguradoMayorEdad;
                }
            }



            var totales = new PerfilTotales
            {
                SumCantidadAsegurados = totalAseg,
                SumPorcentajeParticipacionAsegurados = rangoListReal.Count() > 0 ? rangoListReal.Sum(x => x.PorcentajeParticipacionAsegurados) :
                                                        rangos.Sum(x => x.PorcentajeParticipacionAsegurados),
                SumPorcentajeParticipacionValorAsegurado = rangoListReal.Count() > 0 ? rangoListReal.Sum(x => x.PorcentajeParticipacionValorAsegurado) :
                                                            rangos.Sum(x => x.PorcentajeParticipacionValorAsegurado),
                SumPromedioValorAsegurado = totalAseg == 0 ? 0 : valorAsegTotal / totalAseg,
                SumValorAseguradoTotal = valorAsegTotal
            };

            foreach (var rang in rangos)
            {

                rang.PorcentajeParticipacionAsegurados = rang.CantidadAsegurados > 0 ? (rang.CantidadAsegurados * 100) / totales.SumCantidadAsegurados : 0;
                rang.PorcentajeParticipacionValorAsegurado = rang.ValorAseguradoTotal > 0 ? (rang.ValorAseguradoTotal * 100) / totales.SumValorAseguradoTotal : 0;
            }

            foreach (var rang in rangoListReal)
            {

                rang.PorcentajeParticipacionAsegurados = rang.CantidadAsegurados > 0 ? (rang.CantidadAsegurados * 100) / totales.SumCantidadAsegurados : 0;
                rang.PorcentajeParticipacionValorAsegurado = rang.ValorAseguradoTotal > 0 ? (rang.ValorAseguradoTotal * 100) / totales.SumValorAseguradoTotal : 0;
            }

            totales.SumPorcentajeParticipacionAsegurados = rangoListReal.Count() > 0 ? rangoListReal.Sum(x => x.PorcentajeParticipacionAsegurados) :
                                                            rangos.Sum(x => x.PorcentajeParticipacionAsegurados);
            totales.SumPorcentajeParticipacionValorAsegurado = rangoListReal.Count() > 0 ? rangoListReal.Sum(x => x.PorcentajeParticipacionValorAsegurado) :
                                                                rangos.Sum(x => x.PorcentajeParticipacionValorAsegurado);

            result.Rangos = rangoListReal.Count() > 0 ? rangoListReal : rangos;
            result.Totales = totales;

            return result;
        }

        private InformacionFactorG BuildInformacionFactorG(InformacionNegocio informacionNegocio)
        {
            var ivaComisionIntermediario = informacionNegocio.PorcentajeComision * (informacionNegocio.PorcentajeIvaComision / 100);
            var ivaGastosRetorno = informacionNegocio.PorcentajeRetorno * (informacionNegocio.PorcentajeIvaRetorno / 100);
            var result = new InformacionFactorG
            {
                ComisionIntermediario = informacionNegocio.PorcentajeComision,
                IvaComisionIntermediario = ivaComisionIntermediario,
                GastosCompania = informacionNegocio.PorcentajeGastosCompania,
                GastosRetorno = informacionNegocio.PorcentajeRetorno,
                IvaGastosRetorno = ivaGastosRetorno,
                OtroGgastos = informacionNegocio.PorcentajeOtrosGastos,
                Utilidad = informacionNegocio.UtilidadCompania,
                TotalFactorG = informacionNegocio.FactorG
            };

            return result;
        }

        private async Task<InformacionTomadorFichaTecnica> ObtenerInformacionTomadorAsync(int codigoCotizacion, InformacionNegocio informacionNegocio)
        {
            var result = new InformacionTomadorFichaTecnica();

            var intermediarios = await intermediarioReader.GetIntermediariosAsync(codigoCotizacion);
            var tomador = await tomadorReader.GetTomadorAsync(codigoCotizacion);
            var ciudadResponse = await parametrizacionReader.TraerMunicipioAsync(tomador.CodigoDepartamento, tomador.CodigoMunicipio);
            var riesgoResponse = await informacionPersonasReader.TraerRiesgoActividadAsync(informacionNegocio.CodigoTipoRiesgo);
            var tipoNegocioResponse = await informacionPersonasReader.TraerTipoNegocioxCodigoAsync(informacionNegocio.CodigoTipoNegocio);
            var tipoContratacionResponse = await informacionPersonasReader.TraerTipoContratacionxCodigoAsync(informacionNegocio.CodigoTipoContratacion);
            var nombreTomador = tomador.CodigoTipoDocumento == 3 ? tomador.PrimerApellido
                : $"{tomador.Nombres} {tomador.PrimerApellido} {tomador.SegundoApellido}";

            result.AseguradoraActual = tomador.AseguradoraActual;
            result.ActividadRiesgo = riesgoResponse.NombreRiesgoActividad;
            result.Ciudad = ciudadResponse.NombreMunicipio ?? "N/A";
            result.Tomador = nombreTomador;
            result.Vigencia = new Vigencia
            {

                Desde = ((DateTime)informacionNegocio.FechaInicio).ToString("dd-MM-yyyy"),
                Hasta = ((DateTime)informacionNegocio.FechaFin).ToString("dd-MM-yyyy")
            };

            result.Intermediarios = this.MapIntermediariosFichaTecnica(intermediarios);
            result.TipoNegocio = tipoNegocioResponse.NombreTipoNegocio;
            result.TipoContratacion = tipoContratacionResponse.NombreTipoContratacion;
            result.ConListadoAsegurados = informacionNegocio.ConListaAsegurados;

            return result;

        }

        private IEnumerable<IntermediarioFichaTecnica> MapIntermediariosFichaTecnica(IEnumerable<ModernizacionPersonas.Entities.Intermediario> intermediarios)
        {
            var result = new List<IntermediarioFichaTecnica>();
            foreach (var x in intermediarios)
            {
                result.Add(new IntermediarioFichaTecnica
                {
                    Nombre = $"{x.PrimerNombre} {x.SegundoNombre} {x.PrimerApellido} {x.SegundoApellido}",
                    PorcentajeParticipacion = x.Participacion
                });
            }

            return result;
        }

        private async Task<IEnumerable<GrupoAseguradoFichaTecnica>> BuildGruposAseguradosAsync(int codigoCotizacion, InformacionNegocio informacionNegocio)
        {
            var factorg = informacionNegocio.FactorG / 100;
            var result = new List<GrupoAseguradoFichaTecnica>();
            var codigoTipoTasa1 = informacionNegocio.CodigoTipoTasa1;
            var codigoTipoTasa2 = informacionNegocio.CodigoTipoTasa2;
            var codigoTipoTasa = codigoTipoTasa1 == 5 ? codigoTipoTasa2 : codigoTipoTasa1;
            var tieneSiniestralidad = codigoTipoTasa1 == 5 || codigoTipoTasa2 == 5;
            var amparos = await this.informacionPersonasReader.TraerAmparosAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector);
            var grupos = await this.gruposAseguradosProvider.ObtenerGruposAseguradosAsync(codigoCotizacion, informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector);
            foreach (var grupo in grupos)
            {
                IEnumerable<Asegurado> asegurados = new List<Asegurado>();
                if (informacionNegocio.ConListaAsegurados)
                {
                    asegurados = await this.aseguradosReader.LeerAseguradosAsync(grupo.CodigoGrupoAsegurado);
                }

                var tipoSumaAsegurada = this.tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == grupo.CodigoTipoSuma).FirstOrDefault();
                var args = new CotizacionDataProcessorArgs
                {
                    CodigoCotizacion = informacionNegocio.CodigoCotizacion,
                    IBNR = informacionNegocio.IBNR,
                    FactorG = informacionNegocio.FactorG,
                    TipoSumaAsegurada = tipoSumaAsegurada,
                    CodigoTipoTasa = codigoTipoTasa,
                    TieneSiniestralidad = tieneSiniestralidad,
                    ValorSalarioMinimo = valorSalarioMinimo,
                    ConListaAsegurados = informacionNegocio.ConListaAsegurados,
                    Asegurados = asegurados
                };

                var dataProcessor = this.cotizacionDataProcessorFactory.Resolve(args);
                // Asigne el numero de opciones segun el conteo dela primer amparo. 
                // Se asigna de esta forma que que el numero de opciones no varia por amparo.
                var numeroOpciones = tipoSumaAsegurada.CodigoTipoSumaAsegurada == 1 ? 3 : 1;

                var resumenData = await dataProcessor.BuildGrupoAseguradoResumen(informacionNegocio, grupo);

                var amparosFichaTecnica = await this.MapAmparosFichaTecnicaAsync(grupo, amparos, resumenData.Opciones);
                var asistencias = amparosFichaTecnica.Where(x => x.CodigoGrupoAmparo == 3);

                if (asistencias.Count() != 0)
                {
                    foreach (var asistencia in asistencias)
                    {
                        foreach (var opcion in asistencia.OpcionesValores)
                        {
                            var opcionReader = this.opcionValorReaderService.LeerOpcionValorAseguradoAsync(opcion.CodigoAmparoGrupoAsegurado).Result;
                            var numAsegOpcion = opcionReader.Where(x => x.IndiceOpcion == opcion.IndiceOpcion).FirstOrDefault().NumeroAsegurados;
                            //Cambiar valor Asistencia con distribución
                            // 20_01_2022 Se modifica la formula para obtener la assitencia de la tabla de valores de FT, debe incluir la asistencia total en caso de que se tenga distribución de asegurados.
                            var valorAssitencia = grupo.ConDistribucionAsegurados ? numAsegOpcion > 0 ? (opcion.ValorAsegurado / grupo.NumeroAsegurados) * numAsegOpcion : 0 : opcion.ValorAsegurado;
                            opcion.ValorAsegurado = valorAssitencia * (decimal)1.19 / (1 - factorg);
                        }

                    }
                }
                var valoresAseguradosTotales = await CalcularValoresAseguradosTotalesOpcionesAsync(dataProcessor, grupo);
                if (informacionNegocio.CodigoTipoTasa1 == 3 || informacionNegocio.CodigoTipoTasa2 == 3)
                {
                    // Obtener y guardar tasas de los rangos
                    // Extend OpcionValor a Amparo
                    // this.cotizacionResumenProcessor.GetAndUpdateTasasRangosAsync(codigoCotizacion).Wait();
                }

                // Calcular primas
                var primas = await dataProcessor.CalcularPrimasGrupoAseguradoFichaTecnicaAsync(informacionNegocio, grupo);
                // Calcular Proyeccion Financiera
                var proyeccionFinanciera = await dataProcessor.CalcularProyeccionFinancieraGrupoAsync(this.informacionNegocio, grupo);

                var ga = new GrupoAseguradoFichaTecnica
                {
                    Codigo = grupo.CodigoGrupoAsegurado,
                    ConListaAsegurados = informacionNegocio.ConListaAsegurados,
                    EdadPromedio = grupo.EdadPromedioAsegurados,
                    Nombre = grupo.NombreGrupoAsegurado,
                    NumeroAsegurados = grupo.NumeroAsegurados,
                    Amparos = amparosFichaTecnica.Where(x => x.CodigoGrupoAmparo != 3),
                    Asistencias = asistencias,
                    Primas = primas,
                    ProyeccionesFinancieras = proyeccionFinanciera,
                    NumeroOpciones = numeroOpciones,
                    ValoresAseguradosTotales = valoresAseguradosTotales,
                    TipoSumaAsegurada = tipoSumaAsegurada,
                    PorcentajeEsperado = grupo.PorcentajeAsegurados,
                    ConDistribucionAsegurados = grupo.ConDistribucionAsegurados,
                    AseguradosOpcion1 = grupo.AseguradosOpcion1,
                    AseguradosOpcion2 = grupo.AseguradosOpcion2,
                    AseguradosOpcion3 = grupo.AseguradosOpcion3,
                    Opciones = resumenData.Opciones


                };

                result.Add(ga);
            }

            return result;
        }

        private async Task<List<TotalValorAseguradoOpcion>> CalcularValoresAseguradosTotalesOpcionesAsync(IResumenDataProcessor2 dataProcessor, GrupoAsegurado g)
        {
            var result = new List<TotalValorAseguradoOpcion>();
            var conListaAsegurados = this.informacionNegocio.ConListaAsegurados;
            var amparoBasicoNoAdicional = g.AmparosGrupo.Where(x => x.AmparoInfo.SiNoBasico && !x.AmparoInfo.SiNoAdicional).FirstOrDefault();
            var opcionValores = await this.opcionValorReaderService.LeerOpcionValorAseguradoAsync(amparoBasicoNoAdicional.CodigoAmparoGrupoAsegurado);
            if (g.ConDistribucionAsegurados)
            {
                g.AseguradosOpcion1 = opcionValores.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                g.AseguradosOpcion2 = opcionValores.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                g.AseguradosOpcion3 = opcionValores.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
            }

            foreach (var o in opcionValores)
            {
                var valorAsegurado = dataProcessor.CalcularValorAseguradoTotal(g, o, conListaAsegurados);
                result.Add(new TotalValorAseguradoOpcion
                {
                    IndiceOpcion = o.IndiceOpcion,
                    ValorAseguradoTotal = valorAsegurado
                });
            }

            return result;
        }

        private async Task<IEnumerable<OpcionValorAseguradoFichaTecnica>> MapOpcionesFichaTecnica(int codigoGrupoAsegurado, AmparoGrupoAsegurado amparoBNA, AmparoGrupoAsegurado amparo, IEnumerable<ValorAseguradoOpcionResumen> opciones)
        {
            var result = new List<OpcionValorAseguradoFichaTecnica>();
            foreach (var opcion in amparo.OpcionesValores)
            {
                var opcionResumen = opciones.Where(x => x.IndiceOpcion == opcion.IndiceOpcion && x.Amparo.CodigoAmparo == opcion.CodigoAmparoGrupoAsegurado).FirstOrDefault();
                var tasaOpcion = await this.tasaOpcioneReader.LeerTasaOpcionAsync(codigoGrupoAsegurado, opcion.IndiceOpcion);
                var valorAsegurado = opcion.ValorAsegurado;
                var valorAseguradoAmparoBNA = amparoBNA.OpcionesValores[opcion.IndiceOpcion - 1].ValorAsegurado;
                if (amparo.AmparoInfo.SiNoPorcentajeBasico)
                {
                    valorAsegurado = (opcion.PorcentajeCobertura / 100) * valorAseguradoAmparoBNA;
                }

                var opcionValor = new OpcionValorAseguradoFichaTecnica
                {
                    CodigoAmparoGrupoAsegurado = opcion.CodigoAmparoGrupoAsegurado,
                    CodigoOpcionValorAsegurado = opcion.CodigoOpcionValorAsegurado,
                    IndiceOpcion = opcion.IndiceOpcion,
                    ValorAsegurado = valorAsegurado,
                    TasaRiesgo = opcion.TasaRiesgo,
                    TasaComercial = tasaOpcion.TasaComercial,
                    PrimaAnual = tasaOpcion.PrimaTotal
                };

                // si el amparo es del grupo de asistencias
                // asigne el valor de la propiedad Prima
                if (amparo.CodigoGrupoAmparo == 3)
                {
                    opcionValor.ValorAsegurado = (decimal)opcion.Prima;
                }

                result.Add(opcionValor);
            }

            return result;
        }

        private async Task<IEnumerable<AmparoGrupoAseguradoFichaTecnica>> MapAmparosFichaTecnicaAsync(GrupoAsegurado grupo, IEnumerable<Amparo> amparos, IEnumerable<ValorAseguradoOpcionResumen> opciones)
        {
            var result = new List<AmparoGrupoAseguradoFichaTecnica>();
            var sortedData = grupo.AmparosGrupo.OrderBy(x => x.CodigoGrupoAmparo).OrderBy(x => x.AmparoInfo.SiNoAdicional);

            var amparoBNA = amparos.Where(x => x.SiNoBasico && !x.SiNoAdicional).FirstOrDefault();
            var amparoBNAGrupo = grupo.AmparosGrupo.Where(x => x.CodigoAmparo == amparoBNA.CodigoAmparo).FirstOrDefault();

            foreach (var a in grupo.AmparosGrupo)
            {
                var amparoInfo = amparos.Where(x => x.CodigoAmparo == a.CodigoAmparo).FirstOrDefault();
                var esAmparoBasicoNoAdicional = a.AmparoInfo.SiNoBasico && !a.AmparoInfo.SiNoAdicional;
                a.CodigoGrupoAmparo = a.AmparoInfo.CodigoGrupoAmparo;

                var opcionesValores = await this.MapOpcionesFichaTecnica(grupo.CodigoGrupoAsegurado, amparoBNAGrupo, a, opciones);
                var amparoFichaTecnica = new AmparoGrupoAseguradoFichaTecnica
                {
                    AmparoInfo = amparoInfo,
                    CodigoAmparo = a.CodigoAmparo,
                    CodigoGrupoAmparo = a.AmparoInfo.CodigoGrupoAmparo,
                    NombreGrupoAmparo = a.AmparoInfo.NombreGrupoAmparo,
                    NombreAmparo = a.AmparoInfo.NombreAmparo,
                    CodigoAmparoGrupoAsegurado = a.CodigoAmparoGrupoAsegurado,
                    CodigoGrupoAsegurado = a.CodigoGrupoAsegurado,
                    OpcionesValores = opcionesValores
                };

                result.Add(amparoFichaTecnica);
            }

            var orderNameResult = result.OrderBy(x => x.CodigoGrupoAmparo).ThenByDescending(p => p.AmparoInfo.SiNoBasico).ThenBy(m => m.AmparoInfo.SiNoAdicional).ThenBy(n => n.NombreAmparo).ToList();

            return orderNameResult;
        }

        public async Task<GenerarFichaTecnicaResponse> GenerateAsync(int codigoCotizacion, string userName)
        {
            try
            {
                await this.InitilizeProviderAsync(codigoCotizacion);
                if (string.IsNullOrEmpty(this.informacionNegocio.UsuarioDirectorComercial))
                {
                    this.informacionNegocio.UsuarioDirectorComercial = "SIN SELECCIONAR";
                    this.informacionNegocio.NombreDirectorComercial = "SIN SELECCIONAR";
                    this.informacionNegocio.EmailDirectorComercial = "SIN SELECCIONAR";
                }

                var tiposTasa = await this.informacionPersonasReader.TraerTasasAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector);
                var sucursal = await informacionPersonasReader.TraerSucursalAsync(informacionNegocio.CodigoSucursal);
                var ramo = await informacionPersonasReader.TraerRamoAsync(informacionNegocio.CodigoRamo);
                var subramo = await informacionPersonasReader.TraerSubRamoAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo);
                var tasa = await informacionPersonasReader.TraerTasaAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector, informacionNegocio.CodigoTipoTasa1);
                var sectores = await informacionPersonasReader.TraerSectoresAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo);
                var sector = sectores.Where(x => x.CodigoSector == informacionNegocio.CodigoSector).FirstOrDefault();

                // Obtiene valor de salario minimo
                this.FetchAndSetSalarioMinimoValue();

                // Genera informacion grupos asegurados
                var gruposAsegurados = await this.BuildGruposAseguradosAsync(codigoCotizacion, informacionNegocio);

                var informacionFactorG = this.BuildInformacionFactorG(informacionNegocio);

                // Genera informacion Siniestralidad
                var informacionSiniestralidad = await this.BuildSiniestralidadDataAsync(codigoCotizacion, gruposAsegurados);

                foreach (var grupo in gruposAsegurados)
                {
                    for (int i = 1; i < 3; i++)
                    {
                        var opcion = this.tasaOpcioneReader.LeerTasaOpcionAsync(grupo.Codigo, i).Result;
                        opcion.TasaSiniestralidadTotal = Math.Round(informacionSiniestralidad.ProyeccionFinanciera.TasaComercialTotal, 6);
                        this.tasaOpcionWriter.ActualizarTasaOpcionAsync(opcion);
                    }
                }

                var directorComercialInfo = new DirectorComercialInfo
                {
                    Nombre = this.informacionNegocio.NombreDirectorComercial,
                    Usuario = this.informacionNegocio.UsuarioDirectorComercial,
                    Email = this.informacionNegocio.EmailDirectorComercial
                };

                // Genera informacion de P&G
                var pygAnual = await this.BuildPyGDataAsync(informacionNegocio, informacionSiniestralidad, gruposAsegurados);

                var result = new FichaTecnica
                {
                    EstadoCotizacion = (CotizacionState)informacionNegocio.CodigoEstadoCotizacion,
                    Zona = sucursal.NombreZona,
                    Sucursal = sucursal.NombreSucursal,
                    TipoTasa = tasa,
                    Ramo = ramo.NombreRamo,
                    Subramo = subramo.NombreSubRamo,
                    DirectorComercialInfo = directorComercialInfo,
                    Sector = sector == null ? NOSELECTION : sector.NombreSector,
                    TieneSiniestralidad = this.tieneTasaSiniestralidad,
                    InformacionTomador = await this.ObtenerInformacionTomadorAsync(codigoCotizacion, informacionNegocio),
                    GruposAsegurados = gruposAsegurados,
                    InformacionFactorG = informacionFactorG,
                    InformacionSiniestralidad = informacionSiniestralidad,
                    PygAnual = pygAnual,
                    PerfilEdades = await this.ObtenerPerfilEdadesAsync(codigoCotizacion, informacionNegocio.CodigoPerfilEdad),
                    PerfilValores = await this.ObtenerPerfilValoresAsync(codigoCotizacion, informacionNegocio.CodigoPerfilValor, informacionNegocio.ConListaAsegurados),
                };

                // Actualizar el estado de la cotizacion       
                if (informacionNegocio.CotizacionState < CotizacionState.OnFichaTecnica)
                {
                    await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.OnFichaTecnica);
                    // register transaction                         
                    await this.cotizacionTransactionsProvider.CreateTransactionAsync(codigoCotizacion, informacionNegocio.Version, userName, "Generación de Ficha Técnica");
                }

                //Calcular tasa general
                decimal sumaPrimas = 0;
                decimal sumavalores = 0;

                var transactions = this.cotizacionTransactionsProvider.GetTransactionsAsync(codigoCotizacion, this.informacionNegocio.Version).Result.Transactions;
                var fichaAlterna = transactions.Where(x => x.Description == "Ficha Tecnica Alterna").Count() > 0 ? true : false;

                foreach (var gr in gruposAsegurados)
                {
                    var primaProcesor = new ValoresPrimasDataProcessor(informacionNegocio.CodigoTipoTasa1, gr.TipoSumaAsegurada.CodigoTipoSumaAsegurada);
                    foreach (var op in gr.Opciones)
                    {
                        op.PrimaAnualIndividual = primaProcesor.CalcularPrimaIndividualAnual(op.ValorAsegurado, op.TasaComercialAnual);
                    }
                    if (gr.ConDistribucionAsegurados)
                    {
                        sumaPrimas += gr.Opciones.Sum(x => x.PrimaAnualTotal);
                        sumavalores += gr.Opciones.Sum(x => x.ValorAseguradoTotal);
                    }
                    else
                    {
                        sumaPrimas += gr.Opciones.Where(x => x.IndiceOpcion == 1).Sum(x => x.PrimaAnualIndividual);
                        sumavalores += gr.Opciones.Where(x => x.IndiceOpcion == 1).Sum(x => x.ValorAsegurado);
                    }
                }

                var tasaGeneral = this.gruposUtilities.CalcularTasaGeneral(sumaPrimas, sumavalores);
                var tasaGeneralTotal = Math.Round( tasaGeneral + (tasaGeneral * gruposAsegurados.FirstOrDefault().Opciones.FirstOrDefault().PorcentajeRecargo /100) - (tasaGeneral * gruposAsegurados.FirstOrDefault().Opciones.FirstOrDefault().PorcentajeDescuento /100), 4);
                if (result.TieneSiniestralidad)
                {
                    var tasaSiniestralidad = informacionSiniestralidad.ProyeccionFinanciera.TasaComercialAnual;
                    if (fichaAlterna)
                    {
                        if (tasaGeneral > tasaSiniestralidad)
                        {
                            result.TieneSiniestralidad = true;
                            // Cambiamos proceso de ficha alterna para cuando debe msotrar siniestralidad, se comenta porque antes no iba en el proceso
                            //foreach (var ga in result.GruposAsegurados)
                            //{
                            //    foreach (var amp in ga.Amparos)
                            //    {
                            //        foreach (var op in ga.Opciones)
                            //        {
                            //            foreach (var opAmp in amp.OpcionesValores)
                            //            {
                            //                var opcionInd = ga.Primas.PrimaIndividualAnual.Where(x => x.IndiceOpcion == op.IndiceOpcion).FirstOrDefault();
                            //                var opcionTot = ga.Primas.PrimaTotalAnual.Where(x => x.IndiceOpcion == op.IndiceOpcion).FirstOrDefault();
                            //                var aseg = ga.ConDistribucionAsegurados ? op.IndiceOpcion == 1 ? ga.AseguradosOpcion1 : op.IndiceOpcion == 2 ? ga.AseguradosOpcion2 : ga.AseguradosOpcion3 : ga.NumeroAsegurados;
                            //                if (op.IndiceOpcion == opAmp.IndiceOpcion)
                            //                {
                            //                    decimal asisInd = 0;
                            //                    var asistencia = new OpcionValorAseguradoFichaTecnica();

                            //                    if (ga.Asistencias.Count() > 0)
                            //                    {
                            //                        asistencia = ga.Asistencias.FirstOrDefault().OpcionesValores.Where(x => x.IndiceOpcion == opAmp.IndiceOpcion).FirstOrDefault();
                            //                        asisInd = asistencia.ValorAsegurado / aseg;
                            //                    }

                            //                    var valorPrima = BaseProcesor.CalcularPrimaIndividualAnual(op.ValorAsegurado, tasaSiniestralidad);
                            //                    opcionInd.Valor = valorPrima + asisInd;
                            //                    var vlrNeto = opcionInd.Valor * aseg;
                            //                    opcionTot.Valor = vlrNeto + asistencia.ValorAsegurado;
                            //                }
                            //            }

                            //        }
                            //    }
                            //}
                        }
                        else
                        {
                            //result.TieneSiniestralidad = false;
                            result.InformacionSiniestralidad.ProyeccionFinanciera.TasaComercialAnual = tasaGeneral;
                            result.InformacionSiniestralidad.ProyeccionFinanciera.TasaComercialTotal = tasaGeneralTotal;
                            result.InformacionSiniestralidad.ProyeccionFinanciera.PorcentajeDescuento = gruposAsegurados.FirstOrDefault().Opciones.FirstOrDefault().PorcentajeDescuento;
                            result.InformacionSiniestralidad.ProyeccionFinanciera.PorcentajeRecargo = gruposAsegurados.FirstOrDefault().Opciones.FirstOrDefault().PorcentajeRecargo;


                            foreach (var ga in result.GruposAsegurados)
                            {
                                foreach (var amp in ga.Amparos)
                                {
                                    foreach (var op in ga.Opciones)
                                    {
                                        foreach (var opAmp in amp.OpcionesValores)
                                        {
                                            var opcionInd = ga.Primas.PrimaIndividualAnual.Where(x => x.IndiceOpcion == op.IndiceOpcion).FirstOrDefault();
                                            var opcionTot = ga.Primas.PrimaTotalAnual.Where(x => x.IndiceOpcion == op.IndiceOpcion).FirstOrDefault();
                                            var aseg = ga.ConDistribucionAsegurados ? op.IndiceOpcion == 1 ? ga.AseguradosOpcion1 : op.IndiceOpcion == 2 ? ga.AseguradosOpcion2 : ga.AseguradosOpcion3 : ga.NumeroAsegurados;
                                            if (op.IndiceOpcion == opAmp.IndiceOpcion)
                                            {
                                                decimal asisInd = 0;
                                                var asistencia = new OpcionValorAseguradoFichaTecnica();

                                                if (ga.Asistencias.Count() > 0)
                                                {
                                                    asistencia = ga.Asistencias.FirstOrDefault().OpcionesValores.Where(x => x.IndiceOpcion == opAmp.IndiceOpcion).FirstOrDefault();
                                                    asisInd = asistencia.ValorAsegurado / aseg;
                                                }
                                                opcionInd.Valor = op.PrimaAnualIndividual + asisInd;
                                                opcionTot.Valor = op.PrimaAnualTotal + asistencia.ValorAsegurado;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tasaGeneral > tasaSiniestralidad)
                        {
                            //result.TieneSiniestralidad = false;
                            result.InformacionSiniestralidad.ProyeccionFinanciera.TasaComercialAnual = tasaGeneral;
                            result.InformacionSiniestralidad.ProyeccionFinanciera.TasaComercialTotal = tasaGeneralTotal;
                            result.InformacionSiniestralidad.ProyeccionFinanciera.PorcentajeDescuento = gruposAsegurados.FirstOrDefault().Opciones.FirstOrDefault().PorcentajeDescuento;
                            result.InformacionSiniestralidad.ProyeccionFinanciera.PorcentajeRecargo = gruposAsegurados.FirstOrDefault().Opciones.FirstOrDefault().PorcentajeRecargo;

                            foreach (var ga in result.GruposAsegurados)
                            {
                                var primaProcesor = new ValoresPrimasDataProcessor(informacionNegocio.CodigoTipoTasa1, ga.TipoSumaAsegurada.CodigoTipoSumaAsegurada);

                                foreach (var amp in ga.Amparos)
                                {
                                    foreach (var op in ga.Opciones)
                                    {
                                        op.PrimaAnualIndividual = primaProcesor.CalcularPrimaIndividualAnual(op.ValorAsegurado, op.TasaComercialAplicar);

                                        foreach (var opAmp in amp.OpcionesValores)
                                        {
                                            var opcionInd = ga.Primas.PrimaIndividualAnual.Where(x => x.IndiceOpcion == op.IndiceOpcion).FirstOrDefault();
                                            var opcionTot = ga.Primas.PrimaTotalAnual.Where(x => x.IndiceOpcion == op.IndiceOpcion).FirstOrDefault();
                                            var aseg = ga.ConDistribucionAsegurados ? op.IndiceOpcion == 1 ? ga.AseguradosOpcion1 : op.IndiceOpcion == 2 ? ga.AseguradosOpcion2 : ga.AseguradosOpcion3 : ga.NumeroAsegurados;
                                            if (op.IndiceOpcion == opAmp.IndiceOpcion)
                                            {
                                                decimal asisInd = 0;
                                                var asistencia = new OpcionValorAseguradoFichaTecnica();

                                                if (ga.Asistencias.Count() > 0)
                                                {
                                                    asistencia = ga.Asistencias.FirstOrDefault().OpcionesValores.Where(x => x.IndiceOpcion == opAmp.IndiceOpcion).FirstOrDefault();
                                                    asisInd = asistencia.ValorAsegurado / aseg;
                                                }
                                                opcionInd.Valor = op.PrimaAnualIndividual + asisInd;
                                                opcionTot.Valor = op.PrimaAnualTotal + asistencia.ValorAsegurado;
                                            }
                                        }

                                    }
                                }
                            }
                        }

                    }

                }

                return new GenerarFichaTecnicaResponse
                {
                    Data = result,
                    CodigoCotizacion = codigoCotizacion,
                    CodigoEstadoCotizacion = informacionNegocio.CodigoEstadoCotizacion,
                    NumeroCotizacion = informacionNegocio.NumeroCotizacion
                };

            }
            catch (Exception ex)
            {
                throw new Exception("FichaTecnicaMapper :: GenerateAsync", ex);
            }
        }

        private async Task<InformacionSiniestralidad> BuildSiniestralidadDataAsync(int codigoCotizacion, IEnumerable<GrupoAseguradoFichaTecnica> gruposAsegurados)
        {
            var dataProcessor = this.cotizacionDataProcessorFactory.GetProcessor();
            return await dataProcessor.BuildSiniestralidadDataAsync(codigoCotizacion, informacionNegocio.CodigoRamo, gruposAsegurados, tieneTasaSiniestralidad);
        }
    }

    public class CalcularPyGArgs
    {
        public decimal ComisionIntermediario { get; internal set; }
        public decimal GastosCompania { get; internal set; }
        public decimal PorcentajeRetorno { get; internal set; }
        public decimal PorcentajeOtrosGastos { get; internal set; }
        public List<GrupoAseguradoFichaTecnica> GrupoAsegurados { get; internal set; }
        public decimal TasaComercialSiniestralidad { get; internal set; }
        public decimal TotalSiniestralidadConIBNR { get; internal set; }
        public bool TieneSiniestralidad { get; internal set; }
    }

    public class MapOpcionesFichaArgs
    {
        public int CodigoGrupoAsegurado { get; set; }
        public int NumeroAsegurados { get; set; }
        public List<AmparoGrupoAsegurado> AmparosGrupo { get; set; }
        public List<Amparo> Amparos { get; set; }
        public decimal TasaRiesgoSiniestralidad { get; set; }
        public decimal FactorG { get; set; }
        public decimal ValorAsegurado { get; set; }
        public int CodigoTipoSumaAsegurada { get; internal set; }
    }
}

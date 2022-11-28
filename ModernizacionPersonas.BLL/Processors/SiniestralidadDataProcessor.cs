using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class SiniestralidadDataProcessor
    {
        private readonly IDatosSiniestralidadReader siniestralidadReader;
        private readonly IDatosGruposAseguradoReader gruposReader;
        private readonly int codigoCotizacion;
        private readonly decimal factorG;
        private readonly decimal IBNR;
        private readonly IDatosAmparoGrupoAseguradoReader amparoGrupoAseguradoReaderService;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosOpcionValorAseguradoReader opcionValorReaderService;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly CotizacionTransactionsProvider cotizacionTransactionsProvider;
        private readonly DatosGrupoAseguradosUtilities gruposUtilities;

        public SiniestralidadDataProcessor(int codigoCotizacion, decimal IBNR, decimal factorG)
        {
            this.siniestralidadReader = new DatosSiniestralidadTableReader();
            this.gruposReader = new DatosGruposAseguradosTableReader();
            this.codigoCotizacion = codigoCotizacion;
            this.factorG = factorG / 100;
            this.IBNR = IBNR;
            this.amparoGrupoAseguradoReaderService = new DatosAmparoGrupoAseguradoTableReader();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.opcionValorReaderService = new DatosOpcionValorAseguradoTableReader();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.cotizacionTransactionsProvider = new CotizacionTransactionsProvider();
            this.gruposUtilities = new DatosGrupoAseguradosUtilities();
        }

        private async Task<AmparoGrupoAsegurado> ObtenerAmparoBasicoNoAdicionalAsync(int codigoRamo, int codigoSubramo, int codigoSector, IEnumerable<AmparoGrupoAsegurado> amparosGrupo)
        {
            var amparos = await this.informacionPersonasReader.TraerAmparosAsync(codigoRamo, codigoSubramo, codigoSector);
            var result = (from amparoGrupo in amparosGrupo
                          join amparo in amparos.Where(x => x.SiNoBasico && !x.SiNoAdicional) on amparoGrupo.CodigoAmparo equals amparo.CodigoAmparo
                          select amparoGrupo).FirstOrDefault();

            return result;
        }

        public async Task<InformacionSiniestralidad> ObtenerSiniestralidadAsync(int codigoCotizacion, int codigoRamo, decimal valorTotalGrupoAsegurado)
        {
            var grupos = this.gruposReader.GetGruposAseguradosAsync(codigoCotizacion).Result;
            var informacionNegocio = this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion).Result;
            
            foreach (var gr in grupos)
            {
                var amparosGrupo = await this.amparoGrupoAseguradoReaderService.LeerAmparoGrupoAseguradoAsync(gr.CodigoGrupoAsegurado);
                var esBasico = ObtenerAmparoBasicoNoAdicionalAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector, amparosGrupo).Result;
                var opciones = this.opcionValorReaderService.LeerOpcionValorAseguradoAsync(esBasico.CodigoAmparoGrupoAsegurado).Result;
                esBasico.OpcionesValores.AddRange(opciones);
                gr.ValorAsegurado = gr.CodigoTipoSuma == 10 ?
                                     gr.ValorAsegurado * gr.NumeroAsegurados :
                                     gr.CodigoTipoSuma == 5 ?
                                     opciones.FirstOrDefault().NumeroSalarios > 0 ?
                                     gr.ValorAsegurado * opciones.FirstOrDefault().NumeroSalarios :
                                     gr.ValorAsegurado * gr.NumeroAsegurados :
                                     gr.ValorAsegurado;
                if (gr.ConDistribucionAsegurados)
                {
                    gr.AseguradosOpcion1 = opciones.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                    gr.AseguradosOpcion2 = opciones.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                    gr.AseguradosOpcion3 = opciones.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                }
                
            }
            var valorAseguradoTotal = grupos.Sum(x => x.ValorAsegurado);
            var listaSiniestralidad = new List<SiniestralidadFichaTecnica>();
            var siniestralidadValores = await this.siniestralidadReader.GetSiniestralidadAsync(codigoCotizacion);
            foreach (var siniestros in siniestralidadValores)
            {
                var ibnr = siniestros.Anno == informacionNegocio.anyosSiniestralidad ? this.IBNR : 0;
                var siniestralidadVigencia = new SiniestralidadFichaTecnica()
                {
                    VigenciaDesde = siniestros.FechaInicial,
                    VigenciaHasta = siniestros.FechaFinal,
                    ValorIncurrido = siniestros.ValorIncurrido,
                    NumeroCasos = siniestros.NumeroCasos,
                    SiniestralidadPromedio = CalcularPromedioSiniestralidad(siniestros.ValorIncurrido, siniestros.NumeroCasos),
                    IBNR = ibnr,
                    SumValorIncurridoIBNR = CalcularSumaIBNR(siniestros.ValorIncurrido, ibnr),
                };

                listaSiniestralidad.Add(siniestralidadVigencia);
            }

            var totales = new SiniestralidadTotales();
            if (listaSiniestralidad.Count > 0)
            {
                totales.SumNumeroCasos = (decimal)listaSiniestralidad.Average(x => x.NumeroCasos);
                totales.SumValorPlusIBNR = listaSiniestralidad.Average(x => x.SumValorIncurridoIBNR);
                totales.SumValorIncurrido = listaSiniestralidad.Average(x => x.ValorIncurrido);
            }

            var tasaRiesgo = await this.CalcularTasaSiniestralidadAsync(valorAseguradoTotal);
            var proyeccionFinanciera = this.CalcularProyeccionFinanciera(tasaRiesgo);

            return new InformacionSiniestralidad
            {
                Siniestros = listaSiniestralidad,
                Totales = totales,
                TasaRiesgo = tasaRiesgo,
                ProyeccionFinanciera = proyeccionFinanciera
            };
        }

        public async Task<InformacionSiniestralidad> ObtenerSiniestralidadAsync(int codigoCotizacion, ProyeccionFinanciera proyeccionFinanciera, decimal tasaRiesgo, decimal valorTotalAsegurado, int numeroAsegurados)
        {
            var informacionNegocio = this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion).Result;
            var listaSiniestralidad = new List<SiniestralidadFichaTecnica>();
            var siniestralidadValores = await this.siniestralidadReader.GetSiniestralidadAsync(codigoCotizacion);
            foreach (var siniestros in siniestralidadValores)
            {
                var ibnr = siniestros.Anno == informacionNegocio.anyosSiniestralidad ? this.IBNR : 0;
                var siniestralidadVigencia = new SiniestralidadFichaTecnica()
                {
                    VigenciaDesde = siniestros.FechaInicial,
                    VigenciaHasta = siniestros.FechaFinal,
                    ValorIncurrido = siniestros.ValorIncurrido,
                    NumeroCasos = siniestros.NumeroCasos,
                    SiniestralidadPromedio = CalcularPromedioSiniestralidad(siniestros.ValorIncurrido, siniestros.NumeroCasos),
                    IBNR = ibnr,
                    SumValorIncurridoIBNR = CalcularSumaIBNR(siniestros.ValorIncurrido, ibnr),
                };

                listaSiniestralidad.Add(siniestralidadVigencia);
            }

            var totales = new SiniestralidadTotales();
            if (listaSiniestralidad.Count > 0)
            {
                var sumValorIncurrido = listaSiniestralidad.Average(x => x.ValorIncurrido);
                var sumValorIncurridoIBNR = listaSiniestralidad.Average(x => x.SumValorIncurridoIBNR);
                totales.SumNumeroCasos = (decimal)listaSiniestralidad.Average(x => x.NumeroCasos);
                //totales.SumValorPlusIBNR = listaSiniestralidad.Average(x => x.SumValorIncurridoIBNR);
                totales.SumValorIncurrido = sumValorIncurrido;
                totales.SumValorPlusIBNR = sumValorIncurridoIBNR;
            }

            var proyeccionFinancieraSiniestralidad = new ProyeccionFinancieraSiniestralidad
            {
                PorcentajeDescuento = proyeccionFinanciera.PorcentajeDescuento.FirstOrDefault().Valor,
                PorcentajeRecargo = proyeccionFinanciera.PorcentajeRecargo.FirstOrDefault().Valor,
                TasaComercialAnual = proyeccionFinanciera.TasaComercialAnual.FirstOrDefault().Valor,
                TasaComercialTotal = proyeccionFinanciera.TasaComercialTotal.FirstOrDefault().Valor
            };

            var valorTotalSiniestralidad = siniestralidadValores.Sum(x => x.ValorIncurrido);
            var tasasSiniestralidad = this.BuildTasasSiniestralidad(valorTotalAsegurado, numeroAsegurados, valorTotalSiniestralidad);

            return new InformacionSiniestralidad
            {
                Siniestros = listaSiniestralidad,
                Totales = totales,
                TasaRiesgo = tasaRiesgo,
                ProyeccionFinanciera = proyeccionFinancieraSiniestralidad,
                TasasSiniestralidad = tasasSiniestralidad
            };
        }

        private TasasSiniestralidad BuildTasasSiniestralidad(decimal valorTotalAsegurado, int numeroAsegurados, decimal valorTotalSiniestralidad)
        {
            var tasaPuraRiesgo = this.CalcularTasaPuraRiesgoAsync(valorTotalAsegurado, valorTotalSiniestralidad);
            var tasaComercialSiniestralidad = this.CalcularTasaComercial(tasaPuraRiesgo, this.factorG);
            var primaAnualComercial = this.CalcularPrimaAnualComercial(valorTotalAsegurado, tasaComercialSiniestralidad);
            var primaIndividualComercial = this.CalcularPrimaIndividualComercial(primaAnualComercial, numeroAsegurados);

            return new TasasSiniestralidad
            {
                TasaPuraRiesgo = Math.Round(tasaPuraRiesgo, 6),
                TasaComercial = Math.Round(tasaComercialSiniestralidad, 6),
                PrimaAnualComercial = Math.Round(primaAnualComercial, 0),
                PrimaIndividualComercial = Math.Round(primaIndividualComercial, 0)
            };
        }

        private decimal CalcularTasaAnualSiniestralidad(decimal tasaRiesgo)
        {
            return tasaRiesgo / (1 - this.factorG);
        }

        private ProyeccionFinancieraSiniestralidad CalcularProyeccionFinanciera(decimal tasaRiesgo)
        {
            // TODO ASK de donde calculo descuento y recargo siniestraldiad
            decimal porcentajeDescuento = 0;
            decimal porcentajeRecargo = 0;

            var tasaComercial = this.CalcularTasaAnualSiniestralidad(tasaRiesgo);
            var tasaComercialTotal = this.CalcularTasaComercialTotal(tasaComercial, porcentajeDescuento, porcentajeRecargo);

            var proyeccionFinanciera = new ProyeccionFinancieraSiniestralidad
            {
                TasaRiesgo = tasaRiesgo,
                TasaComercialAnual = tasaComercial,
                PorcentajeDescuento = porcentajeDescuento,
                PorcentajeRecargo = porcentajeRecargo,
                TasaComercialTotal = tasaComercialTotal
            };

            return proyeccionFinanciera;
        }

        private decimal CalcularTasaComercialTotal(decimal tasaComercial, decimal porcentajeDescuento, decimal porcentajeRecargo)
        {
            return tasaComercial - (tasaComercial * porcentajeDescuento) + (tasaComercial * porcentajeRecargo);
        }

        public decimal CalcularPrimaAnualComercial(decimal valorTotalAsegurado, decimal tasaComercialSiniestralidad)
        {
            return valorTotalAsegurado * tasaComercialSiniestralidad / 1000;
        }

        public decimal CalcularTasaComercial(decimal tasaPuraRiesgo, decimal factorG)
        {
            return tasaPuraRiesgo / (1 - factorG);
        }

        public decimal CalcularPrimaIndividualComercial(decimal primaAnualComercial, int avgNumeroAsegurados)
        {
            return primaAnualComercial / avgNumeroAsegurados;
        }

        public decimal CalcularTasaPuraRiesgoAsync(decimal valorTotalAsegurado, decimal valorTotalSiniestralidad)
        {
            return valorTotalSiniestralidad / valorTotalAsegurado * 1000;
        }

        private decimal CalcularPromedioSiniestralidad(decimal valorIncurrido, int casos)
        {
            if (casos == 0)
            {
                return 0;
            }

            var result = valorIncurrido / casos;
            return result;
        }

        private decimal CalcularSumaIBNR(decimal valorIncurrido, decimal ibnr)
        {
            var result = valorIncurrido + (valorIncurrido * ibnr / 100);
            return result;
        }

        public decimal CalcularSiniestralidad(bool tieneSiniestralidad, decimal primasEmitidas, decimal valorAsistencia, IEnumerable<GrupoAseguradoFichaTecnica> gruposAsegurados, InformacionSiniestralidad siniestralidad, decimal siniestralidadIBNR)
        {

            
            // Validar tasa seleccionada
            var informacionNegocio = this.informacionNegocioReader.LeerInformacionNegocioAsync(this.codigoCotizacion).Result;
            //Calcular tasa general
            decimal sumaPrimas = 0;
            decimal sumavalores = 0;

            var transactions = this.cotizacionTransactionsProvider.GetTransactionsAsync(codigoCotizacion, informacionNegocio.Version).Result.Transactions;
            var fichaAlterna = transactions.Where(x => x.Description == "Ficha Tecnica Alterna").Count() > 0 ? true : false;
            //Se modifica para sumar las primas de seguro , sin al assitencia por indicación de Elizabeth 26-03-2022

            // Cuando no hay Siniestralidad, se toma la Prima total de riesgo como Siniestralidad = prima total*(1-fg)
            foreach (var gr in gruposAsegurados)
            {
                var primaProcesor = new ValoresPrimasDataProcessor(informacionNegocio.CodigoTipoTasa1, gr.TipoSumaAsegurada.CodigoTipoSumaAsegurada);
                foreach (var op in gr.Opciones)
                {
                    op.PrimaAnualIndividual = primaProcesor.CalcularPrimaIndividualAnual(op.ValorAsegurado, op.TasaComercialAnual);
                }
                if (gr.ConDistribucionAsegurados)
                {
                    sumaPrimas += gr.Opciones.Sum(x => x.PrimaAnualIndividual);
                    sumavalores += gr.Opciones.Sum(x => x.ValorAsegurado);
                }
                else
                {
                    sumaPrimas += gr.Opciones.Where(x => x.IndiceOpcion == 1).Sum(x => x.PrimaAnualIndividual);
                    sumavalores += gr.Opciones.Where(x => x.IndiceOpcion == 1).Sum(x => x.ValorAsegurado);
                }
            }
            var tasaGeneral = this.gruposUtilities.CalcularTasaGeneral(sumaPrimas, sumavalores);
            var tasaGeneralTotal = Math.Round(tasaGeneral + (tasaGeneral * gruposAsegurados.FirstOrDefault().Opciones.FirstOrDefault().PorcentajeRecargo / 100) - (tasaGeneral * gruposAsegurados.FirstOrDefault().Opciones.FirstOrDefault().PorcentajeDescuento / 100), 4);

            decimal result = 0;

            if (tieneSiniestralidad)
            {
                var tasaSiniestralidad = siniestralidad.ProyeccionFinanciera.TasaComercialAnual;

                if (fichaAlterna)
                {
                    if(tasaGeneral > tasaSiniestralidad)
                    {
                        result = siniestralidadIBNR;
                    }
                    else
                    {
                        result = (primasEmitidas) * (1 - this.factorG);
                    }
                }
                else
                {
                    if (tasaGeneral > tasaSiniestralidad)
                    {
                        result = (primasEmitidas) * (1 - this.factorG);
                    }
                    else
                    {
                        result = siniestralidadIBNR;
                    }
                }

            

                return result;
            }

            return  (primasEmitidas) * (1 - this.factorG);
            

            
        }

        public async Task<decimal> CalcularTasaSiniestralidadAsync(decimal valorAsegurado)
        {
            try
            {
                decimal result = 0;
                decimal sum = 0;
                if (this.IBNR == 0 || valorAsegurado == 0) return 0;
                var siniestralidadValores = await this.siniestralidadReader.GetSiniestralidadAsync(this.codigoCotizacion);
                var registros = siniestralidadValores.ToList();
                if (registros.Count > 0)
                {
                    var count = registros.Count;
                    for (int i = 0; i < count; i++)
                    {
                        decimal valorIncurrido = registros[i].ValorIncurrido;
                        var anno = registros[i].Anno;
                        if(anno == count)
                        {
                            sum += valorIncurrido + (valorIncurrido * (this.IBNR / 100));
                        }
                        else
                        {
                            sum += valorIncurrido;
                        }                        
                    }

                    decimal promedio = sum / count;
                    return promedio / valorAsegurado * 1000;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ResumenCotizacionDataMapper :: CalcularTasaSiniestralidad", ex);
            }
        }
    }
}


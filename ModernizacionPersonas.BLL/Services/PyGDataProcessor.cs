using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class PyGDataProcessor
    {
        private readonly int codigoCotizacion;
        private readonly InformacionNegocio informacionNegocio;
        private readonly InformacionSiniestralidad informacionSiniestralidad;
        private readonly IEnumerable<GrupoAseguradoFichaTecnica> gruposAsegurados;
        private readonly CotizacionTransactionsProvider cotizacionTransactionsProvider;
        private readonly DatosGrupoAseguradosUtilities gruposUtilities;

        public PyGDataProcessor(InformacionNegocio informacionNegocio,
            InformacionSiniestralidad informacionSiniestralidad, IEnumerable<GrupoAseguradoFichaTecnica> gruposAsegurados)
        {
            this.codigoCotizacion = informacionNegocio.CodigoCotizacion;
            this.informacionNegocio = informacionNegocio;
            this.informacionSiniestralidad = informacionSiniestralidad;
            this.gruposAsegurados = gruposAsegurados;
            this.cotizacionTransactionsProvider = new CotizacionTransactionsProvider();
            this.gruposUtilities = new DatosGrupoAseguradosUtilities();
        }

        public decimal CalcularPrimaAnualPyG()
        {

            var tieneSiniestralidad = this.informacionNegocio.CodigoTipoTasa1 == 5 || this.informacionNegocio.CodigoTipoTasa2 == 5;
            var tasaComercialSiniestralidad = this.informacionSiniestralidad.ProyeccionFinanciera.TasaComercialTotal;

            //foreach (var gru in this.gruposAsegurados)
            //{
            //  foreach(var valores in gru.ValoresAseguradosTotales)
            //    {

            //        var prueba = (valores.ValorAseguradoTotal * (decimal)1.19) / 1 - this.informacionNegocio.FactorG;
            //    }

            //}
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
                    sumaPrimas += gr.Opciones.Sum(x => x.PrimaAnualIndividual);
                    sumavalores += gr.Opciones.Sum(x => x.ValorAsegurado);
                }
                else
                {
                    sumaPrimas += gr.Opciones.Where(x => x.IndiceOpcion == 1).Sum(x => x.PrimaAnualIndividual);
                    sumavalores += gr.Opciones.Where(x => x.IndiceOpcion == 1).Sum(x => x.ValorAsegurado);
                }
            }
            // Prima Asegurada individual y Valores Asegurados Individuales
            var tasaGeneral = this.gruposUtilities.CalcularTasaGeneral(sumaPrimas, sumavalores);

            if (tieneSiniestralidad)
            {
                var tasaSiniestralidad = informacionSiniestralidad.ProyeccionFinanciera.TasaComercialAnual;
                if (fichaAlterna)
                {
                    if (tasaGeneral > tasaSiniestralidad)
                    {

                    }
                    else
                    {
                        foreach (var ga in gruposAsegurados)
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
                        foreach (var ga in gruposAsegurados)
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
            }


            var primassinDistribucion = this.gruposAsegurados.Where(x => x.ConDistribucionAsegurados == false).Select(g => g.Primas.PrimaTotalAnual.FirstOrDefault().Valor);
            var primasDistribucion = this.gruposAsegurados.Where(x => x.ConDistribucionAsegurados == true).Select(g => g.Primas.PrimaTotalAnual.Sum(x => x.Valor));
            var primas = primassinDistribucion.Sum(x => x) + primasDistribucion.Sum(x => x);
            var sumPrimaTotalAnualGrupos = primas;
            return sumPrimaTotalAnualGrupos;
        }

        public async Task<PygAnualFichaTecnica> CalcularPyGAnualAsync(InformacionFactorG informacionFactorG)
        {
            var grupoAsegurado = this.gruposAsegurados.FirstOrDefault();

            var result = new PygAnualFichaTecnica();
            var tieneSiniestralidad = this.informacionNegocio.CodigoTipoTasa1 == 5 || this.informacionNegocio.CodigoTipoTasa2 == 5;
            decimal siniestralidad = 0;
            var factorG = (decimal)this.informacionNegocio.FactorG;
            var totalSiniestralidadConIBNR = informacionSiniestralidad.Totales.SumValorPlusIBNR;
            decimal valorAsistencia = 0;
            // TODO solo toma el primer grupo y la primera opcion            
            //var amparoAsistencia = grupoAsegurado.Asistencias.Where(x => x.CodigoAmparo == 95).FirstOrDefault();
            foreach (var grupo in this.gruposAsegurados)
            {
                if (grupo.Asistencias.Count() > 0)
                {
                    // 20-01-2022 Se cambia la obtención de el valor de asistencia para la asistencia proveedor en el PyG, pues anteriormente no se indicó que deberia sumarse los valores de las opciones para este amparos
                    if (grupo.ConDistribucionAsegurados)
                    {
                        var sumAssitencias = grupo.Asistencias.Where(x => x.CodigoAmparo == 95).Sum(x => x.OpcionesValores.Sum(y => y.ValorAsegurado));
                        valorAsistencia = valorAsistencia + sumAssitencias;
                    }
                    else
                    {
                        valorAsistencia = valorAsistencia + grupo.Asistencias.Where(x => x.CodigoAmparo == 95).FirstOrDefault().OpcionesValores.FirstOrDefault().ValorAsegurado;
                    }
                }
            }

            // Inicializa P&G data processor
            var siniestralidadDataProcessor = new SiniestralidadDataProcessor(codigoCotizacion, this.informacionNegocio.IBNR, factorG);

            var primaTotal = this.CalcularPrimaAnualPyG();
            var primaSeguro = primaTotal - valorAsistencia;
            var siniestrosIncurridos = siniestralidadDataProcessor.CalcularSiniestralidad(tieneSiniestralidad, primaSeguro, valorAsistencia, this.gruposAsegurados, informacionSiniestralidad, totalSiniestralidadConIBNR);
            if (primaTotal > 0)
            {
                siniestralidad = siniestrosIncurridos / (primaTotal);
            }

            var asistenciaProveedor = (valorAsistencia * (1 - (factorG / 100))); //// (decimal)1.19;

            var comisionIntermediario = (primaTotal) * this.ConvertToPercentaje(informacionNegocio.PorcentajeComision);
            var gastosRetorno = (primaTotal) * this.ConvertToPercentaje(informacionNegocio.PorcentajeRetorno);
            var orosGastos = (primaTotal) * this.ConvertToPercentaje(informacionNegocio.PorcentajeOtrosGastos);
            var gastosCompania = (primaTotal) * this.ConvertToPercentaje(informacionNegocio.PorcentajeGastosCompania);
            var ivaComisionIntermediario = (primaTotal) * this.ConvertToPercentaje(informacionFactorG.IvaComisionIntermediario);
            var ivaGastosRetorno = (primaTotal) * this.ConvertToPercentaje(informacionFactorG.IvaGastosRetorno);
            var utilidad = primaTotal - asistenciaProveedor - siniestrosIncurridos - ivaComisionIntermediario - ivaGastosRetorno - comisionIntermediario -
                gastosRetorno - orosGastos - gastosCompania;
            decimal porcentajeUtilidadAnno = 0;
            if (primaTotal > 0)
            {
                porcentajeUtilidadAnno = Math.Round(utilidad, 0) / primaTotal;
            }

            result.PrimaTotal = primaTotal;
            result.Asistencia = asistenciaProveedor;
            result.SiniestrosIncurridos = siniestrosIncurridos;
            result.Siniestralidad = siniestralidad;
            result.ComisionIntermediario = comisionIntermediario;
            result.IvaComisionIntermediario = ivaComisionIntermediario;
            result.GastosRetorno = gastosRetorno;
            result.IvaGastosRetorno = ivaGastosRetorno;
            result.OtrosGastos = orosGastos;
            result.GastosCompania = gastosCompania;
            result.Utilidad = utilidad;
            result.PorcentajeSinietralidad = 1;
            result.PorcentajeUtilidadAnno = Math.Round(porcentajeUtilidadAnno, 2);

            return result;
        }

        private decimal ConvertToPercentaje(decimal value)
        {
            return value / 100;
        }
    }
}

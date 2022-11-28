using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.SISEServices;
using ModernizacionPersonas.Entities;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class CotizacionDataProcessorBase
    {
        private readonly IDatosRangoGrupoAseguradoWriter datosRangosWriter;
        private readonly IDatosRangoGrupoAseguradoReader datosRangosReader;
        private readonly IDatosTasaOpcionReader tasaOpcionesReader;
        private readonly IDatosTasaOpcionWriter tasaOpcionesWriter;
        private readonly IDatosOpcionValorAseguradoWriter opcionValorWriter;
        private readonly SISEAseguradosSummaryProcessor SISETasasProcesor;
        private readonly SiniestralidadDataProcessor siniestralidadDataProcessor;

        public CotizacionDataProcessorBase(int codigoCotizacion, decimal IBNR, decimal factorG)
        {
            this.datosRangosReader = new DatosRangoGrupoAseguradoTableReader();
            this.datosRangosWriter = new DatosRangoGrupoAseguradoTableWriter();
            this.tasaOpcionesReader = new DatosTasaOpcionTableReader();
            this.tasaOpcionesWriter = new DatosTasaOpcionTableWriter();
            this.opcionValorWriter = new DatosOpcionValorAseguradoTableWriter();
            this.SISETasasProcesor = new SISEAseguradosSummaryProcessor();

            this.siniestralidadDataProcessor = new SiniestralidadDataProcessor(codigoCotizacion, IBNR, factorG);
        }

        public int CalcularEdad(DateTime dateOfBirth)
        {
            // TODO debe moverse a una carpeta de utilidades
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            int currentYearDayOf = DateTime.Now.DayOfYear;
            int dirthDateYearDayOf = dateOfBirth.DayOfYear;
            if (DateTime.IsLeapYear(DateTime.Now.Year))
                currentYearDayOf -= 1;
            if (DateTime.IsLeapYear(dateOfBirth.Year))
                dirthDateYearDayOf -= 1;
            if (currentYearDayOf < dirthDateYearDayOf)
                age -= 1;

            return age;
        }

        public async Task<GetTasasAmparosResponse> GetTasasAmparosAsync(InformacionNegocio informacionNegocio, GrupoAsegurado grupo)
        {
            var codigoCotizacion = informacionNegocio.CodigoCotizacion;
            var args = new SISEAseguradosProcessorArgs()
            {
                CodigoProceso = 1,
                CodigoPerfilEdades = informacionNegocio.CodigoPerfilEdad,
                CodigoPerfilValores = informacionNegocio.CodigoPerfilValor,
                CodigoRamo = informacionNegocio.CodigoRamo,
                CodigoSubRamo = informacionNegocio.CodigoSubramo,
                CodigoSector = informacionNegocio.CodigoSector,
                CodigoCotizacion = codigoCotizacion,
                CodigoGrupoAsegurados = grupo.CodigoGrupoAsegurado,
                CodigoTipoTasa = informacionNegocio.CodigoTipoTasa1,
                CntAsegurados = grupo.NumeroAsegurados,
                Listado = informacionNegocio.ConListaAsegurados,
            };

            var response = await this.SISETasasProcesor.GetTasasAmparosAsync(args);
            if (response.TasasAmparos.Count() == 0)
            {
                throw new Exception($"No se encontraron tasas parametrizadas para el código de cotización {codigoCotizacion}");
            }

            return response;
        }


        private async Task<IEnumerable<TasaRangoEdad>> GetTasasRangosAsync(int codigoCotizacion, int codigoSector, GrupoAsegurado grupo)
        {
            var args = new SISECalcularTasaRangoArgs()
            {
                CodigoProceso = 1,
                CodigoCotizacion = codigoCotizacion,
                CodigoGrupoAsegurados = grupo.CodigoGrupoAsegurado,
                CodigoSector = codigoSector
            };

            var tasas = await this.SISETasasProcesor.CalcularTasasPorRangosAsync(args);
            var tasasAmparos = tasas;
            return tasasAmparos;
        }

        /// <summary>
        /// Calcula la prima anual del amparo
        /// </summary>
        /// <param name="tasaComercial"></param>
        /// <param name="valorAsegurado"></param>
        /// <returns></returns>
        public decimal CalcularPrimaOpcion(decimal tasaComercial, decimal valorAsegurado)
        {
            var result = tasaComercial * valorAsegurado / 1000;
            return Math.Round(result, 0);
        }

        /// <summary>
        /// Calcula prima anual de amparo basico no adicional por lista de asegurados
        /// </summary>
        /// <param name="asegurados"></param>
        /// <param name="tasasAsegurados"></param>
        /// <param name="numeroSalarios"></param>
        /// <param name="factorG"></param>
        /// <returns></returns>
        public decimal CalcularPrimaOpcion(IEnumerable<Asegurado> asegurados, List<TasaAsegurado> tasasAsegurados, decimal numeroSalarios, decimal factorG)
        {
            var factorg = factorG / 100;
            foreach (var asegurado in asegurados)
            {
                var vlrAsegBasicoVida = asegurado.ValorAsegurado * numeroSalarios;
                var itemTasaRiesgoAseg = tasasAsegurados.Where(x => x.NumeroDocumento == asegurado.NumeroDocumento).FirstOrDefault();
                var tasaComerBasic = itemTasaRiesgoAseg.Tasa / (1 - factorg);
                var primaBasica = tasaComerBasic * vlrAsegBasicoVida / 1000;

                asegurado.ValorPrimaAsegurado = primaBasica;
            }

            var result = asegurados.Sum(x => x.ValorPrimaAsegurado);
            return Math.Round(result, 6);
        }

        public decimal CalcularTasaAnual(decimal primaTotalAnual, decimal valorAsegurado)
        {
            if (valorAsegurado > 0)
            {
                decimal result = primaTotalAnual / valorAsegurado * 1000;
                return Math.Round(result, 6);
            }

            return 0;
        }

        public decimal CalcularValorAsistenciaOpcion(GrupoAsegurado grupo, OpcionValorAsegurado opcion)
        {
            //decimal result = 0;
            //if (grupo.ConDistribucionAsegurados)
            //{
            //    result = opcion.Prima * opcion.NumeroAsegurados;
            //}
            //else
            //{
            //    result = opcion.Prima * grupo.NumeroAsegurados;
            //}
            //return result;
            return opcion.Prima * grupo.NumeroAsegurados;
        }

        public decimal CalcularTasaComercialOpcion(decimal tasaRiesgo, decimal factorG)
        {
            var factorg = factorG / 100;
            decimal result = tasaRiesgo / (1 - factorg);
            return Math.Round(result, 6);
        }

        public decimal CalcularTasaComercialTotal(decimal tasaComercialAnual, decimal porcentajeDescuento, decimal porcentajeRecargo)
        {
            var result = tasaComercialAnual - (tasaComercialAnual * porcentajeDescuento / 100) + (tasaComercialAnual * porcentajeRecargo / 100);
            return Math.Round(result, 6);
        }

        public decimal CalcularPrimaOpcionPorAsegurados(decimal factorG, List<TasaAsegurado> tasasAsegurados, int codigoTipoTasa, decimal valorAsegurado, decimal tasaComercial)
        {
            decimal factorg = factorG / 100;
            decimal prima = 0;
            // if codigoTipoTasa Tasa por edad promedio 
            if (codigoTipoTasa == 1)
            {
                prima = tasaComercial * valorAsegurado / 1000;
            }
            foreach (var item in tasasAsegurados)
            {
                var tasaComercialAsegurado = item.Tasa / (1 - factorg);
                var primaAsegurado = tasaComercialAsegurado * item.ValorAsegurado / 1000;
                prima += primaAsegurado;
            }

            return Math.Round(prima, 0);
        }

        public decimal CalcularPrimaOpcionPorAsegurados(decimal factorG, List<TasaAsegurado> tasasAsegurados)
        {
            decimal factorg = factorG / 100;
            decimal prima = 0;
            // if codigoTipoTasa Tasa por edad promedio 
           
            foreach (var item in tasasAsegurados)
            {
                var tasaComercialAsegurado = item.Tasa / (1 - factorg);
                var primaAsegurado = tasaComercialAsegurado * item.ValorAsegurado / 1000;
                prima += primaAsegurado;
            }

            return Math.Round(prima, 0);
        }

        public decimal CalcularPrimaOpcionPorSalario(decimal factorG, List<TasaAsegurado> tasasAsegurados, decimal numeroSalarios, int tipoTasa)
        {
            decimal factorg = factorG / 100;
            decimal sumaPrimaBasico = 0;
            foreach (var item in tasasAsegurados)
            {
                decimal salario = item.ValorAsegurado;
                if (tipoTasa == 2)
                {
                    var valorAsegurado = numeroSalarios * salario;
                    var tasaComercialBasico = item.Tasa / (1 - factorg);
                    var primaBasico = Math.Round(tasaComercialBasico, 6) * valorAsegurado / 1000;
                    sumaPrimaBasico += Math.Round(primaBasico, 0);
                }
            }

            return Math.Round(sumaPrimaBasico, 0);
        }

        public decimal CalcularPrimaOpcionPorSalario(decimal factorG, List<TasaAsegurado> tasasAsegurados, decimal numeroSalarios, int tipoTasa, decimal tasaComercial)
        {
            decimal factorg = factorG / 100;
            decimal sumaPrimaBasico = 0;
            decimal valorAsegurado = tasasAsegurados.Sum(x => x.ValorAsegurado);

            sumaPrimaBasico = Math.Round(tasaComercial, 6) * ((valorAsegurado * numeroSalarios) / 1000);

            return Math.Round(sumaPrimaBasico, 0);
        }

        public decimal CalcularPrimaTotalAnual(decimal valorAsistencia, decimal valorPrimaAnual)
        {
            var result = valorPrimaAnual + valorAsistencia;
            return Math.Round(result, 0);
        }

        public Dictionary<int, decimal> GetSumatoriaPrimaAmparoOpcion(GrupoAsegurado grupo)
        {
            var count = 3;
            var result = new Dictionary<int, decimal>();
            var totalOpciones = new decimal[count];
            foreach (var a in grupo.AmparosGrupo)
            {
                // No se debe sumar si el amparo es de tipo asistencia, CodigoGrupoAsegurado 95
                if (a.CodigoAmparo != 95)
                {
                    a.OpcionesValores.ForEach(o =>
                    {
                        var s = Math.Round(o.Prima, 0);
                        totalOpciones[o.IndiceOpcion - 1] += s;
                    });
                }
            }

            for (int i = 0; i < count; i++)
            {
                result.Add(i, totalOpciones[i]);
            }

            return result;
        }

        public Dictionary<int, decimal> GetSumatoriaPrimaAmparoOpcionSinAplicar(GrupoAsegurado grupo)
        {
            var count = 3;
            var result = new Dictionary<int, decimal>();
            var totalOpciones = new decimal[count];
            foreach (var a in grupo.AmparosGrupo)
            {
                // No se debe sumar si el amparo es de tipo asistencia, CodigoGrupoAsegurado 95
                if (a.CodigoAmparo != 95)
                {
                    a.OpcionesValores.ForEach(o =>
                    {
                        var s = Math.Round(o.PrimaSinAplicar, 0);
                        totalOpciones[o.IndiceOpcion - 1] += s;
                    });
                }
            }

            for (int i = 0; i < count; i++)
            {
                result.Add(i, totalOpciones[i]);
            }

            return result;
        }

        public decimal GetValorAsistencia(GrupoAsegurado grupo, int opcionIndex, decimal factorG)
        {
            decimal valorAsistenciaIndividual = 0;
            
            var tieneAsistencia = grupo.AmparosGrupo.Where(x => x.CodigoGrupoAmparo == 3).Any();
            if (tieneAsistencia)
            {
                var asistencia = grupo.AmparosGrupo.Where(x => x.CodigoGrupoAmparo == 3).FirstOrDefault();
                if(grupo.ConDistribucionAsegurados && grupo.CodigoTipoSuma == 1)
                {
                    valorAsistenciaIndividual = asistencia.OpcionesValores[opcionIndex - 1].Prima / grupo.NumeroAsegurados ;
                }
                else
                {
                    valorAsistenciaIndividual = asistencia.OpcionesValores[opcionIndex - 1].Prima;// / grupo.NumeroAsegurados ;
                }
                valorAsistenciaIndividual = valorAsistenciaIndividual * ((decimal)1.19 /( 1 - factorG));                
            }
           
            return valorAsistenciaIndividual;
        }

        public async Task UpdateTasasPrimasAmparoAsync(int codigoOpcionValorAsegurado, decimal tasaRiesgo, decimal tasaComercial, decimal prima)
        {
            var roundValue = Math.Round(prima, 0);
            await this.opcionValorWriter.UpdateTasasPrimasAmparoAsync(codigoOpcionValorAsegurado, tasaRiesgo, tasaComercial, roundValue);
        }

        public async Task UpdatePonderacionAmparoAsync(int codigoOpcionValorAsegurado, int indiceOpcion, decimal ponderacion)
        {
            await this.opcionValorWriter.UpdatePonderacionAmparoAsync(codigoOpcionValorAsegurado, indiceOpcion, ponderacion);
        }

        public async Task<IEnumerable<Rango>> LeerRangoGrupoAseguradoAsync(int codigoGrupoAsegurado)
        {
            return await this.datosRangosReader.LeerRangoGrupoAseguradoAsync(codigoGrupoAsegurado);
        }

        public async Task ActualizarRangoGrupoAseguradoAsync(Rango rango)
        {
            await this.datosRangosWriter.ActualizarRangoGrupoAseguradoAsync(rango.CodigoRangoGrupoAsegurado, rango);
        }

        public async Task<TasaOpcion> LeerTasaOpcionAsync(int codigoGrupoAsegurado, int indiceOpcion)
        {
            var tasaOpcion = await this.tasaOpcionesReader.LeerTasaOpcionAsync(codigoGrupoAsegurado, indiceOpcion);
            return tasaOpcion;
        }

        public decimal CalcularPrimaIndividualAnual(decimal valorAsegurado, decimal tasaComercialAplicar)
        {
            var result = valorAsegurado * tasaComercialAplicar / 1000;
            return Math.Round(result, 0);
        }

        public GrupoAseguradoResumen MapGrupoAseguradoResumen(GrupoAsegurado grupo, bool conListaAsegurados)
        {
            var result = new List<GrupoAseguradoResumen>();
            var amparos = grupo.AmparosGrupo.Select(x => x.AmparoInfo).ToList();
            // var opciones = await this.MapOpcionesAmparoAsync(grupo);
            var ga = new GrupoAseguradoResumen
            {
                Codigo = grupo.CodigoGrupoAsegurado,
                ConListaAsegurados = conListaAsegurados,
                EdadPromedio = grupo.EdadPromedioAsegurados,
                Nombre = grupo.NombreGrupoAsegurado,
                NumeroAsegurados = grupo.NumeroAsegurados,
                // NumeroOpciones = opciones.Count(),
                Amparos = amparos,
                ConDistribucionAsegurados = grupo.ConDistribucionAsegurados,
                AseguradosOpcion1 = grupo.AseguradosOpcion1,
                AseguradosOpcion2 = grupo.AseguradosOpcion2,
                AseguradosOpcion3 = grupo.AseguradosOpcion3
                // Opciones = opciones,
                // TipoSumaAsegurada = tipoSumaAsegurada
            };

            return ga;
        }

        public async Task<IEnumerable<Rango>> ProcessTasasRangosAsync(InformacionNegocio informacionNegocio, GrupoAsegurado grupo)
        {
            var result = new List<Rango>();

            var codigoCotizacion = informacionNegocio.CodigoCotizacion;
            var factorG = informacionNegocio.FactorG;
            var codigoSector = informacionNegocio.CodigoSector;

            var tasasRangos = await this.GetTasasRangosAsync(codigoCotizacion, codigoSector, grupo);
            var rangos = await this.LeerRangoGrupoAseguradoAsync(grupo.CodigoGrupoAsegurado);
            var idx = 0;
            foreach (var rango in rangos)
            {
                var tasaRango = tasasRangos.ToList()[idx];
                if (tasaRango != null)
                {
                    var tasaComercial = this.CalcularTasaComercialOpcion(tasaRango.Tasa, factorG);
                    var primaBasico = this.CalcularPrimaIndividualAnual(rango.ValorAsegurado, tasaComercial);
                    rango.TasaRiesgo = tasaRango.Tasa;
                    rango.TasaComercial = tasaComercial;
                    rango.ValorPrimaBasico = primaBasico;
                    await this.ActualizarRangoGrupoAseguradoAsync(rango);

                    result.Add(rango);
                }

                idx++;
            }

            return result;
        }

        public async Task ProcessPonderacionSiniestralidadAsync(List<PrimaAmparoItem> primasAmparos, int optionIndex)
        {
            decimal sumaPonderacion = 0;
            var primaIndividualAnio = primasAmparos.Where(x => x.IndexOption == optionIndex).Sum(x => x.Value);
            if (primaIndividualAnio > 0)
            {
                var primasxAmparo = primasAmparos.Where(x => x.IndexOption == optionIndex).Select(x => x);
                var ponderacionxAmparoArray = new decimal[primasxAmparo.Count()];
                var index = 0;
                foreach (var primaAmparo in primasxAmparo)
                {
                    var valorPrimaAmparo = primaAmparo.Value;
                    var ponderacion = this.CalcularPonderacionSiniestralidad(primaIndividualAnio, valorPrimaAmparo);
                    sumaPonderacion += ponderacion;
                    await this.UpdatePonderacionAmparoAsync(primaAmparo.CodigoOpcionValorAsegurado, primaAmparo.IndexOption, ponderacion);
                    ponderacionxAmparoArray[index] = ponderacion;
                    index++;
                }

                if (sumaPonderacion < 99)
                {
                    throw new Exception("Hubo un error calculando el valor de ponderación. La sumatoria de ponderación es menor a 100%.");
                }
            }
        }

        public SiniestralidadResumen MapOpcionesSiniestralidadResumen(Amparo amparoBasicoNoAdicional, OpcionValorAsegurado opcion, int numeroAsegurados, decimal factorG, TasaOpcion o)
        {
            var porcentajeDescuento = o.DescuentoSiniestralidad;
            var porcentajeRecargo = o.RecargoSiniestralidad;

            var tasaComercial = this.CalcularTasaComercialOpcion(opcion.TasaRiesgoSiniestralidad, factorG);
            var tasaComercialAplicar = this.CalcularTasaComercialTotal(tasaComercial, porcentajeDescuento, porcentajeRecargo);

            var primaAnualTotal = this.CalcularPrimaIndividualAnual(opcion.ValorAsegurado, tasaComercialAplicar);
            var result = new SiniestralidadResumen
            {
                Amparo = amparoBasicoNoAdicional,
                TasaRiesgo = opcion.TasaRiesgoSiniestralidad,
                TasaComercial = tasaComercial,
                PorcentajeDescuento = o.DescuentoSiniestralidad,
                PorcentajeRecargo = o.RecargoSiniestralidad,
                TasaComercialAplicar = tasaComercialAplicar,
                PrimaAnualTotal = primaAnualTotal
            };

            return result;
        }

        public async Task<decimal> CalcularTasaRiesgoSiniestralidad(decimal valorAseguradoTotal)
        {
            var tasaSiniestralidad = await this.siniestralidadDataProcessor.CalcularTasaSiniestralidadAsync(valorAseguradoTotal);
            return Math.Round(tasaSiniestralidad, 6);
        }

        public async Task UpdateTasaOpcionValuesAsync(TasaOpcion tasaOpcion)
        {
            await this.tasaOpcionesWriter.EliminarTasaOpcionAsync(tasaOpcion.CodigoGrupoAsegurado, tasaOpcion.IndiceOpcion);
            //await this.tasaOpcionesWriter.ActualizarTasaOpcionAsync(tasaOpcion);
            await this.tasaOpcionesWriter.CrearTasaOpcionAsync(tasaOpcion);
        }

        public async Task<IEnumerable<ProyeccionFinanciera>> CalcularProyeccionFinancieraGrupoAsync(InformacionNegocio informacionNegocio, GrupoAsegurado grupo)
        {
            var codigoTipoTasa = informacionNegocio.CodigoTipoTasa1 == 5 && informacionNegocio.CodigoTipoTasa2 != 0
                ? informacionNegocio.CodigoTipoTasa2
                : informacionNegocio.CodigoTipoTasa1;

            var tieneSiniestralidad = informacionNegocio.CodigoTipoTasa1 == 5 || informacionNegocio.CodigoTipoTasa2 == 5;
            var factorG = informacionNegocio.FactorG;
            var optionsCount = grupo.CodigoTipoSuma == 1 ? 3 : 1;

            var result = new List<ProyeccionFinanciera>();
            var item = new ProyeccionFinanciera();

            

            if (tieneSiniestralidad)
            {
                var item2 = new ProyeccionFinanciera();
                item2.EsProyeccionSiniestralidad = true;
                for (int i = 1; i < optionsCount + 1; i++)
                {
                    var indice = i;
                    var tasaOpcion = await this.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, 1);
                    var porcentajeDescuento = tasaOpcion.DescuentoSiniestralidad;
                    var porcentajeRecargo = tasaOpcion.RecargoSiniestralidad;
                    var tasaComercialAnual = this.CalcularTasaComercialOpcion(tasaOpcion.TasaSiniestralidad, factorG);
                    var tasaComercialTotal = this.CalcularTasaComercialTotal(tasaComercialAnual, tasaOpcion.DescuentoSiniestralidad, tasaOpcion.RecargoSiniestralidad);

                    item2.PorcentajeDescuento.Add(new ValorOpcionKeyValue { IndiceOpcion = indice, Valor = porcentajeDescuento });
                    item2.PorcentajeRecargo.Add(new ValorOpcionKeyValue { IndiceOpcion = indice, Valor = porcentajeRecargo });
                    item2.TasaComercialAnual.Add(new ValorOpcionKeyValue { IndiceOpcion = indice, Valor = tasaComercialAnual });
                    item2.TasaComercialTotal.Add(new ValorOpcionKeyValue { IndiceOpcion = indice, Valor = tasaComercialTotal });

                    indice++;
                }

                result.Add(item2);
            }
            else
            {
                if (codigoTipoTasa != 5)
                {
                    var amparoBasicoNoAdicional = grupo.AmparosGrupo.Where(x => !x.AmparoInfo.SiNoAdicional && x.AmparoInfo.SiNoBasico).FirstOrDefault();
                    for (int i = 1; i < optionsCount + 1; i++)
                    {
                        var indice = i;
                        var tasaOpcion = await this.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, indice);
                        var porcentajeDescuento = tasaOpcion.Descuento;
                        var porcentajeRecargo = tasaOpcion.Recargo;
                        var tasaComercialAnual = tasaOpcion.TasaComercial;
                        var tasaComercialTotal = tasaOpcion.TasaComercialTotal;

                        item.EsProyeccionSiniestralidad = false;
                        item.PorcentajeDescuento.Add(new ValorOpcionKeyValue { IndiceOpcion = indice, Valor = porcentajeDescuento });
                        item.PorcentajeRecargo.Add(new ValorOpcionKeyValue { IndiceOpcion = indice, Valor = porcentajeRecargo });
                        item.TasaComercialAnual.Add(new ValorOpcionKeyValue { IndiceOpcion = indice, Valor = tasaComercialAnual });
                        item.TasaComercialTotal.Add(new ValorOpcionKeyValue { IndiceOpcion = indice, Valor = tasaComercialTotal });

                        indice++;
                    }

                    result.Add(item);
                }
            }


            return result;
        }

        public decimal CalcularPonderacionSiniestralidad(decimal primaIndividualAnio, decimal primaAmparo)
        {
            return (primaAmparo / primaIndividualAnio) * 100;
        }

        public async Task<InformacionSiniestralidad> BuildSiniestralidadDataAsync(int codigoCotizacion, int codigoRamo, IEnumerable<GrupoAseguradoFichaTecnica> gruposAsegurados, bool tieneTasaSiniestralidad, decimal valorTotalAsegurado, int numeroAsegurados)
        {
            var result = new InformacionSiniestralidad();
            if (gruposAsegurados.Count() == 1 && tieneTasaSiniestralidad)
            {
                var grupo = gruposAsegurados.FirstOrDefault();
                var proyeccionFinanciera = (from proyecciones in grupo.ProyeccionesFinancieras
                                            where proyecciones.EsProyeccionSiniestralidad
                                            select proyecciones).FirstOrDefault();

                result = await this.siniestralidadDataProcessor.ObtenerSiniestralidadAsync(codigoCotizacion, proyeccionFinanciera, grupo.TasaSiniestralidad, valorTotalAsegurado, numeroAsegurados);
            }
            else
            {
                result = await this.siniestralidadDataProcessor.ObtenerSiniestralidadAsync(codigoCotizacion, codigoRamo, valorTotalAsegurado);
            };

            return result;
        }
    }

    public class PrimaAmparoItem
    {
        public PrimaAmparoItem(int codigoAmparo, int codigoGrupoAsegurado, int codigoOpcionValorAsegurado, int indiceOpcion, decimal primaAmparo)
        {
            CodigoAmparo = codigoAmparo;
            CodigoGrupoAsegurado = codigoGrupoAsegurado;
            CodigoOpcionValorAsegurado = codigoOpcionValorAsegurado;
            IndexOption = indiceOpcion;
            Value = primaAmparo;
        }

        public int CodigoAmparo { get; }
        public int CodigoGrupoAsegurado { get; }
        public int CodigoOpcionValorAsegurado { get; }
        public int IndexOption { get; }
        public decimal Value { get; }
    }
}

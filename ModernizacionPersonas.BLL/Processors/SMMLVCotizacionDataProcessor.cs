using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.Entities;
using ModernizacionPersonas.DAL.Services;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class SMMLVCotizacionDataProcessor : CotizacionDataProcessorBase, IResumenDataProcessor2
    {
        private readonly int codigoTipoTasa;
        private readonly bool tieneSiniestralidad;
        private readonly decimal valorSalarioMinimo;
        private readonly TipoSumaAsegurada tipoSumaAsegurada;
        private readonly bool conListaAsegurados;
        private readonly IEnumerable<Asegurado> asegurados;
        private readonly IEnumerable<Asegurado> aseguradosNoVetados;
        private readonly IDatosGruposAseguradoReader gruposReader;
        private readonly IDatosAmparoGrupoAseguradoReader amparoGrupoAseguradoReaderService;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosOpcionValorAseguradoReader opcionValorReaderService;
        private readonly IDatosTasaOpcionWriter tasaOpcionWirterService;

        public SMMLVCotizacionDataProcessor(Entities.CotizacionDataProcessorArgs args)
            : base(args.CodigoCotizacion, args.IBNR, args.FactorG)
        {
            this.codigoTipoTasa = args.CodigoTipoTasa;
            this.tieneSiniestralidad = args.TieneSiniestralidad;
            this.valorSalarioMinimo = args.ValorSalarioMinimo;
            this.tipoSumaAsegurada = args.TipoSumaAsegurada;
            this.conListaAsegurados = args.ConListaAsegurados;
            this.gruposReader = new DatosGruposAseguradosTableReader();
            this.amparoGrupoAseguradoReaderService = new DatosAmparoGrupoAseguradoTableReader();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.opcionValorReaderService = new DatosOpcionValorAseguradoTableReader();
            this.tasaOpcionWirterService = new DatosTasaOpcionTableWriter();

            this.asegurados = args.Asegurados;
            if (this.asegurados != null)
            {
                this.aseguradosNoVetados = args.Asegurados.Where(x => !x.VetadoSarlaft);
            }
        }

        public async Task<GrupoAseguradoResumen> BuildGrupoAseguradoResumen(InformacionNegocio informacionNegocio, GrupoAsegurado grupo)
        {
            var primasAmparos = new List<PrimaAmparoItem>();
            var codigoCotizacion = informacionNegocio.CodigoCotizacion;
            var factorG = informacionNegocio.FactorG;
            var esTipoSumaAseguradaSalario = grupo.CodigoTipoSuma == 2 || grupo.CodigoTipoSuma == 5 || grupo.CodigoTipoSuma == 10;
            var esTasaPorEdad = codigoTipoTasa == 2 || codigoTipoTasa == 3 || codigoTipoTasa == 4;
            var tasasResult = await this.GetTasasAmparosAsync(informacionNegocio, grupo);

            var rangos = await this.ProcessTasasRangosAsync(informacionNegocio, grupo);
            foreach (var amparo in grupo.AmparosGrupo)
            {
                var esAmparoBasicoNoAdicional = amparo.AmparoInfo.SiNoBasico && !amparo.AmparoInfo.SiNoAdicional;
                var esAsistencia = amparo.AmparoInfo.CodigoGrupoAmparo == 3;
                var tasaAmparo = tasasResult.TasasAmparos.Find(x => x.CodigoAmparo == amparo.CodigoAmparo);
                if (tasaAmparo == null && tasasResult.TasasAsegurados.Count() == 0)
                {
                    throw new Exception($"No hay ningúna tasa parametrizada para el código amparo {amparo.CodigoAmparo}");
                }

                Decimal tasaRiesgo = 0;
                tasaRiesgo = tasaAmparo.Tasa > 0 ? tasaAmparo.Tasa : 0;

                var opciones = amparo.OpcionesValores.Where(x => x.ValorAsegurado > 0 || x.NumeroSalarios > 0 || x.PorcentajeCobertura > 0 || x.Prima > 0);
                foreach (var opcion in opciones)
                {
                    tasaRiesgo = tasaAmparo.Tasa;
                    var valorAseguradoOpcion = this.CalcularValorAseguradoAmparoOpcion(grupo, opcion, esAsistencia);
                    var tasaComercial = this.CalcularTasaComercialOpcion(tasaRiesgo, factorG);
                    var tasasOpcion = this.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, opcion.IndiceOpcion).Result;
                    var tasaComercialAplicar = tasaComercial - (tasaComercial * tasasOpcion.Descuento / 100) + (tasaComercial * tasasOpcion.Recargo / 100);
                    var primaAmparo = esAsistencia ? opcion.Prima : this.CalcularPrimaOpcion(tasaComercialAplicar, valorAseguradoOpcion);
                    var primaAmparoSinAplicar = esAsistencia ? opcion.Prima : this.CalcularPrimaOpcion(tasaComercial, valorAseguradoOpcion);
                    if (esAmparoBasicoNoAdicional)
                    {
                        primaAmparo = rangos.Sum(x => x.ValorPrimaBasico);
                        if (informacionNegocio.ConListaAsegurados)
                        {
                            this.aseguradosNoVetados.ToList().ForEach(x => { x.Edad = this.CalcularEdad(x.FechaNacimiento); });
                            var edadPromedio = grupo.EdadPromedioAsegurados;//Math.Round(this.aseguradosNoVetados.Average(x => x.Edad));
                            var rango = rangos.Where(x => x.EdadMaxAsegurado >= edadPromedio && x.EdadMinAsegurado <= edadPromedio).FirstOrDefault();
                            if (rango != null && rango.TasaRiesgo > 0)
                            {
                                tasaRiesgo = rango.TasaRiesgo;
                                tasaComercial = rango.TasaComercial;
                                tasaComercialAplicar = tasaComercial - (tasaComercial * tasasOpcion.Descuento / 100) + (tasaComercial * tasasOpcion.Recargo / 100);
                            }
                            else
                            {
                                tasaRiesgo = tasasResult.TasasAsegurados.FirstOrDefault().Tasa;
                                tasaComercial = this.CalcularTasaComercialOpcion(tasaRiesgo, factorG);
                                tasaComercialAplicar = tasaComercial - (tasaComercial * tasasOpcion.Descuento / 100) + (tasaComercial * tasasOpcion.Recargo / 100);

                            }

                            primaAmparo = esTipoSumaAseguradaSalario
                                ? rango != null && rango.TasaRiesgo < 0 
                                ? this.CalcularPrimaOpcionPorSalario(factorG, tasasResult.TasasAsegurados, opcion.NumeroSalarios, codigoTipoTasa) 
                                : this.CalcularPrimaOpcionPorSalario(factorG, tasasResult.TasasAsegurados, opcion.NumeroSalarios, codigoTipoTasa, tasaComercialAplicar)
                                : this.CalcularPrimaOpcionPorAsegurados(factorG, tasasResult.TasasAsegurados);
                            primaAmparoSinAplicar = esTipoSumaAseguradaSalario
                            ? rango != null && rango.TasaRiesgo < 0
                                ? this.CalcularPrimaOpcionPorSalario(factorG, tasasResult.TasasAsegurados, opcion.NumeroSalarios, codigoTipoTasa)
                                : this.CalcularPrimaOpcionPorSalario(factorG, tasasResult.TasasAsegurados, opcion.NumeroSalarios, codigoTipoTasa, tasaComercial)
                            : this.CalcularPrimaOpcionPorAsegurados(factorG, tasasResult.TasasAsegurados);
                            if (codigoTipoTasa == 4)
                            {
                                primaAmparo = tasaComercialAplicar * valorAseguradoOpcion / 1000;
                                primaAmparoSinAplicar = tasaComercial * valorAseguradoOpcion / 1000;
                            }
                        }
                          
                        else if (!esAmparoBasicoNoAdicional && informacionNegocio.ConListaAsegurados)
                        {
                            tasaRiesgo = esAsistencia ? 0 : tasaAmparo.Tasa;

                            tasaComercial = this.CalcularTasaComercialOpcion(tasaRiesgo, factorG);
                            tasasOpcion = this.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, opcion.IndiceOpcion).Result;
                            tasaComercialAplicar = tasaComercial - (tasaComercial * tasasOpcion.Descuento / 100) + (tasaComercial * tasasOpcion.Recargo / 100);
                            primaAmparo = esAsistencia ? opcion.Prima : this.CalcularPrimaOpcion(tasaComercialAplicar, valorAseguradoOpcion);
                            primaAmparoSinAplicar = esAsistencia ? opcion.Prima : this.CalcularPrimaOpcion(tasaComercial, valorAseguradoOpcion);

                        }
                        else if (esAmparoBasicoNoAdicional && !informacionNegocio.ConListaAsegurados)
                        {
                            
                            if(codigoTipoTasa == 3)
                            {
                                // this.aseguradosNoVetados.ToList().ForEach(x => { x.Edad = this.CalcularEdad(x.FechaNacimiento); });
                                //-var edadPromedio = Math.Round(this.aseguradosNoVetados.Average(x => x.Edad));
                                var primasRango = new decimal[rangos.Count()];
                                var primasRangoSinAplicar = new decimal[rangos.Count()];
                                var idxRango = 0;
                                foreach (var rango in rangos)
                                {
                                    var tasaComercialRango = rango.TasaRiesgo / (1 - factorG/100);
                                    var tasaComercialRangoAplicar = tasaComercialRango 
                                                                    - (tasaComercialRango * tasasOpcion.Descuento / 100) 
                                                                    + (tasaComercialRango * tasasOpcion.Recargo / 100);
                                    var primaTempRango = tasaComercialRangoAplicar * rango.ValorAsegurado / 1000;
                                    var primaTempRangoSinAplicar = tasaComercialRango * rango.ValorAsegurado / 1000;
                                    primasRango[idxRango] = primaTempRango;
                                    primasRangoSinAplicar[idxRango] = primaTempRangoSinAplicar;
                                    idxRango++;
                                }


                                primaAmparo = primasRango.Sum();
                                primaAmparoSinAplicar = primasRangoSinAplicar.Sum();                           
                            }
                            else
                            {
                                tasaRiesgo = tasaAmparo.Tasa;
                                tasaComercial = this.CalcularTasaComercialOpcion(tasaRiesgo, factorG);
                                tasasOpcion = this.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, opcion.IndiceOpcion).Result;
                                tasaComercialAplicar = tasaComercial - (tasaComercial * tasasOpcion.Descuento / 100) + (tasaComercial * tasasOpcion.Recargo / 100);
                                primaAmparo = esAsistencia ? opcion.Prima : this.CalcularPrimaOpcion(tasaComercialAplicar, valorAseguradoOpcion);
                                primaAmparoSinAplicar = esAsistencia ? opcion.Prima : this.CalcularPrimaOpcion(tasaComercial, valorAseguradoOpcion);
                            }
                        }
                    }
                    else
                    {
                        tasaRiesgo = tasaAmparo.Tasa;
                        tasaComercial = this.CalcularTasaComercialOpcion(tasaRiesgo, factorG);
                        tasasOpcion = this.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, opcion.IndiceOpcion).Result;
                        tasaComercialAplicar = tasaComercial - (tasaComercial * tasasOpcion.Descuento / 100) + (tasaComercial * tasasOpcion.Recargo / 100);
                        primaAmparo = esAsistencia ? opcion.Prima : this.CalcularPrimaOpcion(tasaComercialAplicar, valorAseguradoOpcion);
                        primaAmparoSinAplicar = esAsistencia ? opcion.Prima : this.CalcularPrimaOpcion(tasaComercial, valorAseguradoOpcion);
                    }

                    // almacena prima amparo
                    primasAmparos.Add(new PrimaAmparoItem(amparo.CodigoAmparo, grupo.CodigoGrupoAsegurado, opcion.CodigoOpcionValorAsegurado, opcion.IndiceOpcion, primaAmparo));

                    // actualice la informacion de cada opcion en la DB
                    await this.UpdateTasasPrimasAmparoAsync(opcion.CodigoOpcionValorAsegurado, tasaRiesgo, tasaComercial, primaAmparo);

                    // actualice el objeto opcion
                    opcion.TasaRiesgo = tasaRiesgo;
                    opcion.TasaComercial = tasaComercial;
                    opcion.Prima = esAsistencia ? valorAseguradoOpcion : primaAmparo;
                    opcion.PrimaSinAplicar = esAsistencia ? valorAseguradoOpcion : primaAmparoSinAplicar;
                    opcion.ValorAsegurado = valorAseguradoOpcion;
                }
            }

            if (tieneSiniestralidad)
            {
                await this.ProcessPonderacionSiniestralidadAsync(primasAmparos, 1);
            }

            var opcionesResumen = await this.MapOpcionesAmparoAsync(informacionNegocio, grupo);
            var grupoResumen = this.MapGrupoAseguradoResumen(grupo, informacionNegocio.ConListaAsegurados);
            // complete grupo resumen
            grupoResumen.Opciones = opcionesResumen;
            grupoResumen.NumeroOpciones = opcionesResumen.Count();
            grupoResumen.TipoSumaAsegurada = tipoSumaAsegurada;

            return grupoResumen;
        }

        private async Task<IEnumerable<ValorAseguradoOpcionResumen>> MapOpcionesAmparoAsync(InformacionNegocio informacionNegocio, GrupoAsegurado grupo)
        {
            var result = new List<ValorAseguradoOpcionResumen>();
            var amparoBasicoNoAdicional = grupo.AmparosGrupo.ToList().Find(x => x.AmparoInfo.SiNoBasico && !x.AmparoInfo.SiNoAdicional);
            foreach (var o in amparoBasicoNoAdicional.OpcionesValores)
            {
                var opcion = await this.ProcessOpcionAmparoAsync(informacionNegocio, grupo, amparoBasicoNoAdicional, o);
                result.Add(opcion);
            }

            return result;
        }

        private decimal CalcularValorAseguradoAmparoOpcion(GrupoAsegurado grupo, OpcionValorAsegurado opcion, bool esAsistencia)
        {
            var valorAsegurado = opcion.ValorAsegurado;
            if (esAsistencia)
            {
                return this.CalcularValorAsistenciaOpcion(grupo, opcion);
            }

            var amparoBasicoNoAdicional = grupo.AmparosGrupo.Where(x => x.AmparoInfo.SiNoBasico && !x.AmparoInfo.SiNoAdicional).FirstOrDefault();
            var opcionBase = amparoBasicoNoAdicional.OpcionesValores[opcion.IndiceOpcion - 1];
            // TODO revisar si SMMLV recibe valores por porcentaje
            if (opcion.PorcentajeCobertura > 0)
            {
                valorAsegurado = this.valorSalarioMinimo * opcionBase.NumeroSalarios * grupo.NumeroAsegurados;
            }

            if (opcion.NumeroSalarios > 0)
            {
                valorAsegurado = opcion.ValorAsegurado = this.valorSalarioMinimo * opcion.NumeroSalarios * grupo.NumeroAsegurados;
            }
            else
            {
                valorAsegurado = opcion.ValorAsegurado * grupo.NumeroAsegurados;
            }

            return valorAsegurado;
        }

        public decimal CalcularValorAseguradoTotal(GrupoAsegurado grupo, OpcionValorAsegurado opcion, bool conListaAsegurados)
        {
            var amparoBasicoNoAdicional = grupo.AmparosGrupo.Where(x => x.AmparoInfo.SiNoBasico && !x.AmparoInfo.SiNoAdicional).FirstOrDefault();
            var opcionBase = amparoBasicoNoAdicional.OpcionesValores[opcion.IndiceOpcion - 1];
            var result = (valorSalarioMinimo * opcionBase.NumeroSalarios) * grupo.NumeroAsegurados;
            if (conListaAsegurados)
            {
                // si es Tipo Tasa Edad por Cada Asegurado
                if (codigoTipoTasa == 2)
                {
                    result = opcion.ValorAsegurado;
                }
            }

            return result;
        }

        private decimal CalcularPrimaAnualTotal(decimal sumaPrimaAmparosOpcion)
        {
            return sumaPrimaAmparosOpcion;
        }

        private async Task<AmparoGrupoAsegurado> ObtenerAmparoBasicoNoAdicionalAsync(int codigoRamo, int codigoSubramo, int codigoSector, IEnumerable<AmparoGrupoAsegurado> amparosGrupo)
        {
            var amparos = await this.informacionPersonasReader.TraerAmparosAsync(codigoRamo, codigoSubramo, codigoSector);
            var result = (from amparoGrupo in amparosGrupo
                          join amparo in amparos.Where(x => x.SiNoBasico && !x.SiNoAdicional) on amparoGrupo.CodigoAmparo equals amparo.CodigoAmparo
                          select amparoGrupo).FirstOrDefault();

            return result;
        }

        private async Task<ValorAseguradoOpcionResumen> ProcessOpcionAmparoAsync(InformacionNegocio informacionNegocio, GrupoAsegurado grupo, AmparoGrupoAsegurado amparo, OpcionValorAsegurado o)
        {
            var grupos = this.gruposReader.GetGruposAseguradosAsync(informacionNegocio.CodigoCotizacion).Result;
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
            }
            var valorAseguradoTotalSin = grupos.Sum(x => x.ValorAsegurado);// *grupo.NumeroAsegurados;
            var esAsistencia = amparo.AmparoInfo.CodigoGrupoAmparo == 3;
            var conListaAsegurados = informacionNegocio.ConListaAsegurados;
            var sumatoriaPrimaAmparoOpciones = this.GetSumatoriaPrimaAmparoOpcion(grupo);
            var sumatoriaPrimaSinAplicar = this.GetSumatoriaPrimaAmparoOpcionSinAplicar(grupo);
            decimal porcentajeDescuento = 0;
            decimal porcentajeRecargo = 0;

            var tasaOpcion = await this.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, o.IndiceOpcion);
            if (tasaOpcion != null)
            {
                // throw new Exception($"No se encontraron tasas para el grupo asegurado {grupo.NombreGrupoAsegurado}, codigo {grupo.CodigoGrupoAsegurado}");
                porcentajeDescuento = tasaOpcion.Descuento;
                porcentajeRecargo = tasaOpcion.Recargo;
            }

            // obtener sumatoria primas amparos por opcion
            var primaIndividualAnual = sumatoriaPrimaAmparoOpciones[o.IndiceOpcion - 1];
            // calcula primas por grupo
            // var valorAsistencia = this.GetValorAsistencia(grupo, o.IndiceOpcion);
            var primaAnualIndividual = primaIndividualAnual;
            var primaAnualnoAfecta = sumatoriaPrimaSinAplicar[o.IndiceOpcion - 1];
            var primaTotalAnual = this.CalcularPrimaAnualTotal(primaIndividualAnual);
            var primaTotalAnualNoAfecta = this.CalcularPrimaAnualTotal(primaAnualnoAfecta);

            var valorAseguradoAmparo = this.CalcularValorAseguradoAmparoOpcion(grupo, o, esAsistencia);
            var valorAseguradoTotal = this.CalcularValorAseguradoTotal(grupo, o, conListaAsegurados);
            var tasaComercial = this.CalcularTasaAnual(primaTotalAnualNoAfecta, valorAseguradoTotal);
            var tasaComercialTotal = this.CalcularTasaComercialTotal(tasaComercial, porcentajeDescuento, porcentajeRecargo);

            tasaOpcion.CodigoGrupoAsegurado = grupo.CodigoGrupoAsegurado;
            tasaOpcion.IndiceOpcion = o.IndiceOpcion;
            tasaOpcion.PrimaIndividual = primaAnualIndividual;
            tasaOpcion.PrimaTotal = primaTotalAnual;
            tasaOpcion.TasaComercial = tasaComercial;
            tasaOpcion.TasaComercialTotal = tasaComercialTotal;
            tasaOpcion.SumatoriaTasa = primaIndividualAnual;
            tasaOpcion.TasaSiniestralidad = o.TasaRiesgoSiniestralidad;

            var opcion = new ValorAseguradoOpcionResumen
            {
                IndiceOpcion = o.IndiceOpcion,
                Configurado = valorAseguradoAmparo > 0,
                Amparo = amparo.AmparoInfo,
                TasaRiesgo = o.TasaRiesgo,
                TasaComercialAnual = tasaComercial,
                PorcentajeDescuento = porcentajeDescuento,
                PorcentajeRecargo = porcentajeRecargo,
                TasaComercialAplicar = tasaComercialTotal,
                PrimaAnualIndividual = primaAnualIndividual,
                PrimaAnualTotal = primaTotalAnual,
                ValorAsegurado = valorAseguradoAmparo,
                ValorAseguradoTotal = valorAseguradoTotal,
                PorcentajeDescuentoSiniestralidad = tasaOpcion.DescuentoSiniestralidad,
                PorcentajeRecargoSiniestralidad = tasaOpcion.RecargoSiniestralidad,
                TasaSiniestralidadAplicar = tasaOpcion.TasaSiniestralidadTotal,
            };

            if (this.tieneSiniestralidad)
            {
                var tasaOpcionSiniestralidad = await this.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, 1);
                var firstOption = amparo.OpcionesValores.FirstOrDefault();
                // se obtiene el valor asegurado de la primera opcion para calcular la tasa de reisgo de siniestralidad
                var firstOptionValorAseguradoTotal = this.CalcularValorAseguradoTotal(grupo, firstOption, informacionNegocio.ConListaAsegurados);
                var tasaSiniestralidad = await this.CalcularTasaRiesgoSiniestralidad(valorAseguradoTotalSin);
                tasaOpcion.TasaSiniestralidad = tasaSiniestralidad;
                o.TasaRiesgoSiniestralidad = tasaSiniestralidad;
                tasaOpcion.TasaSiniestralidadTotal = tasaSiniestralidad - (tasaSiniestralidad * tasaOpcion.DescuentoSiniestralidad / 100) + (tasaSiniestralidad * tasaOpcion.RecargoSiniestralidad / 100);

                opcion.Siniestralidad = this.MapOpcionesSiniestralidadResumen(amparo.AmparoInfo, o, grupo.NumeroAsegurados, informacionNegocio.FactorG, tasaOpcionSiniestralidad);
                tasaOpcion.PrimaTotal = opcion.Siniestralidad.PrimaAnualTotal;
            }

            // update informacion tasas opcion
            await this.UpdateTasaOpcionValuesAsync(tasaOpcion);
            return opcion;
        }

        public async Task<PrimasGrupoAsegurado> CalcularPrimasGrupoAseguradoFichaTecnicaAsync(InformacionNegocio informacionNegocio, GrupoAsegurado grupo)
        {
            var factorg = informacionNegocio.FactorG / 100;
            var grupos = this.gruposReader.GetGruposAseguradosAsync(informacionNegocio.CodigoCotizacion).Result;
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
            }
            var valorAseguradoTotalSin = grupos.Sum(x => x.ValorAsegurado);
            var result = new PrimasGrupoAsegurado();
            var opcionesCount = grupo.CodigoTipoSuma == 1 ? 3 : 1;
            var amparo = grupo.AmparosGrupo.Where(a => !a.AmparoInfo.SiNoAdicional && a.AmparoInfo.SiNoBasico).FirstOrDefault();
            // obtener el numero de opciones, se toma la cantidad de opciones del primer amparo            
            var sumatoriaTasaOpciones = this.GetSumatoriaPrimaAmparoOpcion(grupo);
            for (int i = 0; i < opcionesCount; i++)
            {
                var indiceOpcion = i + 1;
                var tasaOpcion = await this.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, indiceOpcion);
                var valorAsistencia = this.GetValorAsistencia(grupo, indiceOpcion, factorg);

                // obtener sumatoria primas amparos por opcion
                var sumPrimaAmparoOpcion = sumatoriaTasaOpciones[i];
                // calcula primas por grupo
                // var primaAnual = tasaOpcion.SumatoriaTasa;
                var primaAnual = sumPrimaAmparoOpcion + valorAsistencia;
                var primaTotalAnual = this.CalcularPrimaAnualTotal(primaAnual);
                if (this.tieneSiniestralidad)
                {
                    var option = amparo.OpcionesValores.FirstOrDefault();
                    // se obtiene el valor asegurado de la primera opcion para calcular la tasa de reisgo de siniestralidad
                    var firstOptionValorAseguradoTotal = this.CalcularValorAseguradoTotal(grupo, option, informacionNegocio.ConListaAsegurados);
                    var tasaSiniestralidad = await this.CalcularTasaRiesgoSiniestralidad(valorAseguradoTotalSin);
                    tasaOpcion.TasaSiniestralidad = tasaSiniestralidad;
                    option.TasaRiesgoSiniestralidad = tasaSiniestralidad;
                    tasaOpcion.TasaSiniestralidadTotal = tasaSiniestralidad - (tasaSiniestralidad * tasaOpcion.DescuentoSiniestralidad / 100) + (tasaSiniestralidad * tasaOpcion.RecargoSiniestralidad / 100);

                    var siniestralidad = this.MapOpcionesSiniestralidadResumen(amparo.AmparoInfo, option, grupo.NumeroAsegurados, informacionNegocio.FactorG, tasaOpcion);

                    primaAnual = 0;
                    primaTotalAnual = siniestralidad.PrimaAnualTotal + valorAsistencia;
                }

                result.PrimaIndividualAnual.Add(new ValorOpcionKeyValue { IndiceOpcion = i + 1, Valor = 0 });
                result.PrimaIndividualTotal.Add(new ValorOpcionKeyValue { IndiceOpcion = i + 1, Valor = 0 });
                result.PrimaTotalAnualxTasa.Add(new ValorOpcionKeyValue { IndiceOpcion = i + 1, Valor = primaAnual });
                result.PrimaTotalAnual.Add(new ValorOpcionKeyValue { IndiceOpcion = i + 1, Valor = primaTotalAnual });


                tasaOpcion.PrimaIndividual = primaTotalAnual;
                tasaOpcion.PrimaTotal = primaTotalAnual;

                this.tasaOpcionWirterService.ActualizarTasaOpcionAsync(tasaOpcion);
            }

            return result;
        }

        public async Task<InformacionSiniestralidad> BuildSiniestralidadDataAsync(int codigoCotizacion, int codigoRamo, IEnumerable<GrupoAseguradoFichaTecnica> gruposAsegurados, bool tieneTasaSiniestralidad)
        {
            var amparoBasicoNoAdicional = gruposAsegurados.FirstOrDefault().Amparos.Where(x => !x.AmparoInfo.SiNoAdicional && x.AmparoInfo.SiNoBasico).FirstOrDefault();
            var valorTotalAsegurado = amparoBasicoNoAdicional.OpcionesValores.FirstOrDefault().ValorAsegurado;
            var numeroAsegurados = gruposAsegurados.FirstOrDefault().NumeroAsegurados;
            return await this.BuildSiniestralidadDataAsync(codigoCotizacion, codigoRamo, gruposAsegurados, tieneTasaSiniestralidad, valorTotalAsegurado, numeroAsegurados);
        }
    }
}

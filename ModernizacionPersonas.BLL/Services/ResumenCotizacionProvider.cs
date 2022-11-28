using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Providers
{
    public class ResumenCotizacionProvider
    {
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosInformacionNegocioWriter informacionNegocioWriter;
        private readonly IDatosTasaOpcionWriter tasaOpcionWriter;
        private readonly IDatosTasaOpcionReader tasaOpcionReader;
        private readonly IDatosCotizacionWriter datosCotizacionWriter;
        private readonly DatosGruposAseguradosProvider gruposAseguradosProvider;
        private readonly CotizacionDataProcessorFactory cotizacionDataProcessorFactory;
        private readonly CotizacionStateWriter cotizacionStateWriter;
        private readonly IDatosAseguradoReader aseguradosReader;
        private readonly DatosGrupoAseguradosUtilities gruposUtilities;
        private readonly CotizacionTransactionsProvider cotizacionTransactionsProvider;

        // private IEnumerable<Tasa> tiposTasa;
        private IEnumerable<TipoSumaAsegurada> tiposSumaAsegurada;

        public ResumenCotizacionProvider()
        {
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.informacionNegocioWriter = new DatosInformacionNegocioTableWriter();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.gruposAseguradosProvider = new DatosGruposAseguradosProvider();
            this.cotizacionDataProcessorFactory = new CotizacionDataProcessorFactory();
            this.datosCotizacionWriter = new DatosCotizacionTableWriter();
            this.cotizacionStateWriter = new CotizacionStateWriter();
            this.tasaOpcionWriter = new DatosTasaOpcionTableWriter();
            this.tasaOpcionReader = new DatosTasaOpcionTableReader();
            this.aseguradosReader = new DatosAseguradoTableReader();
            this.gruposUtilities = new DatosGrupoAseguradosUtilities();
            this.cotizacionTransactionsProvider = new CotizacionTransactionsProvider();

        }

        private async Task<IEnumerable<GrupoAseguradoResumen>> ProcessResumenDataAsync(InformacionNegocio informacionNegocio, IEnumerable<GrupoAsegurado> gruposAsegurados)
        {
            var result = new List<GrupoAseguradoResumen>();
            var codigoTipoTasa1 = informacionNegocio.CodigoTipoTasa1;
            var codigoTipoTasa2 = informacionNegocio.CodigoTipoTasa2;
            var codigoTipoTasa = codigoTipoTasa1 == 5 ? codigoTipoTasa2 : codigoTipoTasa1;
            var tieneSiniestralidad = codigoTipoTasa1 == 5 || codigoTipoTasa2 == 5;
            var valorSalarioMinimo = this.GetSalarioMinimo();
            foreach (var grupo in gruposAsegurados)
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
                var group = await dataProcessor.BuildGrupoAseguradoResumen(informacionNegocio, grupo);


                result.Add(group);
            }

            return result;
        }

        private decimal GetSalarioMinimo()
        {
            decimal result = 0;
            // Obtiene el valor del salario minimo                
            var esTipoSumaAseguradaSalario = this.tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == 10).Count() > 0;
            if (esTipoSumaAseguradaSalario)
            {
                result = this.tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == 10).FirstOrDefault().ValorSalarioMinimo;
            }

            return result;
        }

        private async Task UpdateToResumenStateAsync(int codigoCotizacion, InformacionNegocio informacionNegocio)
        {
            // Actualizar el estado de la cotizacion                
            if (informacionNegocio.CotizacionState < CotizacionState.OnResumen)
            {
                await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.OnResumen);
            }
        }

        public async Task<ProcesarResumenResponse> GenerateAsync(int codigoCotizacion, int version)
        {
            try
            {
                var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                var codigoRamo = informacionNegocio.CodigoRamo;
                var codigoSubramo = informacionNegocio.CodigoSubramo;
                var codigoSector = informacionNegocio.CodigoSector;
                var factorG = informacionNegocio.FactorG;

                this.tiposSumaAsegurada = await this.informacionPersonasReader.TraerTiposSumaAsegurada(codigoRamo, codigoSubramo);

                var tiposTasa = await this.informacionPersonasReader.TraerTasasAsync(codigoRamo, codigoSubramo, codigoSector);
                var tipoTasa = tiposTasa.Where(x => x.CodigoTasa == informacionNegocio.CodigoTipoTasa1).FirstOrDefault();
                var tieneTasaSiniestralidad = informacionNegocio.CodigoTipoTasa1 == 5 || informacionNegocio.CodigoTipoTasa2 == 5;
                var gruposAsegurados = await this.gruposAseguradosProvider.ObtenerGruposAseguradosAsync(codigoCotizacion, codigoRamo, codigoSubramo, informacionNegocio.CodigoSector);

                // procesar resumen
                var gruposAseguradosResumen = await this.ProcessResumenDataAsync(informacionNegocio, gruposAsegurados);
                var porcentajeIvaComision = informacionNegocio.PorcentajeComision * (informacionNegocio.PorcentajeIvaComision / 100);
                var porcentajeIvaRetorno = informacionNegocio.PorcentajeRetorno * (informacionNegocio.PorcentajeIvaRetorno / 100);
                var result = new Resumen
                {
                    Comision = informacionNegocio.PorcentajeComision,
                    IvaComision = porcentajeIvaComision,
                    FactorG = factorG,
                    GastosCompania = informacionNegocio.PorcentajeGastosCompania,
                    GRetorno = informacionNegocio.PorcentajeRetorno,
                    IvaGRetorno = porcentajeIvaRetorno,
                    GruposAsegurados = gruposAseguradosResumen,
                    PorcentajeOtrosGastos = informacionNegocio.PorcentajeOtrosGastos,
                    TieneSiniestralidad = tieneTasaSiniestralidad,
                    TipoTasa = tipoTasa,
                    Utilidad = informacionNegocio.UtilidadCompania,
                    PorcentajeIvaComision = informacionNegocio.PorcentajeIvaComision,
                    PorcentajeIvaRetorno = informacionNegocio.PorcentajeIvaRetorno
                };

                // actualizar a estado OnResumen
                await this.UpdateToResumenStateAsync(codigoCotizacion, informacionNegocio);
                //Calcular tasa general
                decimal sumaPrimas = 0;
                decimal sumavalores = 0;
                var transactions = this.cotizacionTransactionsProvider.GetTransactionsAsync(codigoCotizacion, version).Result.Transactions;
                var fichaAlterna = transactions.Where(x => x.Description == "Ficha Tecnica Alterna").Count() > 0 ? true : false;

                //Se modifica para sumar las primas de seguro , sin al assitencia por indicación de Elizabeth 26-03-2022
                

                foreach (var gr in gruposAseguradosResumen)
                {
                    var primaProcesor = new ValoresPrimasDataProcessor(informacionNegocio.CodigoTipoTasa1, gr.TipoSumaAsegurada.CodigoTipoSumaAsegurada);

                    foreach (var op in gr.Opciones)
                    {
                        if (tieneTasaSiniestralidad) {
                            op.PrimaAnualIndividual = primaProcesor.CalcularPrimaIndividualAnual(op.ValorAsegurado, op.TasaComercialAplicar);
                        }
                        
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
                if (result.TieneSiniestralidad)
                {
                    var tasaSiniestralidad = gruposAseguradosResumen.First().Opciones.First().Siniestralidad.TasaComercial;
                    if (fichaAlterna)
                    {
                        if (tasaGeneral > tasaSiniestralidad)
                        {
                            foreach (var gr in gruposAseguradosResumen)
                            {
                                foreach (var op in gr.Opciones)
                                {
                                    op.TasaComercialAnual = 0;
                                    op.TasaComercialAplicar = 0;
                                    op.PrimaAnualIndividual = 0;
                                    op.PrimaAnualTotal = 0;
                                }
                            }
                        }
                        else
                        {
                            result.TieneSiniestralidad = false;
                        }
                    }
                    else
                    {
                        if (tasaGeneral > tasaSiniestralidad)
                        {
                            result.TieneSiniestralidad = false;
                            foreach (var gr in gruposAseguradosResumen)
                            {
                                var primaProcesor = new ValoresPrimasDataProcessor(informacionNegocio.CodigoTipoTasa1, gr.TipoSumaAsegurada.CodigoTipoSumaAsegurada);

                                foreach (var op in gr.Opciones)
                                {
                                    op.PrimaAnualIndividual = primaProcesor.CalcularPrimaIndividualAnual(op.ValorAsegurado, op.TasaComercialAplicar);

                                }
                            }
                            
                               

                        }
                        else
                        {
                            foreach (var gr in gruposAseguradosResumen)
                            {
                                foreach (var op in gr.Opciones)
                                {
                                    op.TasaComercialAnual = 0;
                                    op.TasaComercialAplicar = 0;
                                    op.PrimaAnualIndividual = 0;
                                    op.PrimaAnualTotal = 0;
                                }
                            }
                        }
                    }
                }


                return new ProcesarResumenResponse
                {
                    Data = result,
                    CodigoCotizacion = codigoCotizacion,
                    CodigoEstadoCotizacion = informacionNegocio.CodigoEstadoCotizacion,
                    NumeroCotizacion = informacionNegocio.NumeroCotizacion
                };
            }
            catch (Exception ex)
            {
                // throw new Exception("Hubo un error generando el resumen de la cotización.");
                throw new Exception("ResumenCotizacionDataMapper :: GenerateAsync", ex);
            }
        }

        public async Task<bool> InsertarTasaOpcionAsync(int codigoCotizacion, int version, GuardarResumenArgs1 model)
        {
            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            //var nInformacionNegocio = new InformacionNegocio
            //{
            //    CodigoCotizacion = codigoCotizacion,
            //    PorcentajeGastosCompania = model.GastosCompania,
            //    UtilidadCompania = model.UtilidadCompania,
            //    PorcentajeRetorno = model.PorcentajeRetorno,
            //    PorcentajeOtrosGastos = model.PorcentajeOtrosGastos,
            //    PorcentajeComision = model.PorcentajeComision,
            //    FactorG = model.FactorG
            //};

            var nInformacionNegocio1 = new InformacionNegocio
            {
                CodigoCotizacion = codigoCotizacion,
                PorcentajeComision = model.PorcentajeComision,
                PorcentajeIvaComision = model.PorcentajeIvaComision,
                PorcentajeRetorno = model.PorcentajeRetorno,
                PorcentajeIvaRetorno = model.PorcentajeIvaRetorno,
                PorcentajeOtrosGastos = model.PorcentajeOtrosGastos,
                PorcentajeGastosCompania = model.GastosCompania,
                UtilidadCompania = model.UtilidadCompania,
                FactorG = model.FactorG,
                CodigoEstadoCotizacion = informacionNegocio.CodigoEstadoCotizacion,
                CodigoSector = informacionNegocio.CodigoSector,
                CodigoPerfilEdad = informacionNegocio.CodigoPerfilEdad,
                CodigoPerfilValor = informacionNegocio.CodigoPerfilValor,
                CodigoPeriodoFacturacion = informacionNegocio.CodigoPeriodoFacturacion,
                CodigoRamo = informacionNegocio.CodigoRamo,
                CodigoSubramo = informacionNegocio.CodigoSubramo,
                CodigoSucursal = informacionNegocio.CodigoSucursal,
                CodigoTipoContratacion = informacionNegocio.CodigoTipoContratacion,
                CodigoTipoNegocio = informacionNegocio.CodigoTipoNegocio,
                CodigoTipoRiesgo = informacionNegocio.CodigoTipoRiesgo,
                CodigoTipoTasa1 = informacionNegocio.CodigoTipoTasa1,
                CodigoTipoTasa2 = informacionNegocio.CodigoTipoTasa2,
                CodigoZona = informacionNegocio.CodigoZona,
                ConListaAsegurados = informacionNegocio.ConListaAsegurados,
                EmailDirectorComercial = informacionNegocio.EmailDirectorComercial,
                EsNegocioDirecto = informacionNegocio.EsNegocioDirecto,
                FechaInicio = informacionNegocio.FechaInicio,
                FechaFin = informacionNegocio.FechaFin,
                IBNR = informacionNegocio.IBNR,
                LastAuthorId = informacionNegocio.LastAuthorId,
                LastAuthorName = informacionNegocio.LastAuthorName,
                NombreAseguradora = informacionNegocio.NombreAseguradora,
                NombreDirectorComercial = informacionNegocio.NombreDirectorComercial,
                NumeroCotizacion = informacionNegocio.NumeroCotizacion,
                UsuarioDirectorComercial = informacionNegocio.UsuarioDirectorComercial,
                UsuarioNotificado = informacionNegocio.UsuarioNotificado,
                Version = informacionNegocio.Version,
                VersionCopia = informacionNegocio.VersionCopia,
                anyosSiniestralidad = informacionNegocio.anyosSiniestralidad,
                Bloqueado = informacionNegocio.Bloqueado,
                BloqueadoBy = informacionNegocio.BloqueadoBy,
                Actividad = informacionNegocio.Actividad
            };

            try
            {
                var isEqual = nInformacionNegocio1.PorcentajeGastosCompania == informacionNegocio.PorcentajeGastosCompania;
                if (isEqual)
                    isEqual = nInformacionNegocio1.UtilidadCompania == informacionNegocio.UtilidadCompania;
                if (isEqual)
                    isEqual = nInformacionNegocio1.PorcentajeRetorno == informacionNegocio.PorcentajeRetorno;
                if (isEqual)
                    isEqual = nInformacionNegocio1.PorcentajeOtrosGastos == informacionNegocio.PorcentajeOtrosGastos;
                if (isEqual)
                    isEqual = nInformacionNegocio1.PorcentajeComision == informacionNegocio.PorcentajeComision;
                if (isEqual)
                    isEqual = nInformacionNegocio1.FactorG == informacionNegocio.FactorG;


                if (!isEqual)
                {
                    await this.informacionNegocioWriter.ActualizarInformacionNegocioAsync(codigoCotizacion, nInformacionNegocio1);
                    // update cotizacion modified flag to true
                    await this.datosCotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, true);
                    await this.informacionNegocioWriter.UpdateSelfAuthorizeFlagASync(codigoCotizacion, false);
                }

                foreach (var item in model.TasaOpciones)
                {
                    var tasaOpciones = await this.tasaOpcionReader.LeerTasaOpcionAsync(item.CodigoGrupoAsegurado, item.IndiceOpcion);
                    isEqual = item.Descuento == tasaOpciones.Descuento;
                    if (isEqual)
                        isEqual = item.Recargo == tasaOpciones.Recargo;
                    if (isEqual)
                        isEqual = item.DescuentoSiniestralidad == tasaOpciones.DescuentoSiniestralidad;
                    if (isEqual)
                        isEqual = item.RecargoSiniestralidad == tasaOpciones.RecargoSiniestralidad;

                    await this.tasaOpcionWriter.ActualizarTasaOpcionAsync(item);

                    // Update modified flag after pass authorization controls verificar si está en true
                    informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                    if (isEqual && informacionNegocio.CotizacionChanged != true)
                    {
                        await this.datosCotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, false);
                    }
                    else
                    {
                        // update cotizacion modified flag to true
                        await this.datosCotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, true);
                        await this.informacionNegocioWriter.UpdateSelfAuthorizeFlagASync(codigoCotizacion, false);
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("ResumenCotizacionWriter :: InsertarTasaOpcionAsync", ex);
            }
        }
    }

    public class ProcesarResumenResponse : ActionResponseBase
    {
        public Resumen Data { get; set; }
    }
}

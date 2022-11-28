using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.SISEServices;
using ModernizacionPersonas.Entities;
using PersonasServiceReference;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.SISEServices
{
    public class SISEAuthorizationDataProvider
    {
        private readonly SISEAutorizacionesWriter siseAutorizacionesWriter;
        private readonly SISEAutorizacionesReader siseAutorizacionesReader;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosGruposAseguradoReader gruposAseguradosReader;
        private readonly IDatosAmparoGrupoAseguradoReader amparosReader;
        private readonly IDatosTasaOpcionReader tasaOpcionesReader;
        private readonly IDatosEdadesReader edadesReader;
        private readonly IDatosOpcionValorAseguradoReader opcionValoresReader;
        private readonly IAuthorizationsDataReader authorizationsDataReader;
        private readonly IDatosCotizacionWriter cotizacionWriter;

        private IEnumerable<Amparo> amparos;

        public SISEAuthorizationDataProvider()
        {
            this.siseAutorizacionesWriter = new SISEAutorizacionesWriter();
            this.siseAutorizacionesReader = new SISEAutorizacionesReader();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.gruposAseguradosReader = new DatosGruposAseguradosTableReader();
            this.amparosReader = new DatosAmparoGrupoAseguradoTableReader();
            this.tasaOpcionesReader = new DatosTasaOpcionTableReader();
            this.edadesReader = new DatosEdadesTableReader();
            this.opcionValoresReader = new DatosOpcionValorAseguradoTableReader();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.authorizationsDataReader = new AuthorizationsDataTableReader();
            this.cotizacionWriter = new DatosCotizacionTableWriter();
        }

        public async Task InsertAuthorizationData(string userName, int codigoCotizacion, int version)
        {
            var informacionNegocio = await informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var numCot = int.Parse(informacionNegocio.NumeroCotizacion);
            // INSERCION AUTORIZACIONES EN SISE
            var listaAutorizaciones = new List<CotizacionAuthorization>();
            var grupoAsegurado = new GrupoAsegurado();
            

            var autorizacionesOld = this.authorizationsDataReader.GetAuthorizationsByCotizacionAsync(numCot, informacionNegocio.Version).Result.Authorizations;

            await siseAutorizacionesWriter.RemoverAutorizacionesAsync(numCot, version);

            decimal primaMax = 0;


            

            // Autorización para porcentaje combinado            
            var AutPorcentajeIndiceCombinado = new CotizacionAuthorization
            {
                CodigoCotizacion = numCot,
                CampoEntrada = "porc_ind_combinado",
                CodigoRamo = informacionNegocio.CodigoRamo,
                CodigoSubramo = informacionNegocio.CodigoSubramo,
                CodigoSucursal = informacionNegocio.CodigoSucursal,
                CodigoTipoAutorizacion = 1,
                Version = version,
                CodigoUsuario = userName,
                ValorEntrada = (decimal)informacionNegocio.PorcentajeComision + (decimal)informacionNegocio.PorcentajeRetorno + (decimal)informacionNegocio.PorcentajeOtrosGastos
            };

            listaAutorizaciones.Add(AutPorcentajeIndiceCombinado);

            // Autorización para tipo riesgo            
            var AutTipoRiesgo = new CotizacionAuthorization
            {
                CodigoCotizacion = numCot,
                CampoEntrada = "tipo_riesgo",
                CodigoRamo = informacionNegocio.CodigoRamo,
                CodigoSubramo = informacionNegocio.CodigoSubramo,
                CodigoSucursal = informacionNegocio.CodigoSucursal,
                CodigoTipoAutorizacion = 2,
                Version = version, // TODO: Obtener la versión
                CodigoUsuario = userName, // TODO: Obtener el usaurio desde el Login
                ValorEntrada = informacionNegocio.CodigoTipoRiesgo
            };

            listaAutorizaciones.Add(AutTipoRiesgo);



            var grupos = await this.gruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);
            var valoresPrimaTotal = new List<decimal>(); // Dimensión de valores prima
            var valoresDescuentoTotal = new List<decimal>(); // Dimensión de valores descuento
            foreach (var grupo in grupos)
            {
                decimal[] valoresPrima = new decimal[3]; // Dimensión de valores prima
                decimal[] valoresDescuento = new decimal[3]; // Dimensión de valores descuento
                var codigoGrupo = grupo.CodigoGrupoAsegurado;
                grupoAsegurado = await this.gruposAseguradosReader.GetGrupoAseguradoAsync(codigoGrupo);
                //var tasaOpcionIn = new TasaOpcionViewModel();
                //tasaOpcionIn.CodigoGrupoAsegurado = grupo.CodigoGrupoAsegurado;
                // TODO implementacion anterior no enviaba indiceOpcion
                var tasaOpcion = await this.tasaOpcionesReader.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, 1);
                valoresDescuento[0] = tasaOpcion.Descuento;
                valoresPrima[0] = tasaOpcion.PrimaTotal;
                var tasaOpcion2 = await this.tasaOpcionesReader.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, 2);
                valoresDescuento[1] = tasaOpcion2.Descuento;
                valoresPrima[1] = tasaOpcion2.PrimaTotal;
                var tasaOpcion3 = await this.tasaOpcionesReader.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, 3);
                valoresDescuento[2] = tasaOpcion3.Descuento;
                valoresPrima[2] = tasaOpcion3.PrimaTotal;




                var countGru = 0;

                // Autorización para porcentaje por Descuento            
                var AutPorcDescuento = new CotizacionAuthorization
                {
                    CodigoCotizacion = numCot,
                    CodigoGrupoAsegurado = grupo.CodigoGrupoAsegurado,
                    CampoEntrada = "porc_descuento",
                    CodigoRamo = informacionNegocio.CodigoRamo,
                    CodigoSubramo = informacionNegocio.CodigoSubramo,
                    CodigoSucursal = informacionNegocio.CodigoSucursal,
                    CodigoTipoAutorizacion = 2,
                    Version = version, // TODO: Obtener la versión
                    CodigoUsuario = userName, // TODO: Obtener el usaurio desde el Login
                    ValorEntrada = valoresDescuento.Max()
                };
                listaAutorizaciones.Add(AutPorcDescuento);
                // decimal valorAsegMax = 0; // Variable para almacenar el valor Max de las opciones del Amparo Básico
                this.amparos = await informacionPersonasReader.TraerAmparosAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector);

                var amparosGrupo = await amparosReader.LeerAmparoGrupoAseguradoAsync(codigoGrupo);
                decimal[] valoresAmparo = new decimal[3]; // Dimensión de valores para 3 opciones
                foreach (var amparo in amparosGrupo)
                {
                    var amparoActual = amparos.Where(x => x.CodigoAmparo == amparo.CodigoAmparo).FirstOrDefault(); // Se busca en el objeto de Amapros de Personas la info del amparo del recorrido
                    var informacionEdadesAmparo = await this.edadesReader.LeerEdadesAsync(amparo.CodigoGrupoAsegurado, amparo.CodigoAmparo);
                    var opcionValores = await this.opcionValoresReader.LeerOpcionValorAseguradoAsync(amparo.CodigoAmparoGrupoAsegurado);
                    if (grupo.ConDistribucionAsegurados)
                    {
                        grupo.AseguradosOpcion1 = opcionValores.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                        grupo.AseguradosOpcion2 = opcionValores.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                        grupo.AseguradosOpcion3 = opcionValores.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                    }
                    


                    var count = 0; // Contador opciones

                    foreach (var opcion in opcionValores)
                    {
                        var esAmparoBasicoNoAdicional = amparoActual.SiNoBasico && !amparoActual.SiNoAdicional; // Se alamcena la validación del amparo básico no adicional
                        amparo.OpcionesValores.Add(opcion);
                        // Valor Max Asegurado Grupo
                        if (grupo.CodigoTipoSuma == 1)
                        {
                            if (esAmparoBasicoNoAdicional == true)
                            {
                                valoresAmparo[count] = opcion.ValorAsegurado;
                            }
                        }
                        else
                        {
                            valoresAmparo[count] = grupo.ValorMaxAsegurado;
                        }


                        amparo.OpcionesValores.Add(opcion);
                        if (amparo.CodigoAmparo == 95 && opcion.Prima > 0)
                        {
                            // Autorización para Asistencia           
                            var AutAsistencia = new CotizacionAuthorization
                            {
                                CodigoCotizacion = numCot,
                                CodigoGrupoAsegurado = grupo.CodigoGrupoAsegurado,
                                CampoEntrada = "amparo_asistencia",
                                CodigoAmparo = amparo.CodigoAmparo,
                                CodigoRamo = informacionNegocio.CodigoRamo,
                                CodigoSubramo = informacionNegocio.CodigoSubramo,
                                CodigoSucursal = informacionNegocio.CodigoSucursal,
                                CodigoTipoAutorizacion = 2,
                                Version = version, // TODO: Obtener la versión
                                CodigoUsuario = userName, // TODO: Obtener el usaurio desde el Login
                                ValorEntrada = (decimal)opcion.Prima
                            };
                            listaAutorizaciones.Add(AutAsistencia);
                        }
                        count += 1; // Se aumenta el contador.
                    }




                    countGru += 1; // Se aumenta el contador.


                    // Autorización para Edad Mínima           
                    var AutEdadMin = new CotizacionAuthorization
                    {
                        CodigoCotizacion = numCot,
                        CodigoGrupoAsegurado = grupo.CodigoGrupoAsegurado,
                        CampoEntrada = "edad_minima",
                        CodigoAmparo = amparo.CodigoAmparo,
                        CodigoRamo = informacionNegocio.CodigoRamo,
                        CodigoSubramo = informacionNegocio.CodigoSubramo,
                        CodigoSucursal = informacionNegocio.CodigoSucursal,
                        CodigoTipoAutorizacion = 2,
                        Version = version, // TODO: Obtener la versión
                        CodigoUsuario = userName, // TODO: Obtener el usaurio desde el Login
                        ValorEntrada = informacionEdadesAmparo.EdadMinAsegurado
                    };
                    listaAutorizaciones.Add(AutEdadMin);

                    // Autorización para Edad Máxima           
                    var AutEdadMax = new CotizacionAuthorization
                    {
                        CodigoCotizacion = numCot,
                        CodigoGrupoAsegurado = grupo.CodigoGrupoAsegurado,
                        CampoEntrada = "edad_maxima",
                        CodigoAmparo = amparo.CodigoAmparo,
                        CodigoRamo = informacionNegocio.CodigoRamo,
                        CodigoSubramo = informacionNegocio.CodigoSubramo,
                        CodigoSucursal = informacionNegocio.CodigoSucursal,
                        CodigoTipoAutorizacion = 2,
                        Version = version, // TODO: Obtener la versión
                        CodigoUsuario = userName, // TODO: Obtener el usaurio desde el Login
                        ValorEntrada = (decimal)informacionEdadesAmparo.EdadMaxAsegurado
                    };
                    listaAutorizaciones.Add(AutEdadMax);

                    // Autorización para Edad Máxima Permanencia          
                    var AutEdadMaxPerm = new CotizacionAuthorization
                    {
                        CodigoCotizacion = numCot,
                        CodigoGrupoAsegurado = grupo.CodigoGrupoAsegurado,
                        CampoEntrada = "edad_max_permanencia",
                        CodigoAmparo = amparo.CodigoAmparo,
                        CodigoRamo = informacionNegocio.CodigoRamo,
                        CodigoSubramo = informacionNegocio.CodigoSubramo,
                        CodigoSucursal = informacionNegocio.CodigoSucursal,
                        CodigoTipoAutorizacion = 2,
                        Version = version, // TODO: Obtener la versión
                        CodigoUsuario = userName, // TODO: Obtener el usaurio desde el Login
                        ValorEntrada = (decimal)informacionEdadesAmparo.edadMaxPermanencia
                    };
                    listaAutorizaciones.Add(AutEdadMaxPerm);

                    amparo.EdadesGrupo = informacionEdadesAmparo;
                    grupoAsegurado.AmparosGrupo.Add(amparo);
                }


                // Autorización para suma asegurada    
                var valorAsegAuth = valoresAmparo.Max();
                var AutSumaAsegurada = new CotizacionAuthorization
                {
                    CodigoCotizacion = numCot,
                    CodigoGrupoAsegurado = grupo.CodigoGrupoAsegurado,
                    CampoEntrada = "suma_asegurada",
                    CodigoRamo = informacionNegocio.CodigoRamo,
                    CodigoSubramo = informacionNegocio.CodigoSubramo,
                    CodigoSucursal = informacionNegocio.CodigoSucursal,
                    CodigoTipoAutorizacion = 2,
                    Version = version, // TODO: Obtener la versión
                    CodigoUsuario = userName, // TODO: Obtener el usuario desde el Login
                    ValorEntrada = (decimal)valorAsegAuth
                };
                listaAutorizaciones.Add(AutSumaAsegurada);

                if (grupoAsegurado.ConDistribucionAsegurados)
                {
                    primaMax = valoresPrima.Sum(); // Sumatoria de valores de la distribución.
                }
                else
                {
                    primaMax = valoresPrima.Max(); // Valor máximo de primas
                }
                

                valoresPrimaTotal.Add(primaMax);

            }


            var valorPrimaAuth = valoresPrimaTotal.Sum();

            // Autorización para prima  asegurada MAx      
            var AutSumaAseguradaMax = new CotizacionAuthorization
            {
                CodigoCotizacion = numCot,
                CampoEntrada = "prima_max_grupo_aseg",
                CodigoRamo = informacionNegocio.CodigoRamo,
                CodigoSubramo = informacionNegocio.CodigoSubramo,
                CodigoSucursal = informacionNegocio.CodigoSucursal,
                CodigoTipoAutorizacion = 2,
                Version = version, // TODO: Obtener la versión
                CodigoUsuario = userName, // TODO: Obtener el usaurio desde el Login
                ValorEntrada = valorPrimaAuth
            };

            listaAutorizaciones.Add(AutSumaAseguradaMax);

            informacionNegocio = await informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var changed = informacionNegocio.CotizacionChanged ? informacionNegocio.CotizacionChanged : false;

            foreach(var autOld in autorizacionesOld)
            {
                foreach(var aut in listaAutorizaciones)
                {
                    if(aut.CodigoAmparo != 95)
                    {
                        if (aut.CampoEntrada == autOld.CampoEntrada && aut.CodigoGrupoAsegurado == autOld.CodigoGrupoAsegurado && aut.CodigoAmparo == autOld.CodigoAmparo)
                        {
                            changed = aut.ValorEntrada != autOld.ValorEntrada ? true : changed == true ? true : false;
                        }
                    }                    
                }
            }

            var assOld = autorizacionesOld.Where(x => x.CodigoAmparo == 95);
            var ass = listaAutorizaciones.Where(x => x.CodigoAmparo == 95);

            var sumOldass = assOld.Sum(x => x.ValorEntrada);
            var sumAss = ass.Sum(x => x.ValorEntrada);

            changed = assOld.Sum(x => x.ValorEntrada) != ass.Sum(x => x.ValorEntrada) ? true : changed == true ? true : false;
           

            await this.cotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, changed);

            await siseAutorizacionesWriter.InsertarAutorizacionesAsync(codigoCotizacion, listaAutorizaciones);
        }

        public async Task<IEnumerable<AuthorizationUser>> GetWebAuthorizationUsersAsync(int codigoCotizacion, int version)
        {
            return await this.siseAutorizacionesReader.LeerUsersWEBAsync(codigoCotizacion, version);
        }

        public async Task<IEnumerable<AuthorizationUser>> GetEspAuthorizationUsersAsync(int codigoCotizacion, int version)
        {
            return await this.siseAutorizacionesReader.LeerUsersSpecialAsync(codigoCotizacion, version);
        }
    }
}

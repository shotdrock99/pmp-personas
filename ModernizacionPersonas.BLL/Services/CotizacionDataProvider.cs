using Microsoft.AspNetCore.Http;
using ModernizacionPersonas.Api.Providers;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Utilities;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.DAL;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Soligespro;
using ModernizacionPersonas.Entities;
using Newtonsoft.Json;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class CotizacionDataProvider
    {
        private readonly IDatosCotizacionWriter cotizacionWriterService;
        private readonly IDatosCotizacionReader datosCotizacionReader;
        private readonly IDatosTomadorReader datosTomadorReader;
        private readonly IDatosCausalReader causalesConfirmacionReader;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosInformacionNegocioWriter informacionNegocioWriter;
        private readonly IDatosPersonasReader datosPersonasReader;
        private readonly IDatosGruposAseguradoReader datosGruposAseguradosReader;
        private readonly IDatosGrupoAseguradoWriter datosGruposAseguradoWriter;
        private readonly IDatosIntermediarioReader datosIntermediarioReader;
        private readonly IDatosIntermediarioWriter datosIntermediarioWriter;
        private readonly IDatosSiniestralidadReader datosSiniestralidadReader;
        private readonly IDatosSiniestralidadWriter datosSiniestralidadWriter;
        private readonly IDatosOpcionValorAseguradoReader opcionValorReader;
        private readonly IDatosOpcionValorAseguradoWriter opcionValorWriter;
        private readonly IDatosEdadesReader edadesReader;
        private readonly IDatosEdadesWriter edadesWriter;
        private readonly IDatosAmparoGrupoAseguradoReader amparoGrupoAsguradoReader;
        private readonly IDatosAmparoGrupoAseguradoWriter amparoGrupoAsguradoWriter;
        private readonly DatosParametrizacionReader parametrizacionReader;
        private readonly IDatosBloqueoWriter bloqueoCotizacionWriter;
        private readonly CotizacionTransactionsProvider cotizacionTransactionsProvider;
        private readonly IDatosTasaOpcionReader tasaOpcionesReader;
        private readonly IDatosTasaOpcionWriter tasaOpcionesWriter;
        private readonly IDatosAseguradoReader aseguradoReader;
        private readonly IDatosAseguradoWriter aseguradoWriter;
        private readonly CotizacionStateWriter cotizacionStateUpdater;
        private readonly DatosCotizacionUtilities datosCotizacionUtilities;
        private readonly ISoligesproDataUsuariosReader soligesproUsersDataReader;
        private readonly CotizationEmailSender cotizationEmailSender;
        private readonly ApplicationUserDataProvider applicationUserDataProvider;
        private readonly DatosGruposAseguradosProvider datosGruposAseguradosProvider;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosTomadorReader tomadorReader;
        private readonly IDatosIntermediarioReader intermediarioReader;
        private readonly ResumenCotizacionProvider resumenProvider;
        private readonly IDatosEnvioSlipWriter datosEnvioSlipWriter;

        public CotizacionDataProvider()
        {
            this.cotizacionWriterService = new DatosCotizacionTableWriter();
            this.datosEnvioSlipWriter = new DatosEnvioSlipTableWriter();
            this.causalesConfirmacionReader = new DatosCausalTableReader();
            this.datosTomadorReader = new DatosTomadorTableReader();
            this.datosCotizacionReader = new DatosCotizacionTableReader();
            this.datosPersonasReader = new InformacionPersonasReader();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.informacionNegocioWriter = new DatosInformacionNegocioTableWriter();
            this.datosGruposAseguradosReader = new DatosGruposAseguradosTableReader();
            this.datosGruposAseguradoWriter = new DatosGrupoAseguradoTableWriter();
            this.datosIntermediarioReader = new DatosIntermediarioTableReader();
            this.datosIntermediarioWriter = new DatosIntermediarioTableWriter();
            this.opcionValorReader = new DatosOpcionValorAseguradoTableReader();
            this.opcionValorWriter = new DatosOpcionValorAseguradoTableWriter();
            this.edadesReader = new DatosEdadesTableReader();
            this.edadesWriter = new DatosEdadesTableWriter();
            this.amparoGrupoAsguradoReader = new DatosAmparoGrupoAseguradoTableReader();
            this.amparoGrupoAsguradoWriter = new DatosAmparoGrupoAseguradoTableWriter();
            this.datosSiniestralidadReader = new DatosSiniestralidadTableReader();
            this.datosSiniestralidadWriter = new DatosSiniestralidadTableWriter();
            this.parametrizacionReader = new DatosParametrizacionReader();
            this.bloqueoCotizacionWriter = new DatosBloqueoTableWriter();
            this.cotizacionTransactionsProvider = new CotizacionTransactionsProvider();
            this.tasaOpcionesReader = new DatosTasaOpcionTableReader();
            this.tasaOpcionesWriter = new DatosTasaOpcionTableWriter();
            this.aseguradoReader = new DatosAseguradoTableReader();
            this.aseguradoWriter = new DatosAseguradoTableWriter();
            this.cotizacionStateUpdater = new CotizacionStateWriter();
            this.soligesproUsersDataReader = new SoligesproDataUsuariosReader();
            this.cotizationEmailSender = new CotizationEmailSender();
            this.applicationUserDataProvider = new ApplicationUserDataProvider();
            this.datosGruposAseguradosProvider = new DatosGruposAseguradosProvider();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.tomadorReader = new DatosTomadorTableReader();
            this.intermediarioReader = new DatosIntermediarioTableReader();
            this.resumenProvider = new ResumenCotizacionProvider();

            this.datosCotizacionUtilities = new DatosCotizacionUtilities(this.datosPersonasReader);
        }

        public async Task<IEnumerable<CotizacionItemList>> GetCotizacionesAsync(string userName = null)
        {
            try
            {
                var data = await this.datosCotizacionReader.GetCotizacionesAsync(userName);
                await this.datosCotizacionUtilities.AgreggateCotizacionModelAsync(data);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("CotizacionDataProvider :: GetCotizacionesAsync", ex);
            }
        }

        public async Task<IEnumerable<CotizacionItemList>> GetCotizacionesAsync(CotizacionFilter filterArgs)
        {
            try
            {
                IEnumerable<CotizacionItemList> data = Enumerable.Empty<CotizacionItemList>();
                if (filterArgs.CodigoEstado <= 1106)
                {
                    List<CotizacionItemList> dataList = new List<CotizacionItemList>();
                    for (int i = 1000; i <= 1106; i++)
                    {
                        filterArgs.CodigoEstado = i;
                        dataList.AddRange(this.datosCotizacionReader.GetCotizacionesAsync(filterArgs).GetAwaiter().GetResult());
                    }
                    data = dataList;
                }
                else
                {
                    data = await this.datosCotizacionReader.GetCotizacionesAsync(filterArgs);
                }
                await this.datosCotizacionUtilities.AgreggateCotizacionModelAsync(data);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("CotizacionDataProvider :: GetCotizacionesAsync", ex);
            }
        }

        public async Task<IEnumerable<VersionCotizacion>> GetVersionesCotizacionAsync(int codigoCotizacion)
        {
            try
            {
                var info = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                var transactions = this.cotizacionTransactionsProvider.GetTransactionsAsync(codigoCotizacion, info.Version).Result;
                var firstTransaction = transactions.Transactions.FirstOrDefault();

                var response = await this.datosCotizacionReader.GetVersionesCotizacionAsync(codigoCotizacion);
                if (firstTransaction.Description == "COPIADO" && info.Version == 1)
                {
                    response = await this.datosCotizacionReader.GetVersionesCotizacionQueryAsync(codigoCotizacion);
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("CotizacionDataProvider :: GetVersionesCotizacionAsync", ex);
            }
        }
        
        public async Task<ActionResponseBase> SaveAttachmentsAuthAsync(int codigoCotizacion, IFormFile file)
        {
            try
            {
                using (var s = file.OpenReadStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    s.CopyTo(ms);
                    ms.Position = 0;
                    var array = ms.ToArray();
                    
                    string ext = System.IO.Path.GetExtension(file.FileName);
                    string nameFileTemp = System.IO.Path.GetFileNameWithoutExtension(file.FileName);

                    var nameFile = nameFileTemp + "248699" + ext;

                    var response = await this.datosEnvioSlipWriter.CrearAdjuntoEnvioSlipAsync(codigoCotizacion, array, nameFile);
                    return new CreateActionResponse
                    {
                        CodigoCotizacion = codigoCotizacion,
                        Codigo = response
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SlipDataProvider :: SaveAttachmentsAuthAsync", ex);
            }
        }

        public async Task<IEnumerable<VersionCotizacion>> GetVersionesCotizacionQueryAsync(int codigoCotizacion)
        {
            try
            {
                var response = await this.datosCotizacionReader.GetVersionesCotizacionQueryAsync(codigoCotizacion);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("CotizacionDataProvider :: GetVersionesCotizacionQueryAsync", ex);
            }
        }

        public async Task<ActionResponseBase> InitializeCotizacionAsync(CrearCotizacionArgs model)
        {
            var args = new InicializarCotizacionArgs
            {
                UserId = model.UserId,
                CodigoSucursal = model.CodigoSucursal,
                CodigoRamo = model.CodigoRamo,
                CodigoSubRamo = model.CodigoSubRamo,
                CodigoZona = model.CodigoZona
            };

            // Guardar cotizacion
            var response = await this.cotizacionWriterService.CrearCotizacionAsync(args);

            var message = "Creación de la cotización";
            await this.cotizacionTransactionsProvider.CreateTransactionAsync(response.CodigoCotizacion, 1, model.UserName, message);

            return new ActionResponseBase
            {
                CodigoCotizacion = response.CodigoCotizacion,
                CodigoEstadoCotizacion = response.CodigoEstadoCotizacion,
                NumeroCotizacion = response.NumeroCotizacion,
                Version = response.Version
            };
        }

        public async Task<ActionResponseBase> CopyCotizacionAsync(int userId, int codigoCotizacion, int version, string username)
        {
            var response = await this.cotizacionWriterService.CopiarCotizacionAsync(userId, codigoCotizacion, version, username);
            var gruposAsegurados = await this.datosGruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);
            var siniestros = await this.datosSiniestralidadReader.GetSiniestralidadAsync(codigoCotizacion);
            var intermediarios = await this.datosIntermediarioReader.GetIntermediariosAsync(codigoCotizacion);

            foreach (var siniestralidad in siniestros)
            {
                siniestralidad.CodigoCotizacion = response.CodigoCotizacion;
                var responseInsSinietralidad = this.datosSiniestralidadWriter.CrearSiniestralidadAsync(siniestralidad);
            }

            foreach (var intermediario in intermediarios)
            {
                intermediario.CodigoCotizacion = response.CodigoCotizacion;
                var responseInsIntermediario = this.datosIntermediarioWriter.CreateIntermediarioAsync(response.CodigoCotizacion, intermediario);
            }

            foreach (var ga in gruposAsegurados)
            {
                ga.CodigoCotizacion = response.CodigoCotizacion;
                var codigoGrupoAsegurado = await this.datosGruposAseguradoWriter.CrearGrupoAseguradoAsync(ga);
                var grupoAsegCod = codigoGrupoAsegurado;
                await this.datosGruposAseguradoWriter.ActualizarGrupoAseguradoAsync(grupoAsegCod, ga);
                await this.datosGruposAseguradoWriter.InsertarNumAseguradosAsync(grupoAsegCod, ga.NumeroAsegurados, ga.EdadPromedioAsegurados, (int)ga.PorcentajeAsegurados);
                var amparos = await this.amparoGrupoAsguradoReader.LeerAmparoGrupoAseguradoAsync(ga.CodigoGrupoAsegurado);
                foreach (AmparoGrupoAsegurado amparo in amparos)
                {
                    amparo.CodigoGrupoAsegurado = grupoAsegCod;
                    var codigoAmparo = await this.amparoGrupoAsguradoWriter.CrearAmparoGrupoAseguradoAsync(amparo);
                    var opciones = await this.opcionValorReader.LeerOpcionValorAseguradoAsync(amparo.CodigoAmparoGrupoAsegurado);
                    foreach (var opcion in opciones)
                    {
                        if (ga.ConDistribucionAsegurados)
                        {
                            ga.AseguradosOpcion1 = opciones.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                            ga.AseguradosOpcion2 = opciones.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                            ga.AseguradosOpcion3 = opciones.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                        }
                        opcion.CodigoAmparoGrupoAsegurado = codigoAmparo;
                        var codigoOpcionValor = await this.opcionValorWriter.CrearOpcionValorAseguradoAsync(opcion);
                        var tasaOpcion = await this.tasaOpcionesReader.LeerTasaOpcionAsync(ga.CodigoGrupoAsegurado, opcion.IndiceOpcion);
                        await this.tasaOpcionesWriter.CrearTasaOpcionAsync(tasaOpcion);
                    }

                    var edadesAmparo = await this.edadesReader.LeerEdadesAsync(ga.CodigoGrupoAsegurado, amparo.CodigoAmparo);
                    await this.edadesWriter.InsertarEdadAmparoAsync(grupoAsegCod, amparo.CodigoAmparo, edadesAmparo);
                }

                var asegurados = await this.aseguradoReader.LeerAseguradosAsync(ga.CodigoGrupoAsegurado);
                if (asegurados.Count() > 0)
                {
                    await this.aseguradoWriter.InsertarBloqueAseguradosAsync(asegurados, ga.CodigoGrupoAsegurado);
                }
            }

            var nuevaCotizacion = await this.informacionNegocioReader.LeerInformacionNegocioAsync(response.CodigoCotizacion);
            var nuevosGruposAsegurados = await this.datosGruposAseguradosProvider.ObtenerGruposAseguradosAsync(nuevaCotizacion.CodigoCotizacion, nuevaCotizacion.CodigoRamo, nuevaCotizacion.CodigoSubramo, nuevaCotizacion.CodigoSector);

            foreach (var nga in nuevosGruposAsegurados)
            {
                await this.datosGruposAseguradosProvider.InsertarValoresGrupoAseguradoAsync(response.CodigoCotizacion, response.Version, nga);
            }

            return new ActionResponseBase
            {
                CodigoCotizacion = response.CodigoCotizacion,
                CodigoEstadoCotizacion = response.CodigoEstadoCotizacion,
                NumeroCotizacion = response.NumeroCotizacion,
                Version = response.Version
            };
        }

        public async Task UpdateDistribucionAsync(int codigoGrupo, int distribucion)
        {
            var queryString = "UPDATE PMP.TB_GrupoAsegurado SET IN_distribucion = @VAR_IN_distribucion WHERE IN_cod_grupo_asegurado = @VAR_IN_cod_grupo;";
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.Add("@VAR_IN_cod_grupo", SqlDbType.Int).Value = codigoGrupo;
                cmd.Parameters.Add("@VAR_IN_distribucion", SqlDbType.Int).Value = distribucion;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("UpdateDistribucionAsync :: UpdateDistribucionAsync", ex);
                }
            }
        }

        public async Task<ActionResponseBase> CopyAltCotizacionAsync(int userId, int codigoCotizacion, int version)
        {
            var response = await this.cotizacionWriterService.CreateVersionAltCotizacionAsync(userId, codigoCotizacion);
            response.CodigoCotizacion = int.Parse(response.NumeroCotizacion);
            var responseUser = this.applicationUserDataProvider.GetUsersAsync().Result;
            var username = responseUser.Where(x => x.UserId == userId).Select(x => x.UserName).FirstOrDefault();
            var responseTran = this.cotizacionTransactionsProvider.CreateTransactionAsync(response.CodigoCotizacion, 777, username, "Ficha Tecnica Alterna");
            var gruposAsegurados = await this.datosGruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);
            var siniestros = await this.datosSiniestralidadReader.GetSiniestralidadAsync(codigoCotizacion);
            var intermediarios = await this.datosIntermediarioReader.GetIntermediariosAsync(codigoCotizacion);



            foreach (var siniestralidad in siniestros)
            {
                siniestralidad.CodigoCotizacion = response.CodigoCotizacion;
                var responseInsSinietralidad = this.datosSiniestralidadWriter.CrearSiniestralidadAsync(siniestralidad);
            }

            foreach (var intermediario in intermediarios)
            {
                intermediario.CodigoCotizacion = response.CodigoCotizacion;
                var responseInsIntermediario = this.datosIntermediarioWriter.CreateIntermediarioAsync(response.CodigoCotizacion, intermediario);
            }

            foreach (var ga in gruposAsegurados)
            {
                ga.CodigoCotizacion = response.CodigoCotizacion;
                var codigoGrupoAsegurado = await this.datosGruposAseguradoWriter.CrearGrupoAseguradoAsync(ga);
                var grupoAsegCod = codigoGrupoAsegurado;
                var distribucion = ga.ConDistribucionAsegurados ? 1 : 0;

                await this.datosGruposAseguradoWriter.InsertarNumAseguradosAsync(grupoAsegCod, ga.NumeroAsegurados, ga.EdadPromedioAsegurados, (int)ga.PorcentajeAsegurados);
                var amparos = await this.amparoGrupoAsguradoReader.LeerAmparoGrupoAseguradoAsync(ga.CodigoGrupoAsegurado);
                await this.UpdateDistribucionAsync(grupoAsegCod, distribucion);
                foreach (AmparoGrupoAsegurado amparo in amparos)
                {
                    amparo.CodigoGrupoAsegurado = grupoAsegCod;
                    var codigoAmparo = await this.amparoGrupoAsguradoWriter.CrearAmparoGrupoAseguradoAsync(amparo);
                    var opciones = await this.opcionValorReader.LeerOpcionValorAseguradoAsync(amparo.CodigoAmparoGrupoAsegurado);
                    
                    foreach (var opcion in opciones)
                    {
                        if (ga.ConDistribucionAsegurados)
                        {
                            ga.AseguradosOpcion1 = opciones.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                            ga.AseguradosOpcion2 = opciones.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                            ga.AseguradosOpcion3 = opciones.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                        }
                        opcion.CodigoAmparoGrupoAsegurado = codigoAmparo;
                        var codigoOpcionValor = await this.opcionValorWriter.CrearOpcionValorAseguradoAsync(opcion);
                        var tasaOpcion = await this.tasaOpcionesReader.LeerTasaOpcionAsync(ga.CodigoGrupoAsegurado, opcion.IndiceOpcion);
                        await this.tasaOpcionesWriter.CrearTasaOpcionAsync(tasaOpcion);
                    }
                    await this.datosGruposAseguradoWriter.ActualizarGrupoAseguradoAsync(grupoAsegCod, ga);
                    var edadesAmparo = await this.edadesReader.LeerEdadesAsync(ga.CodigoGrupoAsegurado, amparo.CodigoAmparo);
                    await this.edadesWriter.InsertarEdadAmparoAsync(grupoAsegCod, amparo.CodigoAmparo, edadesAmparo);
                }

                var asegurados = await this.aseguradoReader.LeerAseguradosAsync(ga.CodigoGrupoAsegurado);
                if (asegurados.Count() > 0)
                {
                    await this.aseguradoWriter.InsertarBloqueAseguradosAsync(asegurados, grupoAsegCod);
                }
            }

            var nuevaCotizacion = await this.informacionNegocioReader.LeerInformacionNegocioAsync(response.CodigoCotizacion);
            var nuevosGruposAsegurados = await this.datosGruposAseguradosProvider.ObtenerGruposAseguradosAsync(nuevaCotizacion.CodigoCotizacion, nuevaCotizacion.CodigoRamo, nuevaCotizacion.CodigoSubramo, nuevaCotizacion.CodigoSector);

            foreach (var nga in nuevosGruposAsegurados)
            {
                await this.datosGruposAseguradosProvider.InsertarValoresGrupoAseguradoAsync(response.CodigoCotizacion, response.Version, nga);
            }

            this.resumenProvider.GenerateAsync(response.CodigoCotizacion, response.Version);

            return new ActionResponseBase
            {
                CodigoCotizacion = response.CodigoCotizacion,
                CodigoEstadoCotizacion = response.CodigoEstadoCotizacion,
                NumeroCotizacion = response.NumeroCotizacion,
                Version = response.Version
            };
        }

        public async Task<ActionResponseBase> ContinueCotizacionAsync(int codigoCotizacion, int userId, string userName, string comments)
        {
            // continue cotizacion
            var response = await this.cotizacionWriterService.ContinueCotizacion(codigoCotizacion, userId, comments);
            // log transaction
            var message = "Continuación de la cotización";
            await this.cotizacionTransactionsProvider.CreateTransactionAsync(codigoCotizacion, response.Version, userName, message, comments);

            return new ActionResponseBase();
        }

        public async Task<ActionResponseBase> CreateVersionCotizacionAsync(int userId, int codigoCotizacion, int versionCopia, string username)
        {
            var versionesCopia = new List<VersionCotizacion>();
            var queryString = "SELECT CT.IN_cod_cotizacion, IN_cod_cotizacion_copia ,in_cod_version_consecutivo, IN_cod_estado_cotizacion, CT.IN_cierre, IN_version_copia FROM PMP.TB_Cotizacion CT INNER JOIN  PMP.TB_VersionCotizacion VC  On CT.IN_cod_cotizacion = VC.IN_cod_cotizacion WHERE VC.IN_cod_cotizacion = @VAR_IN_cod_cotizacion";
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Connection = conn;

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var closed = SqlReaderUtilities.SafeGet<int>(reader, "IN_cierre");
                        //var locked = SqlReaderUtilities.SafeGet<int>(reader, "Bloqueado");
                        var item = new VersionCotizacion
                        {
                            CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                            Version = SqlReaderUtilities.SafeGet<int>(reader, "in_cod_version_consecutivo"),
                            CodigoCotizacionPadre = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion_copia"),
                            VersionPadre = SqlReaderUtilities.SafeGet<int>(reader, "IN_version_copia"),
                            CodigoEstado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion"),
                            Closed = closed == 0 ? false : true,
                        };



                        versionesCopia.Add(item);
                    }


                }

                catch (Exception ex)
                {
                    throw new Exception("CotizacionDataProvider :: Crear Version Script Versiones Anterirores", ex);
                }
            }

            var cotizacionPadre = versionesCopia.FirstOrDefault().VersionPadre == 1 ? versionesCopia.FirstOrDefault().CodigoCotizacionPadre : versionesCopia.FirstOrDefault().CodigoCotizacion;
            var versiones = await this.GetVersionesCotizacionAsync(cotizacionPadre);

            var transactions = this.cotizacionTransactionsProvider.GetTransactionsAsync(codigoCotizacion, versionCopia).Result;
            var firstTransaction = transactions.Transactions.FirstOrDefault();
            var copia = firstTransaction.Description == "COPIADO" ? true : false;



            if (versiones.Count() > 0)
            {
                var isValid = versiones.All(x => x.Closed);
                if (!isValid)
                {
                    return ActionResponseBase.CreateInvalidResponse("No es posible crear una versión de esta cotización. Verifique que todas sus versiones estén cerradas.");
                }
            }

            // Guardar cotizacion nueva version
            var response = await this.cotizacionWriterService.CreateVersionCotizacionAsync(userId, cotizacionPadre, versionCopia, copia);
            var gruposAsegurados = await this.datosGruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);
            var infoNegocioPadre = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            await this.informacionNegocioWriter.ActualizarInformacionNegocioAsync(response.CodigoCotizacion, infoNegocioPadre);
            await this.informacionNegocioWriter.ActualizarDirectorComercialAsync(response.CodigoCotizacion, infoNegocioPadre.UsuarioDirectorComercial, infoNegocioPadre.NombreDirectorComercial, infoNegocioPadre.EmailDirectorComercial);
            var intermediarios = await this.datosIntermediarioReader.GetIntermediariosAsync(codigoCotizacion);
            var siniestros = await this.datosSiniestralidadReader.GetSiniestralidadAsync(codigoCotizacion);

            foreach (var intermediario in intermediarios)
            {
                intermediario.CodigoCotizacion = response.CodigoCotizacion;
                await this.datosIntermediarioWriter.CreateIntermediarioAsync(response.CodigoCotizacion, intermediario);
            }

            foreach (var ga in gruposAsegurados)
            {
                ga.CodigoCotizacion = response.CodigoCotizacion;
                var codigoGrupoAsegurado = await this.datosGruposAseguradoWriter.CrearGrupoAseguradoAsync(ga);
                await this.datosGruposAseguradoWriter.ActualizarGrupoAseguradoAsync(codigoGrupoAsegurado, ga);
                await this.datosGruposAseguradoWriter.InsertarNumAseguradosAsync(codigoGrupoAsegurado, ga.NumeroAsegurados, ga.EdadPromedioAsegurados, (int)ga.PorcentajeAsegurados);
                var amparos = await this.amparoGrupoAsguradoReader.LeerAmparoGrupoAseguradoAsync(ga.CodigoGrupoAsegurado);
                foreach (var amparo in amparos)
                {
                    amparo.CodigoGrupoAsegurado = codigoGrupoAsegurado;
                    var codigoAmparoGrupoAsegurado = await this.amparoGrupoAsguradoWriter.CrearAmparoGrupoAseguradoAsync(amparo);
                    var opciones = await this.opcionValorReader.LeerOpcionValorAseguradoAsync(amparo.CodigoAmparoGrupoAsegurado);
                    foreach (var opcion in opciones)
                    {
                        opcion.CodigoAmparoGrupoAsegurado = codigoAmparoGrupoAsegurado;
                        if (ga.ConDistribucionAsegurados)
                        {
                            ga.AseguradosOpcion1 = opciones.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                            ga.AseguradosOpcion2 = opciones.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                            ga.AseguradosOpcion3 = opciones.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                        }
                        await this.opcionValorWriter.CrearOpcionValorAseguradoAsync(opcion);
                        var tasaOpcion = await this.tasaOpcionesReader.LeerTasaOpcionAsync(ga.CodigoGrupoAsegurado, opcion.IndiceOpcion);
                        tasaOpcion.CodigoGrupoAsegurado = codigoGrupoAsegurado;
                        await this.tasaOpcionesWriter.CrearTasaOpcionAsync(tasaOpcion);
                    }

                    var edadeAmparo = await this.edadesReader.LeerEdadesAsync(ga.CodigoGrupoAsegurado, amparo.CodigoAmparo);
                    await this.edadesWriter.InsertarEdadAmparoAsync(codigoGrupoAsegurado, amparo.CodigoAmparo, edadeAmparo);
                }

                var asegurados = await this.aseguradoReader.LeerAseguradosAsync(ga.CodigoGrupoAsegurado);
                if (asegurados.Count() > 0)
                {
                    await this.aseguradoWriter.InsertarBloqueAseguradosAsync(asegurados, codigoGrupoAsegurado);
                }
            }

            var nuevaCotizacion = await this.informacionNegocioReader.LeerInformacionNegocioAsync(response.CodigoCotizacion);
            var nuevosGruposAsegurados = await this.datosGruposAseguradosProvider.ObtenerGruposAseguradosAsync(nuevaCotizacion.CodigoCotizacion, codigoCotizacion, nuevaCotizacion.CodigoRamo, nuevaCotizacion.CodigoSubramo, nuevaCotizacion.CodigoSector);

            foreach (var nga in nuevosGruposAsegurados)
            {
                await this.datosGruposAseguradosProvider.InsertarValoresGrupoAseguradoAsync(response.CodigoCotizacion, response.Version, nga);
            }

            foreach (var siniestralidad in siniestros)
            {
                siniestralidad.CodigoCotizacion = response.CodigoCotizacion;
                await this.datosSiniestralidadWriter.CrearSiniestralidadAsync(siniestralidad);
            }


            await this.cotizacionTransactionsProvider.CreateTransactionAsync(response.CodigoCotizacion, response.Version, username, "VERSION");

            return new ActionResponseBase
            {
                CodigoCotizacion = response.CodigoCotizacion,
                CodigoEstadoCotizacion = response.CodigoEstadoCotizacion,
                NumeroCotizacion = response.NumeroCotizacion,
                Version = response.Version
            };
        }

        public async Task<IEnumerable<CotizacionItemList>> GetPendingAuthorizationCotizacionesAsync(string userName)
        {
            var cotizaciones = await this.datosCotizacionReader.GetCotizacionesAsync(userName);
            var result = cotizaciones.Where(x => x.CodigoEstado == (int)CotizacionState.PendingAuthorization);
            await this.datosCotizacionUtilities.AgreggateCotizacionModelAsync(result);
            // Update filtered data, remove after SP transaction implementation
            return result;
        }

        public async Task<OpenCotizacionResponse> OpenCotizacionAsync(int codigoCotizacion, int version, string userName, int userId)
        {
            try
            {
                var businessData = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                var canContinueResponse = this.ValidateCanOpenCotizacion(businessData, userId, userName);
                if (canContinueResponse.IsValid)
                {
                    if (businessData.LastAuthorName == userName)
                    {
                        await this.LockCotizacionAsync(codigoCotizacion, version, userId, userName);
                    }
                    var response = await FetchCotizacionAsync(codigoCotizacion, version);
                    response.ErrorMessage = canContinueResponse.Message;
                    response.Readonly = canContinueResponse.Status == "readonly";

                    return response;
                }
                else
                {
                    return new OpenCotizacionResponse
                    {
                        CodigoCotizacion = codigoCotizacion,
                        Version = businessData.Version,
                        CodigoEstadoCotizacion = businessData.CodigoEstadoCotizacion,
                        NumeroCotizacion = businessData.NumeroCotizacion,
                        ErrorMessage = canContinueResponse.Message,
                        Status = ResponseStatus.Invalid
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CotizacionDataProvider :: GetCotizacionAsync", ex);
            }
        }

        public async Task<OpenCotizacionResponse> FetchCotizacionAsync(int codigoCotizacion, int version)
        {
            var businessData = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var tomador = await this.tomadorReader.GetTomadorAsync(codigoCotizacion);
            businessData.Actividad = tomador.Actividad;
            var tiposSumaAsegurada = await this.datosPersonasReader.TraerTiposSumaAsegurada(businessData.CodigoRamo, businessData.CodigoSubramo);
            var informacionGruposAsegurados = await this.datosGruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);
            var informacionIntermediarios = await this.datosIntermediarioReader.GetIntermediariosAsync(codigoCotizacion);
            var informacionSiniestralidad = await this.datosSiniestralidadReader.GetSiniestralidadAsync(codigoCotizacion);
            var informacionTomador = await this.datosTomadorReader.GetTomadorAsync(codigoCotizacion);
            var basicDataViewModel = await this.BuildInformacionBasicaAsync(businessData);
            var businessDataViewModel = this.BuildInformacionNegocio(businessData);
            var insuredDataViewModel = await this.BuildInformacionTomadorAsync(informacionTomador);
            var insuredGroupsDataViewModel = this.BuildInformacionAsegurados(informacionGruposAsegurados, tiposSumaAsegurada);
            var intermediariesDataViewModel = this.BuildInformacionIntermediarios(informacionIntermediarios);
            var accidentRateDataViewModel = this.BuildInformacionSiniestraldiad(informacionSiniestralidad);

            var user = await this.applicationUserDataProvider.GetUserAsync(businessData.LastAuthorName);

            var result = new CotizacionViewModel
            {
                CodigoCotizacion = codigoCotizacion,
                CodigoEstadoCotizacion = businessData.CodigoEstadoCotizacion,
                NumeroCotizacion = businessData.NumeroCotizacion,
                Version = businessData.Version,
                UsuarioNotificado = businessData.UsuarioNotificado,
                User = user,
                LastAuthorId = businessData.LastAuthorId,
                LastAuthorName = businessData.LastAuthorName,
                InformacionBasica = basicDataViewModel,
                InformacionNegocio = businessDataViewModel,
                InformacionBasicaTomador = insuredDataViewModel,
                InformacionGruposAsegurados = insuredGroupsDataViewModel,
                InformacionIntermediarios = intermediariesDataViewModel,
                InformacionSiniestralidad = accidentRateDataViewModel,
                Blocked = businessData.Bloqueado
            };

            return new OpenCotizacionResponse
            {
                CodigoCotizacion = codigoCotizacion,
                Version = version,
                CodigoEstadoCotizacion = businessData.CodigoEstadoCotizacion,
                NumeroCotizacion = businessData.NumeroCotizacion,
                Data = result
            };
        }

        private CanOpenCotizacionResponse ValidateCanOpenCotizacion(InformacionNegocio businessData, int userId, string userName)
        {

            var status = "";
            var message = "";
            var cotizacionState = (CotizacionState)businessData.CodigoEstadoCotizacion;
            //businessData.LastAuthorId = userId;
            //this.cotizacionWriterService.UpdateLastAuthorCotizacionAsync(businessData.CodigoCotizacion, userId);
            // validate permission to open cotizacion
            if (cotizacionState.Equals(CotizacionState.PendingAuthorization))
            {
                status = "readonly";
                // status = "pending_authorization";
                message = "La cotización esta pendiente de autorización y no puede ser modificada hasta que sea confirmada.";
                if (businessData.UsuarioNotificado == userName)
                {
                    return new CanOpenCotizacionResponse
                    {
                        IsValid = true,
                        Status = status
                    };
                }
            }
            if (userId != businessData.LastAuthorId)
            {
                status = "readonly";
                // status = "unauthorized";
                message = $"No es posible modificar la cotización <b>{businessData.NumeroCotizacion}</b>, se encuentra bloqueada por <b>{businessData.LastAuthorName}</b>.";
            }
            if (businessData.CodigoEstadoCotizacion == (int)CotizacionState.Accepted)
            {
                status = "readonly";
                // status = "confirmed";
                message = $"La cotización {businessData.NumeroCotizacion} fue aceptada por <b>{businessData.LastAuthorName}</b> y se encuentra en proceso de expedición y no puede ser modificada.";
            }
            if (businessData.CodigoEstadoCotizacion == (int)CotizacionState.Closed)
            {
                status = "readonly";
                // status = "confirmed";
                message = $"La cotización <b>{businessData.NumeroCotizacion}</b> se encuentra cerrada y no puede ser modificada.";
            }
            if (businessData.CodigoEstadoCotizacion == (int)CotizacionState.Expired)
            {
                status = "readonly";
                message = $"La cotización <b>{businessData.NumeroCotizacion}</b> se encuentra cerrada/vencida y no puede ser modificada.";
            }
            if (businessData.CodigoEstadoCotizacion == (int)CotizacionState.RejectedByClient)
            {
                status = "readonly";
                message = $"La cotización <b>{businessData.NumeroCotizacion}</b> se encuentra rechazada por el cliente y no puede ser modificada.";
            }
            if (businessData.CodigoEstadoCotizacion == (int)CotizacionState.RejectedByCompany)
            {
                status = "readonly";
                // status = "confirmed";
                message = $"La cotización <b>{businessData.NumeroCotizacion}</b> se encuentra rechazada por la compañía y no puede ser modificada.";
            }
            if (businessData.CodigoEstadoCotizacion == (int)CotizacionState.Sent)
            {
                status = "readonly";
                // status = "confirmed";
                message = $"La cotización esta en estado <b>Enviada</b> y puede ser consultada en modo de solo lectura.";
            }
            if (businessData.CodigoEstadoCotizacion == (int)CotizacionState.Issued)
            {
                status = "readonly";
                message = $"La cotización esta en estado <b>Expedida</b> y puede ser consultada en modo de solo lectura.";
            }
            if (businessData.CodigoEstadoCotizacion == (int)CotizacionState.ExpeditionRequest)
            {
                status = "readonly";
                message = $"La cotización esta en estado <b>Solicitud Expedición</b> y puede ser consultada en modo de solo lectura.";
            }
            // override response values
            return new CanOpenCotizacionResponse
            {
                IsValid = true,
                Status = status,
                // Message = "El estado de la aplicación permite acceder en modeo de solo lectura.",
                Message = message
                // Details = message
            };
        }

        public async Task<CotizacionTasasResponse> FetchTasasAsync(int codigoCotizacion, int version)
        {
            try
            {
                var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                var _tasas = await this.datosPersonasReader.TraerTasasAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector);
                var grupos = await this.datosGruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);
                var grupo = grupos.FirstOrDefault();
                var tasaOpcion = await this.tasaOpcionesReader.LeerTasaOpcionAsync(grupo.CodigoGrupoAsegurado, 1);
                var tasaGrupoInfo = _tasas.Where(x => x.CodigoTasa == informacionNegocio.CodigoTipoTasa1).FirstOrDefault();
                var tasa = new CotizacionTasa
                {
                    CodigoTipoTasa = tasaGrupoInfo.CodigoTasa,
                    NombreTasa = tasaGrupoInfo.NombreTasa,
                    Value = informacionNegocio.CodigoTipoTasa1 != 5 ? tasaOpcion.TasaComercialTotal : tasaOpcion.TasaSiniestralidadTotal
                };

                var tasas = new List<CotizacionTasa> { tasa };
                if (informacionNegocio.CodigoTipoTasa2 > 0)
                {
                    var tasa2Info = _tasas.Where(x => x.CodigoTasa == informacionNegocio.CodigoTipoTasa2).FirstOrDefault();
                    var valorTasa2 = informacionNegocio.CodigoTipoTasa2 != 5 ? tasaOpcion.TasaComercialTotal : tasaOpcion.TasaSiniestralidadTotal;
                    var tasa2 = new CotizacionTasa
                    {
                        CodigoTipoTasa = tasa2Info.CodigoTasa,
                        NombreTasa = tasa2Info.NombreTasa,
                        Value = valorTasa2
                    };

                    tasas.Add(tasa2);
                }

                return new CotizacionTasasResponse
                {
                    Tasas = tasas
                };
            }
            catch (Exception ex)
            {
                throw new Exception("CotizacionDataProvider :: FetchTasasAsync", ex);
            }
        }

        public async Task<ActionResponseBase> UpdateSelectedTasaAsync(int codigoCotizacion, int version, UpdateCotizacionDataArgs args)
        {
            try
            {
                await this.informacionNegocioWriter.UpdateCotizacionSelectedTasaAsync(codigoCotizacion, args.TasaInfo.Value);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                return ActionResponseBase.CreateInvalidResponse(ex.Message);
            }
        }

        public async Task<ActionResponseBase> ConfirmCotizacionAsync(int codigoCotizacion, ConfirmCotizacionArgs args)
        {
            // confirm cotizacion
            var response = await this.cotizacionWriterService.ConfirmCotizacionAsync(codigoCotizacion, args.CausalId, args.UserId, args.Action);
            //TODO getCausalById
            var responseCausal = await this.causalesConfirmacionReader.GetCausalId(args.CausalId);
            var causal = responseCausal.CausalTexto;
            // change cotizacion state
            var cotizacionState = (CotizacionState)response.CodigoEstadoCotizacion;
            var nextState = args.Action == ConfirmCotizacionAction.Accepted ? CotizacionState.Accepted :
                args.Action == ConfirmCotizacionAction.RejectedByCompany ? CotizacionState.RejectedByCompany :
                args.Action == ConfirmCotizacionAction.RejectedByClient ? CotizacionState.RejectedByClient : CotizacionState.Closed;
            if (nextState < CotizacionState.RejectedByClient)
            {
                await this.cotizacionStateUpdater.UpdateCotizacionStateAsync(codigoCotizacion, nextState);
            }

            // log transaction
            var message = args.Action == ConfirmCotizacionAction.Accepted ? "Aceptación de la Cotización" :
                args.Action == ConfirmCotizacionAction.RejectedByCompany ? "Rechazo de cotización por Compañía" :
                args.Action == ConfirmCotizacionAction.RejectedByClient ? "Rechazo de cotización por Cliente" : "Rechazo de cotización";
            await this.cotizacionTransactionsProvider.CreateTransactionAsync(codigoCotizacion, response.Version, args.UserName, message, args.Observaciones);

            await this.CloseVersionesCotizacionAsync(codigoCotizacion, args);

            var comment = args.Observaciones;
            var userInfo = await this.soligesproUsersDataReader.GetUserAsync(args.UserName);
            var recipients = new string[] { args.To };
            var withCopy = args.WithCopy;

            await this.SendEmailAsync(codigoCotizacion, comment, recipients, withCopy, causal, args.Action);

            return new ActionResponseBase();
        }

        public async Task<ActionResponseBase> CloseVersionesCotizacionAsync(int codigoCotizacion, ConfirmCotizacionArgs args)
        {
            //Get versiones
            var versiones = await this.GetVersionesCotizacionAsync(codigoCotizacion);

            foreach (var version in versiones)
            {
                if (version.CodigoCotizacion != codigoCotizacion)
                {
                    await this.cotizacionWriterService.CambiarEstadoAsync(version.CodigoCotizacion, CotizacionState.Closed);
                    var message = args.Action == ConfirmCotizacionAction.Accepted ? "Cotización cerrada por Aceptación de otra versión" : "Cotización cerrada por Rechazo de otra versión";
                    await this.cotizacionTransactionsProvider.CreateTransactionAsync(version.CodigoCotizacion, version.Version, args.UserName, message);
                }
            }

            return new ActionResponseBase();

        }

        public async Task<FirmasRechazoAceptacionResponse> GetFirmasRechazoAceptacion(int codigoCotizacion)
        {
            var infoNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var sucursal = await this.informacionPersonasReader.TraerSucursalAsync(infoNegocio.CodigoSucursal);
            var zona = await this.informacionPersonasReader.GetZonaByCodigoAsync(infoNegocio.CodigoZona);
            var tomador = await this.GetTomadorAsync(codigoCotizacion);
            var firmas = await this.BuildFirmasRechazoAceptacion(sucursal, zona, infoNegocio.NombreDirectorComercial);
            if (infoNegocio.NombreDirectorComercial == null) {
                firmas = firmas.Where(o=> o.CodigoCargo != 16);
            }
            var firmasRechazoAceptacionResponse = new FirmasRechazoAceptacionResponse
            {
                Tomador = tomador,
                Firmas = firmas,
                OcultarDirector = infoNegocio.NombreDirectorComercial == null ? true : false
            };

            return firmasRechazoAceptacionResponse;

        }

        private async Task<TomadorCotizacion> GetTomadorAsync(int codigoCotizacion)
        {
            // conulta informacion de tomador
            var informacionTomador = await this.tomadorReader.GetTomadorAsync(codigoCotizacion);
            var nombre = informacionTomador.CodigoTipoDocumento == 3 ? informacionTomador.PrimerApellido
                : $"{informacionTomador.Nombres } {informacionTomador.PrimerApellido} {informacionTomador.SegundoApellido}";

            var tomador = new TomadorCotizacion
            {
                Nombre = nombre,
                Email = informacionTomador.Email,
            };

            // consulta infomacion de intermediario
            var intermediarios = await this.intermediarioReader.GetIntermediariosAsync(codigoCotizacion);
            var hasintermediarios = intermediarios.Count() > 0;
            if (hasintermediarios)
            {
                // retorna el intermediario con mayor participacion, o en su defecto el primer intermediario si la participacion es igual
                var maxParticipacion = intermediarios.Max(x => x.Participacion);
                var intermediario = intermediarios.Where(x => x.Participacion == maxParticipacion).FirstOrDefault();
                tomador = new TomadorCotizacion
                {
                    Nombre = $"{intermediario.PrimerNombre} {intermediario.SegundoNombre} {intermediario.PrimerApellido} {intermediario.SegundoApellido}",
                    Email = intermediario.Email,
                    EsIntermediario = true
                };
            }

            return tomador;
        }

        private async Task<IEnumerable<DirectorCotizacion>> BuildFirmasRechazoAceptacion(Sucursal sucursal, Zona zona, string dt)
        {
            var contacts = await this.soligesproUsersDataReader.GetDirectoresAsync(sucursal.CodigoSucursal, zona.CodigoZona);
            if (dt == null)
            {
                var gerente = await this.soligesproUsersDataReader.GetUserGerenteAsync(sucursal.CodigoSucursal);
                dt = gerente.NombreUsuario;

            }
            var trueDt = contacts.Where(x => x.NombreUsuario == dt).First();
            contacts = contacts.Where(x => x.EmailUsuario != "");
            var result = contacts
                .Where(x => (x.EmailUsuario == trueDt.EmailUsuario && x.NombreUsuario == trueDt.NombreUsuario) || (x.EmailUsuario != trueDt.EmailUsuario && x.CodigoCargo != trueDt.CodigoCargo))
                .Select(y => new DirectorCotizacion { Email = y.EmailUsuario, Nombre = y.NombreUsuario, Cargo = y.Cargo, CodigoCargo = (int)y.CodigoCargo });
            if (trueDt.CodigoCargo == 9)
            {
                result = result.Where(o=> o.CodigoCargo != 16);
            }
            return result;
        }

        private async Task SendEmailAsync(int codigoCotización, string comment, string[] recipients, string[] withCopy, string causal, ConfirmCotizacionAction cotizacionAction)
        {
            var infoNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotización);
            if (cotizacionAction == ConfirmCotizacionAction.Accepted)
            {
                await this.cotizationEmailSender.SendApprovedCotizationEmail(codigoCotización, infoNegocio.NumeroCotizacion, infoNegocio.CodigoRamo, recipients, withCopy, comment, causal, infoNegocio.Version);
            }
            else
            {
                await this.cotizationEmailSender.SendRefuzedCotizationEmail(codigoCotización, infoNegocio.NumeroCotizacion, infoNegocio.CodigoRamo, recipients, withCopy, comment, causal, cotizacionAction, infoNegocio.Version);
            }
        }

        public async Task<ActionResponseBase> LockCotizacionAsync(int codigoCotizacion, int version, int userId, string userName)
        {
            // lock cotizacion
            await this.bloqueoCotizacionWriter.BloquearAsync(codigoCotizacion, userId);
            // TODO evaluate if is necessary log lock event
            // log transaction
            // var message = "Bloqueo de la cotización";
            // await this.cotizacionTransactionsProvider.CreateTransaction(codigoCotizacion, version, userName, message);

            return new ActionResponseBase();
        }

        public async Task<ActionResponseBase> UnlockCotizacionAsync(string userName, int codigoCotizacion, int version)
        {
            // TODO
            var numeroCotizacion = ""; //response.NumeroCotizacion

            // unlock cotizacion
            await this.bloqueoCotizacionWriter.DesbloquearAsync(codigoCotizacion);
            // log transaction
            var message = $"Desbloqueo de la cotización {numeroCotizacion}";
            // TODO register transaction every time?
            // await this.cotizacionTransactionsProvider.CreateTransaction(codigoCotizacion, version, userName, message);

            return new ActionResponseBase();
        }

        private async Task<InformacionBasicaTomadorViewModel> BuildInformacionTomadorAsync(Tomador informacionTomador)
        {
            var departamentos = await this.parametrizacionReader.TraerDepartamentosAsync();

            var nombre = informacionTomador.Nombres != null ? informacionTomador.Nombres : " ";
            var nombres = nombre.Split(" ");

            return new InformacionBasicaTomadorViewModel
            {
                //ActividadEconomica = 
                CodigoTomador = informacionTomador.CodigoTomador,
                CodigoDepartamento = informacionTomador.CodigoDepartamento,
                CodigoActividadEconomica = informacionTomador.CodigoActividad,
                Direccion = informacionTomador.Direccion,
                Email = informacionTomador.Email,
                CodigoMunicipio = informacionTomador.CodigoMunicipio,
                NombreContacto = informacionTomador.NombreContacto,
                NumeroDocumento = informacionTomador.NumeroDocumento,
                CodigoPais = informacionTomador.CodigoPais,
                PrimerApellido = informacionTomador.PrimerApellido,
                PrimerNombre = nombres[0],
                SegundoApellido = informacionTomador.SegundoApellido,
                SegundoNombre = nombres[1],
                //Telefono = informacionTomador.
                TelefonoContacto1 = informacionTomador.Telefono1Contacto,
                TelefonoContacto2 = informacionTomador.Telefono2Contacto,
                CodigoTipoDocumento = informacionTomador.CodigoTipoDocumento,
                TomadorSlip = informacionTomador.TomadorSlip
            };
        }

        private InformacionSiniestralidadViewModel BuildInformacionSiniestraldiad(IEnumerable<Siniestralidad> informacionSiniestralidad)
        {
            return new InformacionSiniestralidadViewModel
            {
                InformacionSiniestralidad = informacionSiniestralidad
            };
        }

        private InformacionGruposAseguradosViewModel BuildInformacionAsegurados(IEnumerable<GrupoAsegurado> informacionGruposAsegurados, IEnumerable<PersonasServiceReference.TipoSumaAsegurada> tiposSumaAsegurada)
        {
            var gruposAsegurados = new List<GrupoAseguradoViewModel>();
            foreach (var g in informacionGruposAsegurados)
            {
                var amparos = this.amparoGrupoAsguradoReader.LeerAmparoGrupoAseguradoAsync(g.CodigoGrupoAsegurado).Result;
                var tipoSumaAsegurada = tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == g.CodigoTipoSuma).FirstOrDefault();
                var grupoAseguradoViewModel = new GrupoAseguradoViewModel
                {
                    CodigoCotizacion = g.CodigoCotizacion,
                    CodigoGrupoAsegurado = g.CodigoGrupoAsegurado,
                    CodigoTipoSuma = g.CodigoTipoSuma,
                    TipoSumaAsegurada = tipoSumaAsegurada,
                    //ConListaAsegurados = g.ConListaAsegurados,
                    NombreGrupoAsegurado = g.NombreGrupoAsegurado,
                    NumeroAsegurados = g.NumeroAsegurados,
                    NumeroSalariosAsegurado = g.NumeroSalariosAsegurado,
                    PorcentajeAsegurados = g.PorcentajeAsegurados,
                    ValorAsegurado = g.ValorAsegurado,
                    ValorMaxAsegurado = g.ValorMaxAsegurado,
                    ValorMinAsegurado = g.ValorMinAsegurado,
                    Configured = amparos.Count() > 0
                };

                gruposAsegurados.Add(grupoAseguradoViewModel);
            }

            return new InformacionGruposAseguradosViewModel
            {
                GruposAsegurados = gruposAsegurados
            };
        }

        private InformacionIntermediariosViewModel BuildInformacionIntermediarios(IEnumerable<ModernizacionPersonas.Entities.Intermediario> informacionIntermediarios)
        {
            return new InformacionIntermediariosViewModel
            {
                Intermediarios = informacionIntermediarios
            };
        }

        private async Task<InformacionBasicaViewModel> BuildInformacionBasicaAsync(InformacionNegocio informacionNegocio)
        {
            var sucursal = await this.datosPersonasReader.TraerSucursalAsync(informacionNegocio.CodigoSucursal);
            var ramo = await this.datosPersonasReader.TraerRamoAsync(informacionNegocio.CodigoRamo);
            var subramo = await this.datosPersonasReader.TraerSubRamoAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo);
            return new InformacionBasicaViewModel
            {
                CodigoSucursal = informacionNegocio.CodigoSucursal,
                Sucursal = sucursal,
                CodigoRamo = informacionNegocio.CodigoRamo,
                Ramo = ramo,
                CodigoSubramo = informacionNegocio.CodigoSubramo,
                Subramo = subramo,
                CodigoZona = informacionNegocio.CodigoZona
            };
        }

        private InformacionNegocioViewModel BuildInformacionNegocio(InformacionNegocio informacionNegocio)
        {
            return new InformacionNegocioViewModel
            {
                Sector = informacionNegocio.CodigoSector,
                CodigoPerfilEdad = informacionNegocio.CodigoPerfilEdad,
                CodigoPerfilValor = informacionNegocio.CodigoPerfilValor,
                CodigoTipoTasa1 = informacionNegocio.CodigoTipoTasa1,
                CodigoTipoTasa2 = informacionNegocio.CodigoTipoTasa2,
                ConListaAsegurados = informacionNegocio.ConListaAsegurados,
                EsNegocioDirecto = informacionNegocio.EsNegocioDirecto,
                FechaFin = informacionNegocio.FechaFin,
                FechaInicio = informacionNegocio.FechaInicio,
                NombreAseguradora = informacionNegocio.NombreAseguradora,
                OtrosGastos = informacionNegocio.OtrosGastos,
                GastosCompania = informacionNegocio.PorcentajeGastosCompania,
                UtilidadesCompania = informacionNegocio.UtilidadCompania,
                CodigoPeriodoFacturacion = informacionNegocio.CodigoPeriodoFacturacion,
                PorcentajeComision = informacionNegocio.PorcentajeComision,
                PorcentajeOtrosGastos = informacionNegocio.PorcentajeOtrosGastos,
                PorcentajeRetorno = informacionNegocio.PorcentajeRetorno,
                CodigoTipoContratacion = informacionNegocio.CodigoTipoContratacion,
                CodigoTipoNegocio = informacionNegocio.CodigoTipoNegocio,
                CodigoTipoRiesgo = informacionNegocio.CodigoTipoRiesgo,
                LastAuthorId = informacionNegocio.LastAuthorId,
                LastAuthorName = informacionNegocio.LastAuthorName,
                UsuarioDirectorComercial = informacionNegocio.UsuarioDirectorComercial,
                NombreDirectorComercial = informacionNegocio.NombreDirectorComercial,
                EmailDirectorComercial = informacionNegocio.EmailDirectorComercial,
                Actividad = informacionNegocio.Actividad,
                AnyosSiniestralidad = informacionNegocio.anyosSiniestralidad
            };
        }
    }
}

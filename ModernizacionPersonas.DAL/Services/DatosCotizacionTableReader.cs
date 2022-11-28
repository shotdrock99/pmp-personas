using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosCotizacionTableReader : IDatosCotizacionReader
    {
        private readonly ITransactionsDataReader transactionsReader;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly ITransactionsDataReader cotizacionTransactionsProvider;
        public DatosCotizacionTableReader()
        {
            this.transactionsReader = new TransactionsDataTableReader();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.cotizacionTransactionsProvider = new TransactionsDataTableReader();
        }
            const string SP_NAME = "PMP.USP_TB_Cotizacion";

        public async Task<IEnumerable<VersionCotizacion>> GetVersionesCotizacionAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 19;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Connection = conn;

                var result = new List<VersionCotizacion>();
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
                            Version = SqlReaderUtilities.SafeGet<int>(reader, "in_cod_version"),
                            CodigoCotizacionPadre = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion_copia"),
                            CodigoEstado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion"),
                            Closed = closed == 0 ? false : true,
                        };



                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableReader :: ObtenerListaCotizacionesAsync", ex);
                }
            }
        }
        public async Task<VersionCotizacion> GetCotizacionPadreAsync(int codigoCotizacion, int versionPadre)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 24;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_version_copia", SqlDbType.Int).Value = versionPadre;
                cmd.Connection = conn;

                var result = new VersionCotizacion();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var item = new VersionCotizacion
                        {
                            CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                            Version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version_consecutivo"),
                        };

                        result = item;
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableReader :: GetCotizacionPadreAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<VersionCotizacion>> GetVersionesCotizacionQueryAsync(int codigoCotizacion)
        {
            var queryString = "SELECT CT.IN_cod_cotizacion, IN_cod_cotizacion_copia ,in_cod_version_consecutivo, IN_cod_estado_cotizacion, CT.IN_cierre, IN_version_copia FROM PMP.TB_Cotizacion CT INNER JOIN  PMP.TB_VersionCotizacion VC  On CT.IN_cod_cotizacion = VC.IN_cod_cotizacion WHERE VC.IN_cod_cotizacion = @VAR_IN_cod_cotizacion";
            var info = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var transactions = this.cotizacionTransactionsProvider.GetTransactionsAsync(codigoCotizacion, info.Version).Result;
            var firstTransaction = transactions.Transactions.FirstOrDefault();
            if (firstTransaction.Description == "COPIADO" && info.Version == 1)
            {
                queryString = "UPDATE PMP.TB_VersionCotizacion SET IN_cod_cotizacion_copia = @VAR_IN_cod_cotizacion WHERE IN_cod_cotizacion = @VAR_IN_cod_cotizacion SELECT CT.IN_cod_cotizacion, IN_cod_cotizacion_copia ,in_cod_version_consecutivo, IN_cod_estado_cotizacion, CT.IN_cierre, IN_version_copia FROM PMP.TB_Cotizacion CT INNER JOIN  PMP.TB_VersionCotizacion VC  On CT.IN_cod_cotizacion = VC.IN_cod_cotizacion WHERE VC.IN_cod_cotizacion = @VAR_IN_cod_cotizacion";
            }

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand(queryString, conn);

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Connection = conn;

                var result = new List<VersionCotizacion>();
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
                            CodigoEstado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion"),
                            VersionPadre = SqlReaderUtilities.SafeGet<int>(reader, "IN_version_copia"),
                            Closed = closed == 0 ? false : true,
                        };



                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableReader :: ObtenerListaCotizacionesAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<VersionCotizacion>> GetVersionesCotizacionQueryAsync(int codigoCotizacion, int version)
        {
            var queryString = "SELECT CT.IN_cod_cotizacion, IN_cod_cotizacion_copia ,in_cod_version_consecutivo, IN_cod_estado_cotizacion, CT.IN_cierre, IN_version_copia FROM PMP.TB_Cotizacion CT INNER JOIN  PMP.TB_VersionCotizacion VC  On CT.IN_cod_cotizacion = VC.IN_cod_cotizacion WHERE VC.IN_cod_cotizacion_copia = @VAR_IN_cod_cotizacion AND VC.in_cod_version_consecutivo = @VAR_IN_version";

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand(queryString, conn);

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_version", SqlDbType.Int).Value = version;
                cmd.Connection = conn;

                var result = new List<VersionCotizacion>();
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
                            CodigoEstado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion"),
                            VersionPadre = SqlReaderUtilities.SafeGet<int>(reader, "IN_version_copia"),
                            Closed = closed == 0 ? false : true,
                        };



                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableReader :: ObtenerListaCotizacionesAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<CotizacionItemList>> GetCotizacionesAsync()
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;

                var result = new List<CotizacionItemList>();
                
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var closed = SqlReaderUtilities.SafeGet<int>(reader, "IN_cierre");
                        //var locked = SqlReaderUtilities.SafeGet<int>(reader, "Bloqueado");
                        var item = new CotizacionItemList
                        {
                            CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                            // Version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version"),
                            FechaCreacion = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_creacion"),
                            NumeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion"),
                            CodigoSucursal = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_agencia"),
                            CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo"),
                            CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo"),
                            CodigoTomador = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tomador"),
                            // NombreTomador = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_tomador"),
                            CodigoEstado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion"),
                            UsuarioNotificado = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario_notificado"),
                            CodigoZona = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_zona"),
                            // FechaModificcion = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_ultimo_movimiento"),
                            Closed = closed == 0 ? false : true,
                            // Locked = locked == 0 ? false : true,
                            // LastAuthor = SqlReaderUtilities.SafeGet<string>(reader, "VC_username_ultima_modificacion"),
                            UsuarioAutorizador = SqlReaderUtilities.SafeGet<string>(reader, "UsuarioAutoriza"),
                        };

                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableReader :: ObtenerListaCotizacionesAsync", ex);
                }
            }
        }


        public async Task<IEnumerable<CotizacionItemList>> GetCotizacionesAsync(CotizacionFilter filtros)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = filtros.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_VC_numero_cotizacion", SqlDbType.VarChar).Value = filtros.NumeroCotizacion;
                cmd.Parameters.Add("@VAR_DA_fecha_inicio_vigencia", SqlDbType.DateTime).Value = filtros.FechaDesde;
                cmd.Parameters.Add("@VAR_DA_fecha_fin_vigencia", SqlDbType.DateTime).Value = filtros.FechaHasta;
                cmd.Parameters.Add("@VAR_IN_cod_estado_cotizacion", SqlDbType.Int).Value = filtros.CodigoEstado;
                cmd.Parameters.Add("@VAR_IN_cod_agencia", SqlDbType.Int).Value = filtros.CodigoSucursal;
                cmd.Parameters.Add("@VAR_IN_cod_zona", SqlDbType.Int).Value = filtros.CodigoZona;
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = filtros.CodigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = filtros.CodigoSubramo;
                cmd.Parameters.Add("@VAR_IN_cod_usuario", SqlDbType.VarChar).Value = filtros.CodigoUsuario;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_documento", SqlDbType.Int).Value = filtros.CodigoTipoDocumento;
                cmd.Parameters.Add("@VAR_VC_numero_documento", SqlDbType.VarChar).Value = filtros.NumeroDocumento;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 6;
                cmd.Connection = conn;

                var result = new List<CotizacionItemList>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var closed = SqlReaderUtilities.SafeGet<int>(reader, "IN_cierre");
                        var locked = SqlReaderUtilities.SafeGet<int>(reader, "Bloqueado");
                        var item = new CotizacionItemList
                        {
                            CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                            Version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version"),
                            FechaCreacion = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_creacion"),
                            NumeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion"),
                            CodigoSucursal = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_agencia"),
                            CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo"),
                            CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo"),
                            CodigoTomador = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tomador"),
                            NombreTomador = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_tomador"),
                            CodigoEstado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion"),
                            UsuarioNotificado = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario_notificado"),
                            CodigoZona = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_zona"),
                            FechaModificacion = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_ultimo_movimiento"),
                            BtnFichaAlterna = SqlReaderUtilities.SafeGet<int>(reader, "BT_ficha_alterna"),
                            Closed = closed == 0 ? false : true,
                            Locked = locked == 0 ? false : true,
                            LastAuthor = SqlReaderUtilities.SafeGet<string>(reader, "VC_username_ultima_modificacion"),
                            UsuarioAutorizador = SqlReaderUtilities.SafeGet<string>(reader, "UsuarioAutoriza"),
                        };
                                               
                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableReader :: ObtenerListaCotizacionesAsync", ex);
                }
            }
        }


        public async Task<IEnumerable<CotizacionItemList>> GetCotizacionesAsync(string codigoUsuario)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 6;
                cmd.Connection = conn;
                var result = new List<CotizacionItemList>();
                
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var item = new CotizacionItemList
                        {
                            CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                            Version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version"),
                            FechaCreacion = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_creacion"),
                            NumeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion"),
                            CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo"),
                            CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo"),
                            CodigoTomador = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tomador"),
                            CodigoEstado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion"),
                            UsuarioNotificado = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario_notificado"),
                            CodigoZona = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_zona"),
                            UsuarioAutorizador = SqlReaderUtilities.SafeGet<string>(reader, "UsuarioAutoriza"),
                        };

                       

                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableReader :: ObtenerListaCotizacionesAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<CotizacionItemList>> GetPendingAuthorizationCotizacionesAsync(CotizacionFilter filtros)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = filtros.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_VC_numero_cotizacion", SqlDbType.VarChar).Value = filtros.NumeroCotizacion;
                cmd.Parameters.Add("@VAR_DA_fecha_inicio_vigencia", SqlDbType.DateTime).Value = filtros.FechaDesde;
                cmd.Parameters.Add("@VAR_DA_fecha_fin_vigencia", SqlDbType.DateTime).Value = filtros.FechaHasta;
                cmd.Parameters.Add("@VAR_IN_cod_estado_cotizacion", SqlDbType.Int).Value = filtros.CodigoEstado;
                cmd.Parameters.Add("@VAR_IN_cod_agencia", SqlDbType.Int).Value = filtros.CodigoSucursal;
                cmd.Parameters.Add("@VAR_IN_cod_zona", SqlDbType.Int).Value = filtros.CodigoZona;
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = filtros.CodigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = filtros.CodigoSubramo;
                cmd.Parameters.Add("@VAR_VC_cod_usuario_notificado", SqlDbType.VarChar).Value = filtros.CodigoUsuario;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_documento", SqlDbType.Int).Value = filtros.CodigoTipoDocumento;
                cmd.Parameters.Add("@VAR_VC_numero_documento", SqlDbType.VarChar).Value = filtros.NumeroDocumento;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 61;
                cmd.Connection = conn;

                var result = new List<CotizacionItemList>();
                
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var closed = SqlReaderUtilities.SafeGet<int>(reader, "IN_cierre");
                        var locked = SqlReaderUtilities.SafeGet<int>(reader, "Bloqueado");
                        var item = new CotizacionItemList
                        {
                            CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                            Version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version"),
                            FechaCreacion = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_creacion"),
                            NumeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion"),
                            CodigoSucursal = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_agencia"),
                            CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo"),
                            CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo"),
                            CodigoTomador = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tomador"),
                            NombreTomador = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_tomador"),
                            CodigoEstado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion"),
                            UsuarioNotificado = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario_notificado"),
                            CodigoZona = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_zona"),
                            FechaModificacion = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_ultimo_movimiento"),
                            Closed = closed == 0 ? false : true,
                            Locked = locked == 0 ? false : true,
                            LastAuthor = SqlReaderUtilities.SafeGet<string>(reader, "VC_username_ultima_modificacion"),
                            UsuarioRealNotifica = SqlReaderUtilities.SafeGet<int>(reader, "usuario_real_notificado"),                            
                        };

                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableReader :: ObtenerListaCotizacionesAsync", ex);
                }
            }
        }
    }
}

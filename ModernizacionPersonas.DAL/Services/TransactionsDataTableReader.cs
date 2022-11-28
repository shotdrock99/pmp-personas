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
    public class TransactionsDataTableReader : ITransactionsDataReader
    {
        public string command = "PMP.USP_TB_Movimientos";

        public async Task<GetTransactionsResult> GetTransactionsAsync(int codigoCotizacion, int version)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var transactions = new List<CotizacionTransaction>();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var transaction = new CotizacionTransaction
                            {
                                CodigoTransaccion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_movimiento"),
                                CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                                Version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version"),
                                CodigoEstadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion"),
                                CodigoUsuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                                ConteoAutorizaciones = SqlReaderUtilities.SafeGet<int>(reader, "IN_conteo_autorizaciones"),
                                Description = SqlReaderUtilities.SafeGet<string>(reader, "VC_descripcion_movimiento"),
                                CreationDate = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                                UNotificado = SqlReaderUtilities.SafeGet<string>(reader, "VC_usuario_notificado")
                            };

                            transactions.Add(transaction);
                        }

                        reader.NextResult();
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        while (await reader.ReadAsync())
                        {
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                        }

                        return new GetTransactionsResult
                        {
                            CodigoCotizacion = codigoCotizacion,
                            NumeroCotizacion = numeroCotizacion,
                            CodigoEstadoCotizacion = estadoCotizacion,
                            Transactions = transactions
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosMovimientosTableReader :: GetTransactionsAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<CotizacionTransaction>> GetAuthorizationTransactionsAsync(int codigoCotizacion, int version)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 6;
                cmd.Connection = conn;
                var transactions = new List<CotizacionTransaction>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var transaction = new CotizacionTransaction
                        {
                            CodigoTransaccion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_movimiento"),
                            CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                            Version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version"),
                            CodigoEstadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion"),
                            CodigoUsuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            ConteoAutorizaciones = SqlReaderUtilities.SafeGet<int>(reader, "IN_conteo_autorizaciones"),
                            Description = SqlReaderUtilities.SafeGet<string>(reader, "VC_descripcion_movimiento"),
                            CreationDate = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                            UNotificado = SqlReaderUtilities.SafeGet<string>(reader, "VC_usuario_notificado")
                        };

                        transactions.Add(transaction);
                    }

                    return transactions;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosMovimientosTableReader :: GetTransactionAuthorizationsAsync", ex);
                }
            }
        }
    }
}

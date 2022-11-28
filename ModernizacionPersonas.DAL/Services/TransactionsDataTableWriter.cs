using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class TransactionsDataTableWriter : ITransactionsDataWriter
    {
        private const string SP_NAME = "PMP.USP_TB_Movimientos";

        public async Task<int> CreateTransactionAsync(CotizacionTransaction model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = model.Version;
                // cmd.Parameters.Add("@VAR_IN_cod_estado_cotizacion", SqlDbType.Int).Value = model.CodigoEstadoCotizacion;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.CodigoUsuario;
                cmd.Parameters.Add("@VAR_IN_conteo_autorizaciones", SqlDbType.Int).Value = model.ConteoAutorizaciones;
                cmd.Parameters.Add("@VAR_VC_descripcion_movimiento", SqlDbType.VarChar).Value = model.Description;
                cmd.Parameters.Add("@VAR_VC_usuario_notificado", SqlDbType.VarChar).Value = model.UNotificado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var transactionId = await cmd.ExecuteScalarAsync();
                    return (int)transactionId;
                }
                catch (Exception ex)
                {
                    throw new Exception("TransactionsDataTableWriter :: CreateTransactionAsync", ex);
                }
            }
        }

        public async Task UpdateTransactionAsync(CotizacionTransaction model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_movimiento", SqlDbType.Int).Value = model.CodigoTransaccion;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = model.Version;
                cmd.Parameters.Add("@VAR_VC_descripcion_movimiento", SqlDbType.VarChar).Value = model.Description;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.CodigoUsuario;
                cmd.Parameters.Add("@VAR_IN_conteo_autorizaciones", SqlDbType.Int).Value = model.ConteoAutorizaciones;
                cmd.Parameters.Add("@VAR_VC_usuario_notificado", SqlDbType.VarChar).Value = model.UNotificado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("TransactionsDataTableWriter :: UpdateTransactionAsync", ex);
                }
            }
        }

        public async Task DeleteTransactionAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("TransactionsDataTableWriter :: DeleteTransactionAsync", ex);
                }
            }
        }
    }
}

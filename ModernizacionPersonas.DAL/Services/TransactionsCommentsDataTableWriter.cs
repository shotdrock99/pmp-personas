using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class TransactionsCommentsDataTableWriter : ITransactionCommentsDataWriter
    {
        public string command = "PMP.USP_TB_Observaciones";

        public async Task CreateTransactionCommentAsync(TransactionComment model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };

                cmd.Parameters.Add("@VAR_IN_cod_movimiento", SqlDbType.Int).Value = model.TransactionId;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.CodigoUsuario;
                cmd.Parameters.Add("@VAR_VC_rol_autorizacion", SqlDbType.VarChar).Value = model.CodigoRolAutorizacion;
                cmd.Parameters.Add("@VAR_TX_observacion", SqlDbType.Text).Value = model.Message;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_autorizacion", SqlDbType.Int).Value = model.CodigoTipoAutorizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("TransactionsCommentsDataTableWriter :: CreateTransactionCommentAsync", ex);
                }
            }
        }
    }
}

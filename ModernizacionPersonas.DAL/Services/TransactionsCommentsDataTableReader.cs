using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class TransactionsCommentsDataTableReader : ITransactionCommentsDataReader
    {
        public string command = "PMP.USP_TB_Observaciones";


        public async Task<IEnumerable<TransactionComment>> GetTransactionsCommentsAsync(int transactionId)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_movimiento", SqlDbType.Int).Value = transactionId;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var comments = new List<TransactionComment>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var comment = new TransactionComment
                        {
                            TransactionId = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_movimiento"),
                            CodigoRolAutorizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_rol_autorizacion"),
                            CodigoTipoAutorizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_autorizacion"),
                            CodigoUsuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Message = SqlReaderUtilities.SafeGet<string>(reader, "TX_observacion")
                        };

                        comments.Add(comment);
                    }

                    return comments;
                }
                catch (Exception ex)
                {
                    throw new Exception("TransactionsCommentsDataTableReader :: GetTransactionsCommentsAsync", ex);
                }
            }
        }

    }
}
